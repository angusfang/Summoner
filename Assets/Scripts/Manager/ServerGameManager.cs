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
                StartCoroutine(PerformSkill(playerSelectStates[i].gameObject1, playerSelectStates[i].gameObject2));
                playerSelectStates[i].gameObject1 = playerSelectStates[i].gameObject2 = null;
            }
        }
       
    }
    
    IEnumerator PerformSkill(GameObject monster_select1, GameObject monster_select2)
    {
        
        Monster monster1 = monster_select1.GetComponent<Monster>();
        Monster monster2 = monster_select2.GetComponent<Monster>();
        NavMeshAgent monster1_agent = monster_select1.GetComponent<NavMeshAgent>();
        NavMeshAgent monster2_agent = monster_select2.GetComponent<NavMeshAgent>();
        //TODO: mana check, CD check
        Vector3 origin_position = monster1.original_position;
        Vector3 target_position = monster2.transform.position;
        Vector3 direction_o2t = (target_position - origin_position).normalized;
        Vector3 attack_range = (monster1_agent.radius + monster2_agent.radius) * direction_o2t;
        Animator anim = monster_select1.GetComponent<Animator>();
        Quaternion origin_rotation = Quaternion.LookRotation(Vector3.zero-origin_position);
        float range_between_pos_tar = monster1_agent.stoppingDistance;

        if (monster1.monster_stats.current_health <= 0) yield break;
        // Walk front
        if (monster1.monster_stats.need_walk)
        {
            monster1_agent.destination = target_position - attack_range;
            while (Vector3.Distance(monster1_agent.destination, monster1_agent.transform.position) > range_between_pos_tar) {
                anim.SetFloat("speed", monster1_agent.velocity.magnitude / monster1_agent.speed);
                yield return null;
                if (monster1.monster_stats.current_health <= 0) yield break;
                if (monster2 == null)
                {
                    monster1_agent.destination = origin_position;
                    yield break;
                }
                target_position = monster2.transform.position;
                monster1_agent.destination = target_position - attack_range;
            }
            anim.SetFloat("speed", 0f);
        }

        // Perform skill
        anim.SetTrigger("perform_skill");
        monster1_agent.isStopped = true;

        // Start to hit target
        Debug.Log("play skill animation");
        yield return new WaitForSeconds(monster1.monster_stats.perform_skill_time_point);
        if (monster1.monster_stats.current_health <= 0) yield break;
        //Hit target
        //Animation + Stop agent
        StartCoroutine(GetHit(monster_select2));
        ////Calculated value
        CalculateStateAfterSkill(monster1, monster2);
        ////Judge Death 
        StartCoroutine(JudgeDie(monster2));
        

        //Debug.Log(monster2.monster_stats.current_health);
        monster1_agent.isStopped = false;
        yield return new WaitForSeconds(Mathf.Max(0, monster1.monster_stats.animation_duration - monster1.monster_stats.perform_skill_time_point));
        if (monster1.monster_stats.current_health <= 0) yield break;
        // Walk back

        if (monster1.monster_stats.need_walk )
        {
            monster1_agent.destination = origin_position;
            while (Vector3.Distance(monster1_agent.destination, monster1_agent.transform.position) > range_between_pos_tar) {
                anim.SetFloat("speed", monster1_agent.velocity.magnitude / monster1_agent.speed);
                yield return null;
                if (monster1.monster_stats.current_health <= 0) yield break;
            }

            StartCoroutine(RototateToOrigin(monster_select1, monster_select1.transform.rotation, origin_rotation, 1f));
            float remain_speed = monster1_agent.velocity.magnitude;
            while (remain_speed > 0.0f)
            {
                remain_speed = Mathf.Max(0.0f, remain_speed - 0.1f);
                anim.SetFloat("speed", remain_speed / monster1_agent.speed);
                //Debug.Log(remain_speed / monster1_agent.speed);
                yield return null;
                if (monster1.monster_stats.current_health < 0) yield break;
            }
            //anim.SetFloat("speed", 0.0f);
        }

    }

    private void CalculateStateAfterSkill(Monster monster1, Monster monster2)
    {
        if(monster1.monster_stats.skillType == MonsterStats_SO.SkillType.MeleeAttack)
        {
            monster2.monster_stats.current_health -= monster1.monster_stats.power;
            //TODO: update for client
            UpdateMonsterHealthClientRpc(monster2.GetComponent<NetworkObject>().NetworkObjectId, monster2.monster_stats.current_health);
        }
        
    }
    [ClientRpc]
    void UpdateMonsterHealthClientRpc(ulong monsterNetID, int current_health)
    {
        UIManager.Instance.SetNewObjHealthValueNearByHeart(monsterNetID, current_health);
    }

    IEnumerator JudgeDie(Monster monster)
    {
        if (monster.monster_stats.current_health <= 0)
        {
            monster.GetComponent<Animator>().SetTrigger("go_die");
            monster.GetComponent<NavMeshAgent>().enabled = false;
            monster.GetComponent<Collider>().enabled = false;
            yield return new WaitForSeconds(3f);
            monster.GetComponent<NetworkObject>().Despawn();

        }
    }
    IEnumerator GetHit(GameObject monster_select2)
    {
        NavMeshAgent agent = monster_select2.GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        monster_select2.GetComponent<Animator>().SetTrigger("get_hit");
        yield return new WaitForSeconds(monster_select2.GetComponent<Monster>().monster_stats.freeze_time);
        if (agent.enabled==false) yield break;
        agent.isStopped = false;
    }
    IEnumerator RototateToOrigin(GameObject monster, Quaternion rotationFrom, Quaternion rotationTo, float rotationTime)
    {
        float countTime = 0f;
        
        while (countTime<= rotationTime)
        {
            Debug.Log(countTime);
            monster.transform.rotation = Quaternion.Lerp(rotationFrom, rotationTo, countTime / rotationTime);
            countTime += Time.deltaTime;
            yield return null;
            if (monster == null) yield break;
        }
        
    }

    public void PlayerRayToSelectState(ulong clientID, Ray ray)
    {
        //Debug.Log("USE playerSelectStates clientID"+ clientID);
        //Debug.Log(playerSelectStates[clientID]);
        RaycastHit hitInfo;
        if(!Physics.Raycast(ray, out hitInfo)) return;
        if (hitInfo.collider==null) return;
        GameObject gameObject = hitInfo.collider.gameObject;
        if (!gameObject.CompareTag("Monster")) return;
        if (playerSelectStates[clientID].gameObject1 == null)
        {
            if (gameObject.GetComponent<Monster>().master_id != clientID) return;
            playerSelectStates[clientID].gameObject1 = gameObject;
        }
        else playerSelectStates[clientID].gameObject2 = gameObject;
        
    }
    public void SetSelectState(ulong UserID, ulong monsterID1, ulong monsterID2)
    {
        playerSelectStates[UserID].gameObject1 = ObjManager.Instance.MonsterNetIDToObj[monsterID1];
        playerSelectStates[UserID].gameObject2 = ObjManager.Instance.MonsterNetIDToObj[monsterID2];
    }
}
