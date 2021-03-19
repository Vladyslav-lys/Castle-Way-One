using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CastleType
{
    PlayerCastle = 0,
    EnemyCastle = 1,
    NeutralCastle = 2
}

public class Castle : MonoBehaviour
{
    #region StartParametres

    public CastleType castleType;
    public int startCastleLevel;
    public int startUnitCount;
    
    #endregion
    #region UIParams

    public Color playerUIColor;
    public Color enemyUIColor;
    public Color neutralUIColor;
    public List<TextMeshProUGUI> textsUIList;
    public List<Image> imagesUIList;
    public List<Canvas> castleCanvases;
    #endregion
    
    #region CastleLevelTypes
    public List<GameObject> castleTypes;
    public List<GameObject> playerLevelCastles;
    public List<GameObject> enemyLevelCastles;
    public List<GameObject> neutralLevelCastles;
    #endregion

    public bool isBuilding;
    public bool canCount;
    [HideInInspector]
    public int maxUnitCount;
    public int unitCount;
    public int castleLevel;
    public int ownerUnitLayer;
    public int maxCastleLevel;
    [HideInInspector]
    public List<int> collisionLayers;
    public TextMeshProUGUI unitCountText;
    public TextMeshProUGUI levelText;
    public Image unitScale;
    public Image levelBack;
    public GameManager gm;
    public GameObject maxText;
    public GameObject upgradeArrow;
    public GameObject building;
    public GameObject removeUnitTextPref;
    public Transform castleTransform;
    public Transform winCameraTransform;
    public Spawner spawner;
    public ArrowShootCastle arrowShootCastle;
    public float repeatingTime;
    public float startRepeatingTime;
    public GameObject lineRendererPrefab;
    public LineRenderer lineRenderer;
    public Vector3 offset;
    
    protected void Awake()
    {
        castleLevel = startCastleLevel;
        unitCount = startUnitCount;
        if (isEqualsCastleType(CastleType.EnemyCastle))
        {
            ownerUnitLayer = 9;
            spawner.unitPref = gm.enemyPref;
            InitEnemyCastle(false);
        }
        if (isEqualsCastleType(CastleType.PlayerCastle))
        {
            ownerUnitLayer = 8;
            spawner.unitPref = gm.playerPref;
            InitPlayerCastle(false);
        }
        if (isEqualsCastleType(CastleType.NeutralCastle))
        {
            InitNeutralCastle();
        }
    }

    protected void Start()
    {
        unitCountText.text = unitCount.ToString();
        levelText.text = "Lv. " + castleLevel;
        for (int i = 0; i < castleLevel - 1; i++)
        {
            repeatingTime -= 0.1f;
        }
        SetMaxLevelByLevel();
        SetUnitScale();
    }
    
    private void Update()
    {
        if(unitCount >= maxUnitCount/2 && castleLevel < maxCastleLevel && gameObject.layer != 13 && gameObject.layer != 11 && !isBuilding)
            upgradeArrow.SetActive(true);
        else
            upgradeArrow.SetActive(false);
        
        if (unitCount >= maxUnitCount)
            maxText.SetActive(true);
        else
            maxText.SetActive(false);
        
        if (unitCount < 0)
        {
            unitCount = 0;
            unitCountText.text = unitCount.ToString();
            gm.InitCommonUnits();
        }
    }
    
