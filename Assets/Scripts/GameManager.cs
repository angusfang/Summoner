using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject MushRoom;
    [SerializeField] private GameObject DragonNightmare;
    //Dictionary<ulong,>

    private List<GameObject> Player1Monsters;
    // Start is called before the first frame update
    void Start()
    {
        Player1Monsters = new List<GameObject>();

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
        GameObject monster_select1, monster_select2;
        bool effective_select;
        (effective_select, monster_select1, monster_select2) = DealSelectState();
        if (effective_select)
        {
            
            //Debug.Log(monster_select1.name + " act to " + monster_select2.name);
            StartCoroutine( PerformSkill(monster_select1, monster_select2));
            
            //monster_select1.GetComponent<NavMeshAgent>().destination = monster_select2.transform.position;
            
            
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(Player1Monsters.Count);
            foreach (GameObject monster in Player1Monsters)
            {
                Debug.Log(monster.name);
            }
        }
    }
    private (bool, GameObject, GameObject) DealSelectState()
    {
        GameObject monster_select1, monster_select2;
        bool effective_select;
        Player1SelectManager.Instance.GetSelectState();
        (monster_select1, monster_select2) = Player1SelectManager.Instance.GetSelectState();
        if (monster_select2 != null) {
            Player1SelectManager.Instance.CleanSelect();
            effective_select = true;
            return (effective_select, monster_select1, monster_select2);
        }
        effective_select = false;
        return (effective_select, monster_select1, monster_select2);
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
        float range_between_pos_tar = monster1_agent.stoppingDistance;

        // Walk front
        if (monster1.monster_stats.need_walk)
        {
            monster1_agent.destination = target_position - attack_range;
            while (Vector3.Distance(monster1_agent.destination, monster1_agent.transform.position) > range_between_pos_tar) {
                anim.SetFloat("speed", monster1_agent.velocity.magnitude/monster1_agent.speed);
                yield return null;
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
            monster1_agent.destination = monster1_agent.destination + (origin_position- monster1_agent.destination)*1.5f;
            while (Vector3.Distance(monster1_agent.destination, monster1_agent.transform.position) > range_between_pos_tar) {
                anim.SetFloat("speed", monster1_agent.velocity.magnitude / monster1_agent.speed);
                yield return null;
            } 
            monster1_agent.destination = origin_position;
            while (Vector3.Distance(monster1_agent.destination, monster1_agent.transform.position) > range_between_pos_tar)
            {
                anim.SetFloat("speed", monster1_agent.velocity.magnitude / monster1_agent.speed);
                yield return null;
            }
            float remain_speed = monster1_agent.velocity.magnitude;
            while (remain_speed > 0.0f)
            {
                remain_speed = Mathf.Max(0.0f, monster1_agent.velocity.magnitude-0.0f);
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
        yield return new WaitForSeconds(monster_select2.GetComponent < Monster >().monster_stats.freeze_time);
        monster_select2.GetComponent<NavMeshAgent>().isStopped = false;
    }
}
