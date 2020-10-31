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
    SwitchingWorld switchScript;

    public GameObject Transition;
    public GameObject end;
    bool Switching;
    
    void Start()
    {
        playerScript = FindObjectOfType<PlayerMove>();
        switchScript = FindObjectOfType<SwitchingWorld>();
        playerScript.PlayerHasWon += YouWin;
        playerScript.PlayerIsCaught += YouLose;
        switchScript.SwitchReal += Switch;
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

        if (Switching)
        {
            Transition.transform.position = Vector3.MoveTowards(Transition.transform.position, end.transform.position, 5 * Time.deltaTime);
            Switching = false;
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

    void Switch()
    {
        Switching = true;
    }
}
