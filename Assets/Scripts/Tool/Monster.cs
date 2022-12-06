using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Monster : NetworkBehaviour
{
    public MonsterStats_SO monster_stats_origin;
    public ulong master_id;
    [HideInInspector]
    public MonsterStats_SO monster_stats;
    public override void OnNetworkSpawn()
    {
        monster_stats = Instantiate(monster_stats_origin);
    }
    
    public virtual void UseSkill(Monster target)
    {
        Debug.Log("Skill is empty");
    }
}
