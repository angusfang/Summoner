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

    private List<GameObject> Player1Monsters;
    // Start is called before the first frame update
    void Start()
    {
        Player1Monsters = new List<GameObject>();

        //TODO: monster should be spawn next to the summonor;
        Vector3 spawnPosition = new Vector3(0f, 0f, 0f);
        //TODO: monster should be Instantiate after player click the monster icon in magic book
        Player1Monsters.Add((GameObject)Instantiate(MushRoom, spawnPosition, Quaternion.identity));
        Player1Monsters.Add((GameObject)Instantiate(DragonNightmare, spawnPosition, Quaternion.identity));
        Player1Monsters.Add((GameObject)Instantiate(DragonNightmare, spawnPosition, Quaternion.identity));
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
            Monster monster1 = monster_select1.GetComponent<Monster>();
            Monster monster2 = monster_select2.GetComponent<Monster>();
            NavMeshAgent monster1_agent = monster_select1.GetComponent<NavMeshAgent>();
            NavMeshAgent monster2_agent = monster_select2.GetComponent<NavMeshAgent>();
            StartCoroutine( Perform_skill(monster1, monster2, monster1_agent, monster2_agent));
            
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
    IEnumerator Perform_skill(Monster monster1, Monster monster2, NavMeshAgent monster1_agent, NavMeshAgent monster2_agent)
    {
        //TODO: mana check, CD check
        Quaternion origin_position_rotation = monster1_agent.transform.rotation;
        Vector3 origin_position = monster1.transform.position;
        if (monster1.monster_stats.need_walk)
        {
            Vector3 target_position = monster2.transform.position;
            Vector3 direction_o2t = (target_position - origin_position).normalized;
            Vector3 attack_range = (monster1_agent.radius + monster2_agent.radius) * direction_o2t;
            monster1_agent.destination = target_position - attack_range;
            while(Vector3.Distance(monster1_agent.destination, monster1_agent.transform.position) > 0.1 ) yield return null;
        }
        yield return new WaitForSeconds(monster1.monster_stats.perform_skill_time_point);
        monster1.UseSkill(monster2);
        Debug.Log(monster2.monster_stats.current_health);
        yield return new WaitForSeconds(Mathf.Max(0, monster1.monster_stats.animation_duration - monster1.monster_stats.perform_skill_time_point));
        if (monster1.monster_stats.need_walk)
        {
            monster1_agent.destination = monster1_agent.destination + (origin_position- monster1_agent.destination)*1.5f;
            while (Vector3.Distance(monster1_agent.destination, monster1_agent.transform.position) > 0.1) yield return null;
            monster1_agent.destination = origin_position;
        }
        
    }
}
