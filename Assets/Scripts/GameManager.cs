using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool levelMaker;
    public int level;
    public int enemyUnitCount;
    public int playerUnitCount;
    public bool isStarted;
    public List<Spawner> spawners;
    public List<Level> levels;
    public List<Material> levelSkyboxes;
    public GameObject playerPref;
    public GameObject enemyPref;
    public GameObject finishPlayerPref;
    public Transform winCameraTransform;
    public Transform lastCastleTransform;
    public TextMeshProUGUI enemyCountText;
    public TextMeshProUGUI playerCountText;
    public UIManager uim;
    public AIManager aim;
    public DragArrowPanel arrowController;
    private Camera _camera;
    private Vector3 _cameraOffset;
    
    private void Awake()
    {
        InitPlayerPrefs();
        
        if (levelMaker)
            PlayerPrefs.SetInt("Level", level);
        
        levels[PlayerPrefs.GetInt("Level") - 1].gameObject.SetActive(true);
        RenderSettings.skybox = levelSkyboxes[PlayerPrefs.GetInt("Level") - 1];
        DynamicGI.UpdateEnvironment();
    }
    
    private void Start()
    {
        InitCommonUnits();
        Time.timeScale = 1f;
        _camera = Camera.main;
        _camera.transform.localPosition = new Vector3(10.4f, 9.5f, 2f);
        _cameraOffset = new Vector3(-0.6f, 0f, 0f);
    }

    private void Update()
    {
        if (aim.enemyCastles.Count == 0 && winCameraTransform != null && lastCastleTransform != null)
        {
            _camera.transform.localPosition = Vector3.MoveTowards(_camera.transform.localPosition, 
                winCameraTransform.localPosition,
                Time.deltaTime * 30f);
            _camera.transform.rotation = Quaternion.LookRotation((lastCastleTransform.position + _cameraOffset) - _camera.transform.position);
        }
    }

    public void InitCommonUnits()
    {
        enemyUnitCount = 0;
        playerUnitCount = 0;
        
        foreach (var castle in levels[PlayerPrefs.GetInt("Level") - 1].castles)
        {
            if (castle.gameObject.layer == 11)
            {
                enemyUnitCount += castle.unitCount;
                if(!aim.enabled && !isStarted)
                    aim.enemyCastles.Add(castle);
            }
            
            if(castle.gameObject.layer == 10)
            {
                playerUnitCount += castle.unitCount;
                if(!aim.enabled && !isStarted)
                    aim.castles.Add(castle);
            }

            if (castle.gameObject.layer == 13 && !aim.enabled && !isStarted)
            {
                aim.castles.Add(castle);
            }
        }

        enemyCountText.text = enemyUnitCount.ToString();
        playerCountText.text = playerUnitCount.ToString();
    }

    public void SpawnFinishPlayers(Transform currentCastleTransform, ref Vector3 offset)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject finishPlayer = GameObject.Instantiate(finishPlayerPref, currentCastleTransform.position + offset, Quaternion.identity);
            offset.x += 0.5f;
        }
    }

    public void SetSpawner()
    {
        int count = spawners.Count;
        for (int i = 0; i < count; i++)
        {
            spawners[i].StartSpawner();
        }
    }

    public void StartNextLevel()
    { 
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
        if(PlayerPrefs.GetInt("Level") > 5)
            PlayerPrefs.SetInt("Level", 1);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void InitPlayerPrefs()
    {
        if(!PlayerPrefs.HasKey("Level"))
            PlayerPrefs.SetInt("Level", 1);
        
        if(!PlayerPrefs.HasKey("Audio"))
            PlayerPrefs.SetInt("Audio", 1);

        if (!PlayerPrefs.HasKey("IsVibration"))
            PlayerPrefs.SetInt("IsVibration", 1);
    }
}
