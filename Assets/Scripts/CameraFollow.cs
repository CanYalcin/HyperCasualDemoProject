using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    // give offset where camera follow player
    private Vector3 offset = new Vector3(0, 6f, -10f);

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        // every frame, change cameras position to follow player with given offset
        transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, Time.deltaTime * 50);
    }
}
