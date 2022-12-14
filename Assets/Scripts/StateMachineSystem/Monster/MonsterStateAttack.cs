using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/MonsterState/Attack", fileName = "MonsterStateAttack")]
public class MonsterStateAttack : MonsterState
{
    [SerializeField] float AttackDuration;
    [SerializeField] float AttackPrepareTime;
    float attackCountDown;
    bool alreadyAttack;
    public override void Enter()
    {
        animator.SetBool("Attack", true);
        agent.enabled = false;
        agentObstacle.enabled = true;
        collider.enabled = true;
        attackCountDown = AttackDuration;
        alreadyAttack = false;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (stateMachine.HurtSignal) stateMachine.SwitchState(typeof(MonsterStateHurt));
        if (attackCountDown > AttackPrepareTime) return;
        if (!alreadyAttack && stateMachine.AttackSignal)
        {
            stateMachine.TargetMonster.stateMachine.HurtSignal = true;
            stateMachine.TargetMonster.stateMachine.ReceiveDamage = monster.MonsterStats.power;
            stateMachine.AttackSignal = false;
            alreadyAttack = true;
        }
        if (attackCountDown > 0) return;
        stateMachine.SwitchState(typeof(MonsterStateMove));
    }

    public override void Exit()
    {
        animator.SetBool("Attack", false);
        stateMachine.GoBackSignal = true;
    }


}
