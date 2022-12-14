using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class MonsterStateMachine : StateMachine
{
    [SerializeField] MonsterState[] states;
    public struct Str_targetMonster
    {
        public GameObject gameObject;
        public Monster monster;
        public NavMeshAgent agent;
        public MonsterStateMachine stateMachine;
    }

    bool m_attackSignal;
    bool m_hurtSignal;
    bool m_timeUpSignal;
    bool m_goBackSignal;
    bool m_collideSignal;
    bool m_isDie;
    //float m_countDown;

    int m_receiveDamage;
    IState previousState;
    Str_targetMonster m_targetMonster;

    public bool AttackSignal { get { return m_attackSignal; } set { m_attackSignal = value; } }
    public bool HurtSignal { get { return m_hurtSignal; } set { m_hurtSignal = value; } }
    public bool TimeUpSignal { get { return m_timeUpSignal; } set { m_timeUpSignal = value; } }
    public bool GoBackSignal { get { return m_goBackSignal; } set { m_goBackSignal = value; } }
    public bool CollideSignal { get { return m_collideSignal; } set { m_collideSignal = value; } }
    public int ReceiveDamage { get { return m_receiveDamage; } set { m_receiveDamage = value; } }
    public Str_targetMonster TargetMonster { get { return m_targetMonster; } set { m_targetMonster = value; } }
    //public float CountDown { get { return m_countDown; } set { m_countDown = value; } }
    public bool IsDie { get { return m_isDie; } set { m_isDie = value; } }

    public override void SwitchState(System.Type newStateType)
    {
        if (stateTable[newStateType] == stateTable[typeof(MonsterStateHurt)] && currentState != stateTable[typeof(MonsterStateHurt)]) previousState = currentState;
        SwitchState(stateTable[newStateType]);
    }
    public void FromHurtSwitchToPreviousState()
    {
        SwitchState(previousState);
    }

    private void Awake()
    {
        stateTable = new Dictionary<Type, IState>(states.Length);
        foreach (MonsterState state in states)
        {
            state.Initialize(GetComponent<Animator>(), GetComponent<NavMeshAgent>(), GetComponent<NavMeshObstacle>(), GetComponent<Collider>(),
                GetComponent<MonsterStateMachine>(), gameObject, GetComponent<Monster>());
            stateTable.Add(state.GetType(), state);
        }
        m_attackSignal = false;
        m_hurtSignal = false;
        m_timeUpSignal = false;
        m_goBackSignal = false;
        m_collideSignal = false;
        m_receiveDamage = 0;
        m_targetMonster = new Str_targetMonster() { gameObject = null, agent = null, monster = null };
    }

    // Start is called before the first frame update
    void Start()
    {
        SwitchOn(stateTable[typeof(MonsterStateIdle)]);
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        CollideSignal = true;
    }

    
}
