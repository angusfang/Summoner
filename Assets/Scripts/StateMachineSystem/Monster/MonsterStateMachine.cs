using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;

public class MonsterStateMachine : StateMachine
{
    public struct Str_targetMonster
    {
        public GameObject gameObject;
        public Monster monster;
        public NavMeshAgent agent;
    }

    bool m_attackSignal;
    bool m_hurtSignal;
    bool m_timeUpSignal;
    bool m_goBackSignal;
    bool m_collideSignal;
    float m_countDown;

    IState previousState;

   
    Str_targetMonster m_targetMonster;
    public bool AttackSignal { get { return m_attackSignal; } set { m_attackSignal = value; } }
    public bool HurtSignal { get { return m_hurtSignal; } set { m_hurtSignal = value; } }
    public bool TimeUpSignal { get { return m_timeUpSignal; } set { m_timeUpSignal = value; } }
    public bool GoBackSignal { get { return m_goBackSignal; } set { m_goBackSignal = value; } }
    public bool CollideSignal { get { return m_collideSignal; } set { m_collideSignal = value; } }
    public Str_targetMonster TargetMonster { get { return m_targetMonster; } set { m_targetMonster = value; } }
    public float CountDown { get { return m_countDown; } set { m_countDown = value; } }
    
    public override void SwitchState(System.Type newStateType)
    {
        if(newStateType != typeof(MonsterStateHurt)&& newStateType != typeof(MonsterStateDie)) previousState = currentState;

        SwitchState(stateTable[newStateType]);
    }
    public void SwitchToPreviousState(System.Type newStateType)
    {
        SwitchState(previousState);
    }



    // Start is called before the first frame update
    void Start()
    {
        m_attackSignal = false;
        m_hurtSignal = false;
        m_timeUpSignal = false;
        m_goBackSignal = false;
        m_collideSignal = false;
        m_countDown = 0;

        m_targetMonster = new Str_targetMonster() { gameObject = null, agent=null, monster = null};
    }

    // Update is called once per frame

}
