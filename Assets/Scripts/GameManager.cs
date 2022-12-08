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
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class GameManager : NetworkBehaviour
{
    int maxPlayer = 6;
    private static GameManager instance;
    public static GameManager Instance => instance;
    
    //[SerializeField] private GameObject MushRoom;
    //[SerializeField] private GameObject DragonNightmare;

    //Dictionary<ulong,>
    struct PlayerSelectState
    {
        
        public GameObject gameObject1;
        public GameObject gameObject2;
    };

    private PlayerSelectState[] playerSelectStates;
    //private List<GameObject> Player1Monsters;
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        //if (!IsServer) return;
        //Player1Monsters = new List<GameObject>();
        if (instance != null) Destroy(gameObject);
        else instance = this;
        playerSelectStates = new PlayerSelectState[maxPlayer];
        //for (int i = 0; i < 4; i++) {
        //    playerSelectStates[i].gameObject1 = new GameObject();
        //    playerSelectStates[i].gameObject2 = new GameObject();
        //}
        //Debug.Log("playerSelectStates init");
        //Debug.Log(playerSelectStates[0]);

        //TODO: monster should be spawn next to the summonor;
        //Vector3 spawnPosition = new Vector3(0f, 0f, 0f);
        //TODO: monster should be Instantiate after player click the monster icon in magic book
        //Player1Monsters.Add((GameObject)Instantiate(MushRoom, spawnPosition, Quaternion.identity));
        //Player1Monsters.Add((GameObject)Instantiate(DragonNightmare, spawnPosition, Quaternion.identity));
        //Player1Monsters.Add((GameObject)Instantiate(DragonNightmare, spawnPosition, Quaternion.identity));
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
       
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    Debug.Log(Player1Monsters.Count);
        //    foreach (GameObject monster in Player1Monsters)
        //    {
        //        Debug.Log(monster.name);
        //    }
        //}
    }

    IEnumerator PerformSkill(GameObject monster_select1, GameObject monster_select2)
    {
        
        Monster monster1 = monster_select1.GetComponent<Monster>();
        Monster monster2 = monster_select2.GetComponent<Monster>();
        NavMeshAgent monster1_agent = monster_select1.GetComponent<NavMeshAgent>();
        NavMeshAgent monster2_agent = monster_select2.GetComponent<NavMeshAgent>();
        //TODO: mana check, CD check
        Vector3 origin_position = monster1.transform.position;
        Vector3 target_position = monster2.transform.position;
        Vector3 direction_o2t = (target_position - origin_position).normalized;
        Vector3 attack_range = (monster1_agent.radius + monster2_agent.radius) * direction_o2t;
        Animator anim = monster_select1.GetComponent<Animator>();
        Quaternion origin_rotation = Quaternion.LookRotation(Vector3.zero-origin_position);
        float range_between_pos_tar = monster1_agent.stoppingDistance;

        // Walk front
        if (monster1.monster_stats.need_walk)
        {
            monster1_agent.destination = target_position - attack_range;
            while (Vector3.Distance(monster1_agent.destination, monster1_agent.transform.position) > range_between_pos_tar) {
                anim.SetFloat("speed", monster1_agent.velocity.magnitude / monster1_agent.speed);
                yield return null;
                target_position = monster2.transform.position;
                monster1_agent.destination = target_position - attack_range;
            }
        }

        // Perform skill
        anim.SetTrigger("perform_skill");
        monster1_agent.isStopped = true;

        // Hit target
        Debug.Log("play skill animation");
        yield return new WaitForSeconds(monster1.monster_stats.perform_skill_time_point);
        if (monster1.monster_stats.is_damage) StartCoroutine(GetHit(monster_select2));
        monster1.UseSkill(monster2);

        Debug.Log(monster2.monster_stats.current_health);
        monster1_agent.isStopped = false;
        yield return new WaitForSeconds(Mathf.Max(0, monster1.monster_stats.animation_duration - monster1.monster_stats.perform_skill_time_point));

        // Walk back
        if (monster1.monster_stats.need_walk)
        {
            monster1_agent.destination = origin_position;
            while (Vector3.Distance(monster1_agent.destination, monster1_agent.transform.position) > range_between_pos_tar) {
                anim.SetFloat("speed", monster1_agent.velocity.magnitude / monster1_agent.speed);
                yield return null;
            }

            StartCoroutine(RototateToOrigin(monster_select1, monster_select1.transform.rotation, origin_rotation, 1f));
            float remain_speed = monster1_agent.velocity.magnitude;
            while (remain_speed > 0.0f)
            {
                remain_speed = Mathf.Max(0.0f, remain_speed - 0.1f);
                anim.SetFloat("speed", remain_speed / monster1_agent.speed);
                //Debug.Log(remain_speed / monster1_agent.speed);
                yield return null;
            }
            //anim.SetFloat("speed", 0.0f);
        }

    }
    IEnumerator GetHit(GameObject monster_select2)
    {
        monster_select2.GetComponent<NavMeshAgent>().isStopped = true;
        monster_select2.GetComponent<Animator>().SetTrigger("get_hit");
        yield return new WaitForSeconds(monster_select2.GetComponent<Monster>().monster_stats.freeze_time);
        monster_select2.GetComponent<NavMeshAgent>().isStopped = false;
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
}
