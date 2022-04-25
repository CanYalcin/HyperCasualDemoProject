using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalMoveObstacle : MonoBehaviour
{
    private float maxDistance = 5; // max distance that obstacle can move (-5,5)
    private Vector3[] movementLimit; // vectors that obstacle can move between

    private float speed; // we didnt set the speed here. We set it in Start function to randomize speeds
    private bool direction;

    private void Start()
    {
        movementLimit = new Vector3[2];
        movementLimit[0] = new Vector3(transform.position.x + maxDistance, transform.position.y, transform.position.z);
        movementLimit[1] = new Vector3(transform.position.x - maxDistance, transform.position.y, transform.position.z);
        speed = Random.Range(1f, 4f); // randomize speed
        direction = Random.Range(0, 2) == 0; // we are setting the start direction of obstacle. false => start move to right, true => start move to left
    }

    private void Update()
    {
        // if it goes out from movementLimit vectors limits, change direction 
        if (transform.position.x >= movementLimit[0].x || transform.position.x <= movementLimit[1].x)
            direction = !direction;

        if (!direction)
            transform.position = Vector3.MoveTowards(transform.position, movementLimit[0], speed * Time.deltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position, movementLimit[1], speed * Time.deltaTime);
    }
}