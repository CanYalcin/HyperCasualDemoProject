using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerAI : MonoBehaviour
{
    private Animator opponentAnim;
    private Rigidbody rb;
    private float forwardSpeed = 15f;
    private Vector3 startPos;
    private bool didIWin = false;

    private GameManager gameManager;

    private LayerMask layerMask = 1 << 6;

    private bool readyFlag = false;
    private bool runFlag = false;

    public void SetFlags() { readyFlag = false; runFlag = false; }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody>();
        opponentAnim = GetComponent<Animator>();
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        int gameStatus = gameManager.GetGameStatus();
        if (!readyFlag && gameStatus == 0)
        {
            opponentAnim.SetBool("GameStarted", false); // activate idle animation
            readyFlag = true;
        }
        if(gameStatus == 1) // status 1 means game started
        {
            if (!didIWin) // if ai didnt crossed the finish line yet, he will move
            {
                if (!runFlag) // if we didnt start running animation, we are doing it here
                {
                    opponentAnim.SetBool("GameStarted", true); // activate running animation
                    runFlag = true;
                }
                float zSpeed = forwardSpeed * Time.deltaTime; // movement to forward

                float xSpeed = 0; // movement to right and left sides with default zero
                if (Physics.SphereCast(transform.position, 3f, Vector3.forward, out RaycastHit hit, 3f, layerMask)) // check if sphereCast hits any objects forward
                {
                    if (hit.collider.CompareTag("Obstacle")) // if sphereCast hits obstacle, move ai to sides
                    {
                        xSpeed = 10 * Time.deltaTime; // change movement speed
                        if (transform.position.x <= hit.transform.position.x) // if hitted object is on right side, ai need to go left side
                            xSpeed *= -1;
                        if (transform.position.x + xSpeed > 4 || transform.position.x + xSpeed < -4) // if ai goes out from platform when he moves, he cant go that side
                            xSpeed *= -1;
                    }
                }
#if UNITY_STANDALONE_WIN
zSpeed *= 5;
xSpeed *= 5;
#endif
                rb.MovePosition(new Vector3(transform.position.x + xSpeed, transform.position.y, transform.position.z + zSpeed));
            }
        }
        else if (gameStatus == 2)
        {
            opponentAnim.SetBool("GameStarted", false); // status 2 means player won the game
        }
    }

    private void GameOver() // if ai hit an obstacle, he will start from beginning
    {
        transform.position = startPos;
        transform.rotation = Quaternion.identity;
        runFlag = false;
        readyFlag = false;
    }

    IEnumerator OpponentWon() // ai finished the game
    {
        yield return new WaitForSeconds(0.3f);
        didIWin = true;
        opponentAnim.SetBool("GameStarted", false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle")) // if ai hits obstacle, its game over for him
        {
            GameOver();
        }
        else if (collision.gameObject.CompareTag("AI") || collision.gameObject.CompareTag("Player")) // if ai hits another ai or player, apply force to both of them
        {
            var force = 3f;
            Vector3 dir = collision.contacts[0].point - transform.position;
            // We then get the opposite (-Vector3) and normalize it
            dir = -dir.normalized;
            // And finally we add force in the direction of dir and multiply it by force. 
            // This will push back the player
            GetComponent<Rigidbody>().AddForce(dir * force);
            collision.gameObject.GetComponent<Rigidbody>().AddForce(-dir * force);
        }
    }

    private void OnTriggerEnter(Collider other) // player lost to ai :/
    {
        if (other.tag == "FinishLine")
        {
            StartCoroutine(OpponentWon());
        }
    }
}
