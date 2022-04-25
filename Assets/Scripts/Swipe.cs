using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Swipe : MonoBehaviour
{
    public int gameStatus = 0; // 0: not started, 1: started, 2: won

    private Vector3 startPos;

    private bool isDragging = false;

    private Vector2 startTouch, swipeDelta;

    private float width;

    private float xMoveSpeed = 50f;
    private float maxClamp = 4.6f;

    private float forwardSpeed = 15f;

    private Rigidbody rb;
    private Animator playerAnim;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private TMP_Text colorRateText;

    private void Start()
    {
        mainCam = Camera.main;
        //rendCam = GameObject.FindGameObjectWithTag("PaintCamera").GetComponent<Camera>();
        //rendCam.enabled = false;
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        width = (float)Screen.width / 2.0f;
    }

    // Update is called once per frame
    private void Update()
    {
        /*#if UNITY_EDITOR || UNITY_STANDALONE
                if (Input.GetMouseButtonDown(0))
                {
                    isDragging = true;
                    startTouch = Input.mousePosition;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    isDragging = false;
                    Reset();
                }
        #elif UNITY_ANDROID*/
        if (gameStatus == 0 || gameStatus == 1)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];
                if (touch.phase == TouchPhase.Began)
                {
                    gameStatus = 1;
                    playerAnim.SetBool("GameStarted", true);
                    isDragging = true;
                    startTouch = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isDragging = false;
                    Reset();
                }
            }
            //#endif
            if (gameStatus == 1)
            {
                swipeDelta = Vector2.zero;
                float newPosX = transform.position.x;
                if (isDragging)
                {
                    if (Input.touchCount > 0)
                    {
                        Touch touch = Input.touches[0];
                        swipeDelta = touch.position - startTouch;
                        //Debug.Log("swipeDelta - " + swipeDelta);
                    }
                    /*else if (Input.GetMouseButton(0))
                    {
                        swipeDelta = (Vector2)Input.mousePosition - startTouch;
                    }*/

                    float xDist = swipeDelta.x / width;
                    float xSpeed = xDist * xMoveSpeed * Time.deltaTime;
                    newPosX = transform.position.x + xSpeed;
                    newPosX = Mathf.Clamp(newPosX, -maxClamp, maxClamp);
                }
                float zSpeed = forwardSpeed * Time.deltaTime;
                rb.MovePosition(new Vector3(newPosX, transform.position.y, transform.position.z + zSpeed));
            }
        }
        else if (gameStatus == 2)
        {
            if (Input.touchCount > 0)
            {
                Paint();
            }
        }
    }

    bool saving = false;
    private Camera mainCam;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private GameObject brushContainer;
    [SerializeField] private Material renderMaterial;
    [SerializeField] private GameObject brushPrefab;
    private Color color = new Color(255, 0, 0, 255);
    private int brushCounter = 0;
    private int maxBrushCount = 50;

    void Paint()
    {
        if (saving)
            return;
        Vector3 hitPos = Vector3.zero;
        if (HitTestUVPosition(ref hitPos))
        {
            GameObject brushObj = Instantiate(brushPrefab); //Paint a brush
            brushObj.name = "brush";
            brushObj.GetComponent<SpriteRenderer>().color = color; //Set the brush color

            color.a = 85.0f; // Brushes have alpha to have a merging effect when painted over.
            brushObj.transform.parent = brushContainer.transform; //Add the brush to our container to be wiped later
            brushObj.transform.localPosition = hitPos; //The position of the brush
            brushObj.transform.localScale = Vector3.one*3;//The size of the brush
        }
        brushCounter++; //Add to the max brushes
        if (brushCounter >= maxBrushCount)
        { //If we reach the max brushes available, flatten the texture and clear the brushes
            saving = true;
            Invoke("SaveTexture", 0.1f);

        }
    }

    /*IEnumerator CalculatePercentPainted()
    {
        int ct = 0;
        while (true)
        {
            if (ct % 10 == 0)
            {

            }
            yield return null;
        }
    }*/

    bool HitTestUVPosition(ref Vector3 hitPos)
    {
        RaycastHit hit;
        Touch t = Input.touches[0];
        Vector3 cursorPos = new Vector3(t.position.x, t.position.y, 0.0f);
        Ray cursorRay = mainCam.ScreenPointToRay(cursorPos);
        if (Physics.Raycast(cursorRay, out hit, 25, layerMask))
        {
                hitPos = hit.point - Vector3.forward * 0.1f;
                return true;
        }
        else
            return false;
    }

    [SerializeField] private Material wallMat;
    void SaveTexture()
    {
        brushCounter = 0;
        RenderTexture.active = renderTexture;
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        int ct = 0, ct2 = 0;
        var colors = tex.GetPixels32();
        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i].r > 150 && colors[i].g < 50 && colors[i].b < 50)
                ct++;
            else
                ct2++;
        }
        Debug.Log(ct);
        Debug.Log(ct2);
        Debug.Log(colors.Length);
        colorRateText.text = "%" + (((float)ct / (float)colors.Length)*100).ToString();

        tex.Apply();

        wallMat.mainTexture = tex;
        RenderTexture.active = null;

        foreach (Transform child in brushContainer.transform)
        {//Clear brushes
            Destroy(child.gameObject);
        }
        saving = false;
    }

    private void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;
        isDragging = false;
    }
}