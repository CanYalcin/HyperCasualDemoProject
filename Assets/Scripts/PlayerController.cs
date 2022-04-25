using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // i didnt know what swerve mechanics are. I tried to make a controller that player can freely move right and left

    //private bool isDragging = false;

    private Vector3 startPos;
    private Vector2 startTouch, swipeDelta;

    private float width; // we need width to calculate drag distance

    private readonly float xMoveSpeed = 50f; // right and left movement speed
    private float maxClamp = 4.6f; // max distance to go right and left
    private readonly float forwardSpeed = 15f; // forward speed

    private Rigidbody rb;
    private Animator playerAnim;

    private GameManager gameManager;


    private void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        width = (float)Screen.width / 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        int gameStatus = gameManager.GetGameStatus(); // get game status
        if (gameStatus == 0) // if game didnt start yet
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];
                if (touch.phase == TouchPhase.Began) // touch began, so does the game
                {
                    gameManager.SetGameStatus(1);
                    playerAnim.SetBool("GameStarted", true); // and player starts to run
                    startTouch = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    Reset();
                }
                swipeDelta = Vector2.zero;
                float newPosX = transform.position.x;
                if (touch.phase == TouchPhase.Moved) // if touch moves
                {
                    swipeDelta = touch.position - startTouch; // how much touch moved?

                    float xDist = swipeDelta.x / width; // and what is the x movement of touch
                    float xSpeed = xDist * xMoveSpeed * Time.deltaTime;
                    newPosX = transform.position.x + xSpeed;
                    newPosX = Mathf.Clamp(newPosX, -maxClamp, maxClamp); // new position cant be outside of (-maxclamp,maxclamp)
                }
            }
        }
        //#endif
        if (gameStatus == 1)
        {
            float newPosX = transform.position.x;
            if (Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];
                if (touch.phase == TouchPhase.Began) // lets get the start touch pos
                {
                    startTouch = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    Reset();
                }
                swipeDelta = Vector2.zero;
                if (touch.phase == TouchPhase.Moved ) // if touch moves
                {
                    swipeDelta = touch.position - startTouch; // how much touch moved?
                    
                    float xDist = swipeDelta.x / width; // and what is the x movement of touch
                    float xSpeed = xDist * xMoveSpeed * Time.deltaTime;
                    newPosX = transform.position.x + xSpeed;
                    newPosX = Mathf.Clamp(newPosX, -maxClamp, maxClamp); // new position cant be outside of (-maxclamp,maxclamp)
                }
            }
            float zSpeed = forwardSpeed * Time.deltaTime;
            rb.MovePosition(new Vector3(newPosX, transform.position.y, transform.position.z + zSpeed));
        }
    }

    public void Reset() // reset touches
    {
        startTouch = swipeDelta = Vector2.zero;
        //isDragging = false;
    }

    private void GameOver() // player hit an obstacle
    {
        gameManager.GameOver(); // game over
        playerAnim.SetBool("GameStarted", false);   // stop running
        transform.position = startPos;              // reset pos
        transform.rotation = Quaternion.identity;   // reset rotation
        Reset();                                    // reset touches
    }

    IEnumerator YouWon()                            // player won the game
    {
        yield return new WaitForSeconds(0.3f);      // run a little more
        playerAnim.SetBool("GameStarted", false);   // then stop
        
        gameManager.WonGame();                      // then go to next step
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle")) // player hit an obstacle. Game Over.
        {
            GameOver();
        }
        // we dont need to check collision with ai here. Because when ai hit a player, it will apply force. We dont need to do it more than 1 time.
    }

    private void OnTriggerEnter(Collider other) // player won!
    {
        if (other.CompareTag("FinishLine"))
        {
            StartCoroutine(YouWon());
        }
    }
}