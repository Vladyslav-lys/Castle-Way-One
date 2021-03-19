using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject winUI;
    public GameObject levelText;
    public GameObject loseUI;
    public GameObject pauseUI;
    public GameObject playUI;
    public GameManager gm;

    private void Start()
    {
        levelText.GetComponent<TextMeshProUGUI>().text = "Level " + PlayerPrefs.GetInt("Level");
    }

    public void SetWin()
    {
        winUI.SetActive(true);
    }
    
    public void SetLose()
    {
        loseUI.SetActive(true);
        SetEnd();
    }
    
    public void SetEnd()
    {
        levelText.SetActive(false);
        playUI.SetActive(false);
    }

    public void OpenPause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0;
    }
    
    public void ClosePause()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1;
    }
    
    public void OnOffVibration(TextMeshProUGUI text)
    {
        if (PlayerPrefs.GetInt("IsVibration") != 0)
        {
            PlayerPrefs.SetInt("IsVibration", 0);
            text.text = "Vibration Off";
        }
        else
        {
            PlayerPrefs.SetInt("IsVibration", 1);
            text.text = "Vibration On";
        }
    }
    
    public void OnOffSound(TextMeshProUGUI text)
    {
        if(PlayerPrefs.GetInt("Audio") != 0)
        {
            PlayerPrefs.SetInt("Audio", 0);
            text.text = "Sound Off";
        }
        else
        {
            PlayerPrefs.SetInt("Audio", 1);
            text.text = "Sound On";
        }
    }
}
