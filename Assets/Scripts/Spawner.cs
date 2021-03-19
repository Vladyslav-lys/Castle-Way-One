using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public float spawnInterval;
    public bool isIncluded;
    public Transform spawnPoint;
    public Transform targetTransform;
    public GameObject unitPref;
    public GameManager gm;
    public Castle castle;
    private bool _isStarted;
    private int _needCount;
    
    public void StartSpawner()
    {
        if(castle.unitCount <= 1)
            return;
        
        _needCount = castle.unitCount / 2;
        castle.SetUnitScale();
        Invoke(nameof(SpawnUnits), 0.1f);
        Invoke(nameof(DiffUnits), 0.1f);
        _isStarted = true;
    }

    private void DiffUnits()
    {
        castle.unitCount -= _needCount;
        castle.SetUnitScale();
        castle.unitCountText.text = castle.unitCount.ToString();
        gm.InitCommonUnits();
        if(castle.unitCount < castle.maxUnitCount)
            castle.maxText.SetActive(false);
    }

    private void SpawnUnits()
    {
        if (castle && castle.unitCount > 1)
        {
            SpawnUnitCo(); 
        }
    }

    private void SpawnUnitCo()
    {
        StartCoroutine(SpawnUnitsByTime());
    }

    private IEnumerator SpawnUnitsByTime()
    {
        if (_needCount % 2 == 0)
        {
            for (int i = 0; i < _needCount / 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    SpawnUnit();
                }
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        if (_needCount % 2 == 1)
        {
            for (int i = 0; i < _needCount; i++)
            {
                SpawnUnit();
                yield return new WaitForSeconds(spawnInterval);
            }
        }
        
        if (castle.unitCount <= 1 || castle.unitCount <= _needCount)
        {
            _needCount = 0;
            CancelInvoke(nameof(SpawnUnits));
            //spawnPoint = null;
        }
    }

    public void SpawnUnit()
    {
        Transform currentTransform = spawnPoint;
        unitPref.GetComponent<AI>().gm = gm;
        GameObject unit = GameObject.Instantiate(unitPref, currentTransform.position, Quaternion.identity);
        unit.transform.LookAt(targetTransform);
        unit.GetComponent<AI>().targetTransform = targetTransform;
    }
}
