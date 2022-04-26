using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private int gameStatus = 0;     // 0 => game didnt started yet. Waiting for first touch
                                    // 1 => game started and running
                                    // 2 => player won the game
    public int GetGameStatus() { return gameStatus; }
    public void SetGameStatus(int _gameStatus) { gameStatus = _gameStatus; }

    private GameObject player;
    private GameObject[] enemies;
    private Vector3[] enemiesStartPoses;
    private GameObject[] rankings;
    [SerializeField] private TMP_Text rankingText;

    private Camera mainCam;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PaintController>().enabled = false;
        enemies = GameObject.FindGameObjectsWithTag("AI");
        rankings = new GameObject[enemies.Length + 1];
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].name = "AI_" + i;    // bad names given to the enemies
            rankings[i] = enemies[i];       // start rankings with enemies
        }
        rankings[enemies.Length] = player;  // dont forget to add player
        mainCam = Camera.main;
        enemiesStartPoses = new Vector3[enemies.Length];
        
        for (int i = 0; i < enemies.Length; i++)        // set enemies start poses. We need them for restarting the game
        {
            enemiesStartPoses[i] = enemies[i].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Array.Sort(rankings, CompareArray);     // sort objects with z axises.
        rankingText.text = "";
        for (int i = 0; i < rankings.Length-1; i++)
        {
            rankingText.text += ((i + 1) + " - " +rankings[i].name+"\n");
        }
        rankingText.text += (rankings.Length + " - " + rankings[rankings.Length - 1].name);
    }

    private int CompareArray(GameObject g1, GameObject g2)
    {
        return g1.transform.position.z - g2.transform.position.z > 0 ? -1 : 1;
    }

    public void GameOver()
    {
        RestartGame();
    }

    private void RestartGame()      // if player hits an obstacle, restart game completely. Its unfair to AI's!!!
    {
        gameStatus = 0;
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].transform.position = enemiesStartPoses[i];
            enemies[i].transform.rotation = Quaternion.identity;
            enemies[i].GetComponent<RunnerAI>().SetFlags();
        }
    }

    public void WonGame()
    {
        StartCoroutine(YouWon());
    }

    private Vector3 endGameCamPos = new Vector3(0, 2.5f, 70f);

    IEnumerator YouWon()       // when player wins the game, stop "PlayerController" and start painting!
    {
        gameStatus = 2;
        mainCam.gameObject.GetComponent<CameraFollow>().enabled = false;        // stop following player and show us his talent as an artist!
        while (Vector3.Distance(mainCam.transform.position, endGameCamPos) > 0.001f)
        {
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, endGameCamPos, Time.deltaTime * 10);
            mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, Quaternion.identity, Time.deltaTime * 10);

            yield return null;
        }
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<PaintController>().enabled = true;
    }
}
