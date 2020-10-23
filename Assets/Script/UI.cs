using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public GameObject WinUI;
    public GameObject LoseUI;
    bool GameOver;
    PlayerMove playerScript;
    
    void Start()
    {
        playerScript = FindObjectOfType<PlayerMove>();
        playerScript.PlayerHasWon += YouWin;
        playerScript.PlayerIsCaught += YouLose;
    }

   
    void Update()
    {
        if (GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void YouLose()
    {
        OnGameOver(LoseUI);
    }

    void YouWin()
    {
        OnGameOver(WinUI);
    }

    void OnGameOver(GameObject gameOverUI)
    {
        gameOverUI.SetActive(true);
        GameOver = true;
        playerScript.PlayerHasWon -= YouWin;
        playerScript.PlayerIsCaught -= YouLose;
    }
}
