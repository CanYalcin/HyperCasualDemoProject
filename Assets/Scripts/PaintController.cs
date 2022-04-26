using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaintController : MonoBehaviour
{
    private Camera mainCam;

    private GameManager gameManager;
    
    [SerializeField] private GameObject brushPrefab;
    [SerializeField] private Transform brushContainer;
    [SerializeField] private TMP_Text paintRateText;
    [SerializeField] private RenderTexture renderTexture;

    private Color32 color = new Color32(255, 0, 0, 255);

    private int maxBrushCount = 200;
    private int brushCounter = 0;

    private LayerMask layerMask = 1 << 3;

    private bool saving = false;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (saving)
            return;
        if(gameManager.GetGameStatus() == 2)
        {
#if UNITY_STANDALONE_WIN
            if(Input.GetMouseButton(0))
            {
                Paint();
            }
#elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0) // will only work after player finish the game
            {
                Paint();
            }
#endif
        }
    }

    void Paint()
    {
        Vector3 hitPos = Vector3.zero;
        if (HitTestUVPosition(ref hitPos)) // check if ray hits something
        {
            GameObject brushObj = Instantiate(brushPrefab);         // Instantiate a brush
            brushObj.name = "brush";                                // name it brush. not necessery but i like it
            brushObj.GetComponent<SpriteRenderer>().color = color;  // Set the brush color

            color.a = 85;                                           // Brushes have alpha to have a merging effect when painted over.
            brushObj.transform.parent = brushContainer;             //Add the brush to our container to be wiped later
            brushObj.transform.localPosition = hitPos;              //The position of the brush
            brushObj.transform.localScale = Vector3.one * 3;        //The size of the brush
        }
        brushCounter++;                                             //Add to the max brushes
        if (brushCounter >= maxBrushCount)  //If we reach the max brushes available, flatten the texture and clear the brushes
        {                                                           
            saving = true;
            Invoke("SaveTexture", 0.1f);    // lets save the texture

        }
    }

    bool HitTestUVPosition(ref Vector3 hitPos) // check if touch pos hits the wall and return a boolean value. hitpos is ref to get touch hit position on the wall
    {
        RaycastHit hit;
        Vector3 cursorPos = Vector3.zero;
#if UNITY_STANDALONE_WIN
        cursorPos = new Vector3(Input.mousePosition.x,Input.mousePosition.y,0.0f);
#elif UNITY_ANDROID || UNITY_IOS
        Touch t = Input.touches[0];
        cursorPos = new Vector3(t.position.x, t.position.y, 0.0f);
#endif
        Ray cursorRay = mainCam.ScreenPointToRay(cursorPos);
        if (Physics.Raycast(cursorRay, out hit, 25, layerMask)) // we used layermask to not hit anything unnecessary
        {
            hitPos = hit.point - Vector3.forward * 0.1f; // if we use hit.point directly, hitpos will be inside wall. wall.z = 0.2f. so we took hitPos halfway forward
            return true;
        }
        else // didnt hit anything
            return false;
    }

    [SerializeField] private Material wallMat;
    void SaveTexture()      // we will save the texture. If we dont do this occationally, game would be stack because of the count of the brushes.
    {                       // we will save texture, then erase all brushes. So we can easily continue the painting
        brushCounter = 0; 
        RenderTexture.active = renderTexture;       
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);        // write rendertextures pixels to tex
        tex.Apply(); // dont forget the apply the tex
        var colors = tex.GetPixels32();     // get pixels of tex for checking the rate of completion of painting the wall
        int ct = 0;     // red pixel count
        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i].r > 100 && colors[i].g < 50 && colors[i].b < 50) // check if the pixel mostly red
                ct++;
        }
        paintRateText.text = "%" + (((float)ct / (float)colors.Length) * 100).ToString(); // red pixel count / total pixel count

        wallMat.mainTexture = tex; // change walls materials texture to new texture so we dont lose the paintings we did
        RenderTexture.active = null;

        foreach (Transform child in brushContainer.transform) // destroy all the brushes we instantiated before save
        {
            Destroy(child.gameObject);
        }
        saving = false;
    }
}
