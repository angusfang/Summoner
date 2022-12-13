using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/MonsterState/Idle", fileName = "MonsterStateIdle")]
public class MonsterStateIdle :MonsterState
{
    public override void Enter()
    {
        agent.enabled = false;
        agentObstacle.enabled = true;
        collider.enabled = true;
    }
    public override void LogicUpdate()
    {
        if (stateMachine.HurtSignal) stateMachine.SwitchState(typeof(MonsterStateHurt));
        else if (stateMachine.AttackSignal)
        {
            if(Vector3.Distance(stateMachine.TargetMonster.agent.transform.position, agent.transform.position)
                - stateMachine.TargetMonster.agent.radius - agent.radius <= monsterStat.attack_range) stateMachine.SwitchState(typeof(MonsterStateAttack));
            else
            {
                stateMachine.SwitchState(typeof(MonsterStateMove));
            }
        }
        else if (stateMachine.GoBackSignal)
        {
            if (Vector3.Distance(agent.transform.position, monster.OriginalPosition) <= agent.stoppingDistance) stateMachine.GoBackSignal = false;
            else
            {
                stateMachine.SwitchState(typeof(MonsterStateMove));
            }
        }
    }
    public override void Exit()
    {
        stateMachine.CollideSignal= false;
    }
}
