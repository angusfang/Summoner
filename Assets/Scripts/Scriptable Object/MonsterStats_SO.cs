using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="MonsterStats",menuName ="Monster")]
public class MonsterStats_SO : ScriptableObject
{
    public enum SkillType
    {
        MeleeAttack,
    }

    
    public int max_health;
    public int current_health;
    public int CD;
    public int current_CD;
    public int cost;
    public int power;
    public float animation_duration;
    public float perform_skill_time_point;
    public float freeze_time;
    public bool need_walk;
    public bool is_damage;
    public SkillType skillType;
}
