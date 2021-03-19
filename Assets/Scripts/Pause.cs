using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public TextMeshProUGUI vibrationText;
    public TextMeshProUGUI soundText;

    private void Start()
    {
        if (PlayerPrefs.GetInt("Audio") != 0)
        {
            soundText.text = "Sound On";
        }
        else
        {
            soundText.text = "Sound Off";
        }

        if (PlayerPrefs.GetInt("IsVibration") != 0)
        {
            vibrationText.text = "Vibration On";
        }
        else
        {
            vibrationText.text = "Vibration Off";
        }
    }
}
