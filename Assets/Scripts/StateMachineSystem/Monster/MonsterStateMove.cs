using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/MonsterState/Move", fileName = "MonsterStateMove")]
public class MonsterStateMove : MonsterState
{
    Vector3 destination;
    Vector3 tempDestination;
    bool detour;
    public override void Enter()
    {
        animator.SetBool("Move", true);
        agent.enabled = true;
        agentObstacle.enabled = false;
        collider.enabled = true;

        stateMachine.CollideSignal = false;
        detour = false;

    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        animator.SetFloat("Speed", agent.velocity.magnitude*1.5f/agent.speed);
        destination = stateMachine.AttackSignal ? stateMachine.TargetMonster.agent.transform.position : monster.OriginalPosition;

        if (stateMachine.CollideSignal)
        {
            stateMachine.CollideSignal = false;
            detour = true;
            tempDestination = gameObject.transform.right*agent.radius + gameObject.transform.position;
        }
        if (detour)
        {
            agent.destination= tempDestination;
            if (Vector3.Distance(tempDestination, agent.transform.position) <= agent.stoppingDistance) detour = false;
        }
        else
        {
            agent.destination = destination;
        }

        if (stateMachine.HurtSignal) stateMachine.SwitchState(typeof(MonsterStateHurt));
        else if (stateMachine.AttackSignal)
        {
            if (Vector3.Distance(destination, agent.transform.position)
                - stateMachine.TargetMonster.agent.radius - agent.radius <= monster.MonsterStats.attack_range) stateMachine.SwitchState(typeof(MonsterStateAttack));
        }
        else if (stateMachine.GoBackSignal)
        {
            if (Vector3.Distance(destination, agent.transform.position) <= agent.stoppingDistance) {
                stateMachine.GoBackSignal = false;
                stateMachine.SwitchState(typeof(MonsterStateIdle));
            }
            
        }
    }
    public override void Exit()
    {
        animator.SetBool("Move", false);
    }

}
