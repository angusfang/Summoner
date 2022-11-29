using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonNightmare1 : Monster
{
    
    public override void UseSkill(Monster target)
    {
        target.monster_stats.current_health = Mathf.Max(target.monster_stats.current_health - monster_stats.power, 0);
    }
}
