using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static System.Action ForceBack;
    public GameObject sliderObj;
    Slider slider;

    public GameObject WinUI;
    public GameObject LoseUI;
    bool GameOver;
    PlayerMove playerScript;

    bool Switching;
    
    void Start()
    {
        slider = sliderObj.GetComponent<Slider>();
        playerScript = FindObjectOfType<PlayerMove>();
        playerScript.PlayerHasWon += YouWin;
        playerScript.PlayerIsCaught += YouLose;
        SwitchingWorld.SwitchWorld += Switch;
    }

   
    void Update()
    {
        if (GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
            else if (Input.GetKeyDown(KeyCode.Escape) | Input.GetKeyDown(KeyCode.E))
            {
                Application.Quit();
            }
        }

        StaminaDrain(Time.deltaTime);
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
        Switching = !Switching;
    }

    void StaminaDrain(float amount)
    {
        if (Switching)
        {
            sliderObj.SetActive(true);
            float temp = slider.value;
            temp -= amount;
            slider.value = temp;
            if(slider.value == slider.minValue)
            {
                if(ForceBack != null)
                {
                    ForceBack();
                }
                Switch();
            }
        }
        else if (!Switching)
        {
            float temp = slider.value;
            temp += amount;
            slider.value = temp;
            if(slider.value == slider.maxValue)
            {
                sliderObj.SetActive(false);
            }
        }
    }
}
