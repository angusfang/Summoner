using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/MonsterState/Die", fileName = "MonsterStateDie")]
public class MonsterStateDie : MonsterState
{
    [SerializeField] float DieTime;
    float DieCountDown;
    public override void Enter()
    {
        animator.SetBool("Die", true);
        collider.enabled = false;
        agentObstacle.enabled = false;
        collider.enabled = true;
        DieCountDown = DieTime;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        DieCountDown -= Time.deltaTime;
        if (DieCountDown <= 0)
        {
            monster.GetComponent<NetworkObject>().Despawn();
        }
    }
    public override void Exit()
    {
        animator.SetBool("Die", false);
    }
}
