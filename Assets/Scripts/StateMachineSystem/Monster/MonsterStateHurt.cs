using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/MonsterState/Hurt", fileName = "MonsterStateHurt")]
public class MonsterStateHurt : MonsterState {
    [SerializeField] float freezeTime;
    float freezeCountDown;
    public override void Enter()
    {
        agent.enabled= false;
        agentObstacle.enabled= true;
        collider.enabled= true;

        animator.SetBool("Hurt", true);
        freezeCountDown = freezeTime;
        monster.MonsterStats.current_health -= stateMachine.ReceiveDamage;
        if(monster.MonsterStats.current_health<=0) stateMachine.SwitchState(typeof(MonsterStateDie));
        stateMachine.HurtSignal = false;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (stateMachine.HurtSignal) {
            animator.SetTrigger("HurtTri");
            stateMachine.SwitchState(typeof(MonsterStateHurt));
        }
        
        if (freezeCountDown <= 0) stateMachine.FromHurtSwitchToPreviousState();
        freezeCountDown -= Time.deltaTime;
    }
    public override void Exit()
    {
        animator.SetBool("Hurt", false);
    }
}
