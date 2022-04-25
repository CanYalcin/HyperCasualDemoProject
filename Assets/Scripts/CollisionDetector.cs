using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    private PlayerController controller;

    private void Start()
    {
        controller = gameObject.GetComponent<PlayerController>();
    }

    
}
