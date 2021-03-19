using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIManager : MonoBehaviour
{
    public float startTime;
    public float repeatEnemyCastleTime;
    public float repeatUpgradeTime;
    public float repeatCastleTime;
    public float repeatEnemyCastleCountTime;
    public float repeatEnemySpawnTime;
    public GameManager gm;
    public List<Castle> enemyCastles;
    public List<Castle> castles;
    private int _enemyCastleIndex;
    private int _castleIndex;
    private int _enemyCastleCount;
    
    private void Start()
    {
        Invoke(nameof(ThrowUpgradeByTime), repeatUpgradeTime);
        Invoke(nameof(ThrowEnemyCastleCountByTime), repeatEnemyCastleCountTime);
        Invoke(nameof(ThrowEnemyCastleByTime), repeatEnemyCastleTime);
        Invoke(nameof(ThrowCastleByTime), repeatCastleTime);
        Invoke(nameof(ThrowEnemySpawnByTime), repeatEnemySpawnTime);
    }
    
    private void ThrowUpgradeByTime()
    {
        int randomUpgrade = GetRandomUpgrade();
        int enemyCastleIndex = _enemyCastleIndex;
        if (randomUpgrade == 1 && isEnemyCastlesExist())
        {
            enemyCastles[enemyCastleIndex].CastleLevelUp();
            repeatUpgradeTime = GetRandomRepeatTime(10);
        }
        Invoke(nameof(ThrowUpgradeByTime), repeatUpgradeTime);
    }

    private void ThrowEnemyCastleCountByTime()
    {
        if (isEnemyCastlesExist())
        {
            _enemyCastleCount = GetRandomEnemyCastleCount();
            repeatEnemyCastleCountTime = GetRandomRepeatTime(20);
            Invoke(nameof(ThrowEnemyCastleCountByTime), repeatEnemyCastleCountTime);
        }
    }
    
    private void ThrowEnemyCastleByTime()
    {
        if (isEnemyCastlesExist())
        {
            _enemyCastleIndex = GetRandomEnemyCastle();
            repeatEnemyCastleTime = GetRandomRepeatTime(10);
            Invoke(nameof(ThrowEnemyCastleByTime), repeatEnemyCastleTime);
        }
    }
    
    private void ThrowCastleByTime()
    {
        if (isCastlesExist())
        {
            _castleIndex = GetRandomCastle();
            repeatCastleTime = GetRandomRepeatTime(10);
            Invoke(nameof(ThrowCastleByTime), repeatCastleTime);
        }
    }
    
    private void ThrowEnemySpawnByTime()
    {
        int castleIndex = _castleIndex;
        if(castleIndex < castles.Count)
            castleIndex = GetRandomCastle();
        if (_enemyCastleCount > 0 && castles.Count > 0 && enemyCastles.Count > 0)
        {
            int enemyCastleIndex = 0;
            for (int i = 0; i < _enemyCastleCount; i++)
            {
                enemyCastleIndex = GetRandomEnemyCastle();
                enemyCastles[enemyCastleIndex].spawner.targetTransform = castles[castleIndex].castleTransform;
                enemyCastles[enemyCastleIndex].spawner.StartSpawner();
            }
            repeatEnemySpawnTime = GetRandomRepeatTime(20);
            Invoke(nameof(ThrowEnemySpawnByTime), repeatEnemySpawnTime);
        }
    }

    private bool isEnemyCastlesExist()
    {
        if (enemyCastles.Count > 0)
            return true;
        return false;
    }
    
    private bool isCastlesExist()
    {
        if (castles.Count > 0)
            return true;
        return false;
    }

    private int GetRandomUpgrade()
    {
        return Random.Range(0, 2);
    }
    
    private int GetRandomCastle()
    {
        return Random.Range(0, castles.Count);
    }
    
    private int GetRandomEnemyCastle()
    {
        return Random.Range(0, enemyCastles.Count);
    }

    private int GetRandomEnemyCastleCount()
    {
        return Random.Range(1, enemyCastles.Count + 1);
    }

    private int GetRandomRepeatTime(int maxTime)
    {
        return Random.Range(4, maxTime);
    }

    public void RemoveEnemyCastle(Castle enemyCastle)
    {
        foreach (var enemyCastleOfCastles in enemyCastles)
        {
            if (enemyCastleOfCastles.gameObject == enemyCastle.gameObject)
            {
                enemyCastles.Remove(enemyCastleOfCastles);
                break;
            }
        }
    }

    public void RemoveCastle(Castle castle)
    {
        foreach (var castleOfCastles in castles)
        {
            if (castleOfCastles.gameObject == castle.gameObject)
            {
                castles.Remove(castleOfCastles);
                break;
            }
        }
    }

    public void AddEnemyCastle(Castle enemyCastle)
    {
        foreach (var enemyCastleOfCastles in enemyCastles)
        {
            if (enemyCastleOfCastles == enemyCastle)
            {
                return;
            }
        }
        enemyCastles.Add(enemyCastle);
    }

    public void AddCastle(Castle castle)
    {
        foreach (var castleOfCastles in castles)
        {
            if (castleOfCastles == castle)
            {
                return;
            } 
        }
        castles.Add(castle);
    }
}
