using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector2 : MonoBehaviour
{
    private RunnerAI controller;

    private void Start()
    {
        controller = gameObject.GetComponent<RunnerAI>();
    }

}
