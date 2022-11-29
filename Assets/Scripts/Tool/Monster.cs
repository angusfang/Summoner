using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public MonsterStats_SO monster_stats_origin;
    
    [HideInInspector]
    public MonsterStats_SO monster_stats;
    private void Awake()
    {
        monster_stats = Instantiate(monster_stats_origin);
    }
    public virtual void UseSkill(Monster target)
    {
        Debug.Log("Skill is empty");
    }
}
