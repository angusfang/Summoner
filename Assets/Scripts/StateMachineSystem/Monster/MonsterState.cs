using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

public class MonsterState : ScriptableObject, IState
{
    protected Animator animator;
    protected NavMeshAgent agent;
    protected NavMeshObstacle agentObstacle;
    protected Collider collider;
    protected MonsterStateMachine stateMachine;
    protected GameObject gameObject;
    protected MonsterStats_SO monsterStat;
    protected Monster monster;




    public virtual void Enter()
    {
        
    }

    public virtual void Exit()
    {

    }

    public virtual void LogicUpdate()
    {

    }
    public void Initialize(Animator animator, NavMeshAgent agent, NavMeshObstacle agentObstacle, Collider collider, MonsterStateMachine stateMachine, GameObject gameObject,
        MonsterStats_SO monsterStat, Monster monster)
    {
        this.animator = animator;
        this.agent = agent;
        this.agentObstacle = agentObstacle;
        this.collider= collider;
        this.stateMachine = stateMachine;
        this.gameObject = gameObject;
        this.monsterStat = monsterStat;
        this.monster = monster;
    }

    
    
}
