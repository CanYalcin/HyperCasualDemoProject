using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfDonut : MonoBehaviour
{
    private float maxX = -0.12f;
    private float minX = 0.15f;

    private float moveSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveDonut());     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator MoveDonut()
    {
        float time = Random.Range(1f, 3f);
        var timer = new WaitForSeconds(time);
        Debug.Log(time);
        while (true)
        {
            yield return timer;
            while (transform.localPosition.x-maxX > 0.001f)
            {
                float speed = moveSpeed * Time.deltaTime;
                transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition-Vector3.right* speed, Time.deltaTime);
                yield return null;
            }
            yield return timer;
            while (transform.localPosition.x - minX < -0.001f)
            {
                float speed = moveSpeed * Time.deltaTime;
                transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + Vector3.right * speed, Time.deltaTime);
                yield return null;
            }
        }
    }
}