    protected void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < collisionLayers.Count; i++)
        {
            if (other.gameObject.layer == collisionLayers[i])
            {
                unitCount--;
                SetUnitScale();
                unitCountText.text = unitCount.ToString();
                ChangeCastle(other.gameObject);
                Destroy(other.gameObject);
                gm.InitCommonUnits();
            }
        }
        
        if (other.gameObject.layer == ownerUnitLayer)
        {
            if (other.gameObject.GetComponent<AI>().targetTransform == castleTransform)
            {
                Destroy(other.gameObject);
                unitCount++;
                SetUnitScale();
                unitCountText.text = unitCount.ToString();
                gm.InitCommonUnits();
                return;
            }
        
            spawner.targetTransform = other.gameObject.GetComponent<AI>().targetTransform;
            spawner.SpawnUnit();
            Destroy(other.gameObject);
        }
    }

    protected void InitEnemyCastle(bool isChange)
    {
        //Смена цвета UI
        foreach (Image image in imagesUIList)
        {
            image.color = enemyUIColor;
        }
        //Убираем все замки
        DropAllCastles();
        castleTypes[(int)CastleType.EnemyCastle].SetActive(true);
        
        if (isChange)
        {
            //Меняем тип на врага
            castleType = CastleType.EnemyCastle;
            //Ставим первый уровень замка
            castleLevel = 1;
            levelText.text = castleLevel.ToString();
            enemyLevelCastles[0].SetActive(true);
            if(isBuilding)
                building.SetActive(false);
        }
        else
        {
            collisionLayers.Add(8);
            arrowShootCastle?.collisionLayers.Add(8);
            //Ставим стартовый уровень замка
            enemyLevelCastles[startCastleLevel - 1].SetActive(true);
        }
    }
    
    protected void InitPlayerCastle(bool isChange)
    {
        //Создание LineRenderer на замке
        if(lineRendererPrefab)
            lineRenderer = Instantiate(lineRendererPrefab, transform).GetComponent<LineRenderer>();
        
        //Смена цвета UI
        foreach (Image image in imagesUIList)
        {
            image.color = playerUIColor;
        }
        //Убираем все замки
        DropAllCastles();
        castleTypes[(int)CastleType.PlayerCastle].SetActive(true);
        if (isChange)
        {
            //Меняем тип на врага
            castleType = CastleType.PlayerCastle;
            //Ставим первый уровень замка
            castleLevel = 1;
            levelText.text = castleLevel.ToString();
            playerLevelCastles[0].SetActive(true);
            if(isBuilding)
                building.SetActive(false);
        }
        else
        {
            collisionLayers.Add(9);
            arrowShootCastle?.collisionLayers.Add(9);
            //Ставим стартовый уровень замка
            playerLevelCastles[startCastleLevel - 1].SetActive(true);
        }
    }
    
    protected void InitNeutralCastle()
    {
        //Смена цвета UI
        foreach (Image image in imagesUIList)
        {
            image.color = neutralUIColor;
        }
        //Убираем все замки
        DropAllCastles();
        castleTypes[(int)CastleType.NeutralCastle].SetActive(true);
        collisionLayers.Add(8);
        collisionLayers.Add(9);
        arrowShootCastle?.collisionLayers.Add(8);
        arrowShootCastle?.collisionLayers.Add(9);
        //Ставим стартовый уровень замка
        neutralLevelCastles[startCastleLevel - 1].SetActive(true);
    }

    protected void DropAllCastles()
    {
        foreach (var castle in castleTypes)
        {
            castle.SetActive(false);
        }
        foreach (var castle in enemyLevelCastles)
        {
            castle.SetActive(false);
        }
        foreach (var castle in playerLevelCastles)
        {
            castle.SetActive(false);
        }
        foreach (var castle in neutralLevelCastles)
        {
            castle.SetActive(false);
        }
    }

    protected void DropAllUI()
    {
        foreach (var castle in gm.levels[PlayerPrefs.GetInt("Level") - 1].castles)
        {
            foreach (var castleCanvas in castle.castleCanvases)
            {
                castleCanvas.gameObject.SetActive(false);
            }
        }
    }

    protected void ChangeCastle(GameObject enemyObj)
    {
        if (unitCount > 0)
            return;

        if (isEqualsCastleType(CastleType.NeutralCastle))
        {
            collisionLayers.RemoveAt(0);
            arrowShootCastle?.collisionLayers.RemoveAt(0);
        }

        if (enemyObj.gameObject.layer == 8)
        {
            InitPlayerCastle(true);
            gm.aim.RemoveEnemyCastle(this);
            gm.aim.AddCastle(this);
            spawner.unitPref = gm.playerPref;
            if (gm.aim.enemyCastles.Count == 0 && gm.isStarted)
            {
                gm.isStarted = false;
                DropAllUI();
                //TODO change this to paint
                // Enemy[] enemies = FindObjectsOfType<Enemy>();
                // Player[] players = FindObjectsOfType<Player>();
                // foreach (Enemy enemy in enemies)
                // {
                //     Instantiate(enemy.blood, enemy.transform.position, Quaternion.identity);
                //     Destroy(enemy.gameObject);
                // }
                // foreach (Player player in players)
                // {
                //     Instantiate(player.blood, player.transform.position, Quaternion.identity);
                //     Destroy(player.gameObject);
                // }
                gm.winCameraTransform = winCameraTransform;
                gm.lastCastleTransform = castleTransform;
                Camera.main.orthographic = false;
                Camera.main.transform.SetParent(transform);
                gm.uim.SetEnd();
                Invoke(nameof(SpawnWinUnits), 2f);
                Invoke(nameof(SetWin), 6f);
            }
        }

        if (enemyObj.gameObject.layer == 9)
        {
            InitEnemyCastle(true);
            if(lineRenderer)
                Destroy(lineRenderer);
            gm.aim.RemoveCastle(this);
            gm.aim.AddEnemyCastle(this);
            spawner.unitPref = gm.enemyPref;
            
            if (gm.aim.castles.Count == 0 && gm.isStarted)
            {
                gm.isStarted = false;
                DropAllUI();
                gm.uim.SetLose();
                Time.timeScale = 0;
            }
        }

        collisionLayers[0] = enemyObj.GetComponent<AI>().enemyLayer;
        if(arrowShootCastle)
            arrowShootCastle.collisionLayers[0] = enemyObj.GetComponent<AI>().enemyLayer;
        ownerUnitLayer = enemyObj.layer;
        gameObject.layer = enemyObj.GetComponent<AI>().ownerCastleLayer;

        SetMaxLevelByLevel();
    }

    protected void SetWin()
    {
        gm.uim.SetWin();
    }

    protected void SpawnWinUnits()
    {
        gm.SpawnFinishPlayers(castleTransform, ref offset);
    }

    public Castle StartAddUnits()
    {
        InvokeRepeating(nameof(AddUnits), startRepeatingTime, repeatingTime);
        return this;
    }
    
    public Castle StartRemoveUnits()
    {
        InvokeRepeating(nameof(RemoveUnits), startRepeatingTime, repeatingTime);
        return this;
    }

    protected void AddUnits()
    {
        if(unitCount >= maxUnitCount || !canCount)
            return;

        unitCount++;
        SetUnitScale();
        unitCountText.text = unitCount.ToString();
        gm.InitCommonUnits();
    }
    
    protected void RemoveUnits()
    {
        if(unitCount <= maxUnitCount || !canCount)
            return;

        unitCount--;
        SetUnitScale();
        unitCountText.text = unitCount.ToString();
        gm.InitCommonUnits();
        
        GameObject removeUnitTextObj = Instantiate(removeUnitTextPref);
        removeUnitTextObj.transform.SetParent(castleCanvases[0].transform);
        removeUnitTextObj.transform.localPosition = removeUnitTextPref.transform.localPosition;
        removeUnitTextObj.transform.localScale = removeUnitTextPref.transform.localScale;
        removeUnitTextObj.transform.localRotation = removeUnitTextPref.transform.localRotation;
        Destroy(removeUnitTextObj, 1.5f);
    }

    public void CastleLevelUp()
    {
        if (castleLevel < maxCastleLevel && unitCount >= maxUnitCount/2)
        {
            //TODO refacDO refator this bulshit
            SetBuildling();
            unitCount -= maxUnitCount/2;
            SetUnitScale();
            unitCountText.text = unitCount.ToString();
            gm.InitCommonUnits();
            Invoke(nameof(SetNewLevelCastle), 4f);
        }
    }
    
    protected void SetBuildling()
    {
        isBuilding = true;
        if(isEqualsCastleType(CastleType.EnemyCastle))
            enemyLevelCastles[castleLevel - 1].SetActive(false);
        else if(isEqualsCastleType(CastleType.PlayerCastle))
            playerLevelCastles[castleLevel - 1].SetActive(false);
        building.SetActive(true);
    }

    protected void SetNewLevelCastle()
    {
        isBuilding = false;
        building.SetActive(false);
        
        if(!gm.isStarted)
            return;
        
        castleLevel++;
        levelText.text = "Lv. " + castleLevel;
        SetMaxLevelByLevel();
        CancelInvoke(nameof(AddUnits));
        repeatingTime -= 0.1f;
        InvokeRepeating(nameof(AddUnits), 0f, repeatingTime);
        
        if(isEqualsCastleType(CastleType.EnemyCastle))
            enemyLevelCastles[castleLevel - 1].SetActive(true);
        else if(isEqualsCastleType(CastleType.PlayerCastle))
            playerLevelCastles[castleLevel - 1].SetActive(true);
    }

    protected void SetMaxLevelByLevel()
    {
        maxUnitCount = castleLevel * 10;
    }

    public void SetUnitScale()
    {
        unitScale.fillAmount = unitCount * 1.0f / maxUnitCount;
    }

    public bool isEqualsCastleType(CastleType castleType)
    {
        return this.castleType == castleType;
    }
}
