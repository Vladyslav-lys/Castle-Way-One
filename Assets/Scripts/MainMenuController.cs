using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour, IPointerDownHandler
{
    public GameObject playUI;
    public GameManager gm;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        gm.isStarted = true;
        playUI.SetActive(true);
        
        for (int i = 0; i < gm.levels[PlayerPrefs.GetInt("Level") - 1].castles.Length; i++)
        {
            gm.levels[PlayerPrefs.GetInt("Level") - 1].castles[i].StartAddUnits().StartRemoveUnits();
        }
        
        gm.aim.enabled = true;
        gameObject.SetActive(false);
    }
}
