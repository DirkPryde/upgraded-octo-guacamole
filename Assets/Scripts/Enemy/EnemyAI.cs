using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {

    public enum EnemyType
    {
        MINION,
        SHOOTER,
        BRUTE
    }

    public enum State
    {
        PATROL,
        REGROUP,
        CHASE,
        HUNT,
        STUNNED
    }

    // Variables for all Enemies
    public EnemyType m_EnemyType; // IMPORTANT: Must be set when attaching to EnemyPrefab. The default State will be set based on this.
    public State m_State;    
    public float m_ChaseSpeed = 4f;
    public float m_StunDuration = 3f;
    public bool m_IsAlive = true;
    private EnemyController m_EnemyController;
    private NavMeshAgent m_NavAgent;
    private GameObject m_Player;    

    // Variables for Minions    
    public float m_MinionRegroupSpeed = 4f;
    public float m_MinionRegroupDistance = 8f;    
    private Vector3 m_MinionAvgGroupPosition;

    // Variables for Shooters
    public float m_ShooterMaxDistance = 16f;
    public float m_ShooterRetreatSpeed = 8f;
    public float m_ShooterRetreatDistance = 8f;

    // Variables for Brutes
    public float m_BrutePatrolSpeed = 3f;
    private GameObject m_PatrolRoute;
    private int m_PatrolNodeIndex = 0;

    // Use this for initialization
    void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_PatrolRoute = GameObject.FindGameObjectWithTag("PatrolRoute");
        m_PatrolNodeIndex = Random.Range(0, m_PatrolRoute.transform.childCount);
        m_NavAgent = GetComponent<NavMeshAgent>();
        m_IsAlive = true;
        m_EnemyController = GetComponentInParent<EnemyController>();

        // Set the starting State for each EnemyType
        SetDefaultState();

        // Start the Finite State Machine
        StartCoroutine("FSM");
    }

    IEnumerator FSM()
    {
        while (m_IsAlive)
        {
            switch (m_State)
            {
                case State.REGROUP:
                    Regroup();
                    break;
                case State.PATROL:
                    Patrol();
                    break;
                case State.CHASE:
                    Chase();
                    break;
                case State.HUNT:
                    Hunt();
                    break;
                case State.STUNNED:
                    // Stop the Enemy from moving
                    m_NavAgent.isStopped = true;
                    // Calls the Resume function in m_StunDuration seconds
                    Invoke("Resume", m_StunDuration);
                    break;
            }
            yield return null;
        }
    }

    // FUNCTION FOR PATROL STATE
    private void Patrol()
    {
        if (m_EnemyType != EnemyType.BRUTE)
        {
            SetDefaultState();
            return;
        }

        m_NavAgent.speed = m_BrutePatrolSpeed;
        float distance = Vector3.Distance(transform.position, m_PatrolRoute.transform.GetChild(m_PatrolNodeIndex).transform.position);

        Vector3 direction = m_Player.transform.position - transform.position;
        RaycastHit hitInfo;

        // if "canSeePlayer" set mode to CHASE else continue to PATROL
        if (Physics.Raycast(transform.position, direction, out hitInfo) && (hitInfo.collider.tag == "Player"))
        {
            m_State = State.CHASE;
        }
        else
        {
            if (distance > 2f)
            {
                m_NavAgent.SetDestination(m_PatrolRoute.transform.GetChild(m_PatrolNodeIndex).transform.position);
            }
            else if (distance <= 2f)
            {
                m_PatrolNodeIndex = Random.Range(0, m_PatrolRoute.transform.childCount);
                m_NavAgent.SetDestination(m_PatrolRoute.transform.GetChild(m_PatrolNodeIndex).transform.position);
            }
            else
            {
                m_NavAgent.isStopped = true;
            }
        }
    }

    // FUNCTION FOR REGROUP STATE
    private void Regroup()
    {
        if (m_EnemyType != EnemyType.MINION)
        {
            SetDefaultState();
            return;
        }

        m_NavAgent.speed = m_MinionRegroupSpeed;
        m_MinionAvgGroupPosition = m_EnemyController.m_MinionGroupPosition;
        float distance = Vector3.Distance(transform.position, m_MinionAvgGroupPosition);
        Vector3 direction = m_Player.transform.position - transform.position;
        RaycastHit hitInfo;

        if ((distance < m_MinionRegroupDistance) || (Physics.Raycast(transform.position, direction, out hitInfo) && (hitInfo.collider.tag == "Player")))
        {
            m_State = State.CHASE;
        }
        else
        {
            m_NavAgent.SetDestination(m_MinionAvgGroupPosition);
        }
    }

    // FUNCTION FOR CHASE STATE
    private void Chase()
    {
        // If the player is far enough away AND not in LoS return to PATROL else keep CHASING
        // Currently enemy units block LoS, I have not managed to get ignore LayerMasks working yet 
        Vector3 direction = m_Player.transform.position - transform.position;
        float distance = Vector3.Distance(transform.position, m_Player.transform.position);
        RaycastHit hitInfo;

        if ( (distance > 10f) && Physics.Raycast(transform.position, direction, out hitInfo) && (hitInfo.collider.tag != "Player") )
        {
            SetDefaultState();
        }
        else
        {
            m_NavAgent.speed = m_ChaseSpeed;
            m_NavAgent.SetDestination(m_Player.transform.position);
        }
    }

    // FUNCTION FOR HUNT STATE
    private void Hunt()
    {
        if (m_EnemyType != EnemyType.SHOOTER)
        {
            SetDefaultState();
            return;
        }

        float distance = Vector3.Distance(transform.position, m_Player.transform.position);
        Vector3 direction = transform.position - m_Player.transform.position; // AWAY from the player

        if (distance > m_ShooterMaxDistance)
        {
            m_NavAgent.speed = m_ChaseSpeed;
            m_NavAgent.SetDestination(m_Player.transform.position);
            m_NavAgent.isStopped = false;
        }
        else if (distance < m_ShooterRetreatDistance)
        {
            m_NavAgent.speed = m_ShooterRetreatSpeed;
            m_NavAgent.SetDestination(transform.position + direction);
            m_NavAgent.isStopped = false;
        }
        else
        {
            m_NavAgent.isStopped = true;
        }
    }
    
    // FUNTION FOR STUNNED STATE
    private void Resume()
    {
        m_NavAgent.isStopped = false;
        SetDefaultState();
    }

    // FUNCTION TO SET THE DEFAULT STATE FOR EACH ENEMYTYPE
    private void SetDefaultState()
    {
        switch (m_EnemyType)
        {
            case EnemyType.MINION:
                m_State = State.REGROUP;
                break;
            case EnemyType.SHOOTER:
                m_State = State.CHASE;
                break;
            case EnemyType.BRUTE:
                m_State = State.PATROL;
                break;
            default:
                break;
        }
    }
}