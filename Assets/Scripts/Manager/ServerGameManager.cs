using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static MonsterDataManager;
//using static MonsterStateMachine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ServerGameManager : NetworkBehaviour
{
    int maxPlayer = 6;
    private static ServerGameManager instance;
    public static ServerGameManager Instance => instance;
    
   
    struct PlayerSelectState
    {
        
        public GameObject gameObject1;
        public GameObject gameObject2;
    };

    private PlayerSelectState[] playerSelectStates;
    
    public override void OnNetworkSpawn()
    {
        
        if (instance != null) Destroy(gameObject);
        else instance = this;
        playerSelectStates = new PlayerSelectState[maxPlayer];
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        //GameObject monster_select1, monster_select2;
        for(int i=0;i < maxPlayer; i++)
        {
            UnityEngine.Random.Range(i, maxPlayer);
            if (playerSelectStates[i].gameObject2 != null)
            {
                //TODO: GetComponent may not a good idea
                MonsterStateMachine attacker = playerSelectStates[i].gameObject1.GetComponent<MonsterStateMachine>();
                attacker.AttackSignal = true;

                GameObject targetGameObject = playerSelectStates[i].gameObject2;
                NavMeshAgent targetAgent = targetGameObject.GetComponent<NavMeshAgent>();
                Monster targetMonster = targetGameObject.GetComponent<Monster>();
                MonsterStateMachine targetStateMachine = targetGameObject.GetComponent<MonsterStateMachine>();
                MonsterStateMachine.Str_targetMonster targetInf = new MonsterStateMachine.Str_targetMonster()
                { gameObject = targetGameObject, agent = targetAgent, monster = targetMonster, stateMachine = targetStateMachine};
                attacker.TargetMonster = targetInf;


                playerSelectStates[i].gameObject1 = playerSelectStates[i].gameObject2 = null;
            }
        }
       
    }
    
    
    public void SetSelectState(ulong UserID, ulong monsterID1, ulong monsterID2)
    {
        playerSelectStates[UserID].gameObject1 = ObjManager.Instance.MonsterNetIDToObj[monsterID1];
        playerSelectStates[UserID].gameObject2 = ObjManager.Instance.MonsterNetIDToObj[monsterID2];
    }
}
