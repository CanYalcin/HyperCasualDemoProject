using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwerveController2 : MonoBehaviour
{
    private Animator playerAnim;

    private void Start()
    {
        playerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            playerAnim.SetBool("GameStarted", true);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("f");
            playerAnim.SetBool("GameStarted", false);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            playerAnim.SetBool("GameOver", true);
        }
    }
}
