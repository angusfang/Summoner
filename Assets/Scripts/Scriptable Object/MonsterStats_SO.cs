using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="MonsterStats",menuName ="Monster")]
public class MonsterStats_SO : ScriptableObject
{
    public int max_health;
    public int current_health;
    public int CD;
    public int current_CD;
    public int cost;
    public int power;
    public float animation_duration;
    public float perform_skill_time_point;
    public bool need_walk;
    
}
