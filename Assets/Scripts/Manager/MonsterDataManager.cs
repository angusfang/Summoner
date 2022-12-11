using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDataManager : Singleton<MonsterDataManager>
{
    public MonsterStats_SO NightmareDragonSO;
    public GameObject NightmareDragonPrefab;

    [HideInInspector]
    public enum MonsterID { NightmareDragon, };

    protected override void Awake()
    {
        base.Awake();
    }

    public MonsterStats_SO MonsterIDMapToState(MonsterID monsterID)
    {
        switch (monsterID)
        {
            case MonsterID.NightmareDragon:
                return NightmareDragonSO;
        }
        return null;
    }

    public GameObject MonsterIDMapToPrefab(MonsterID monsterID)
    {
        switch (monsterID)
        {
            case MonsterID.NightmareDragon:
                return NightmareDragonPrefab;
        }
        return null;
    }
}
