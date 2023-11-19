using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    [Header("Vision and Hearing")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform player_pos;
    [SerializeField] private float visionRange;
    [SerializeField] private float visionConeAngle;
    [SerializeField] private float hearingRange;
    [SerializeField] private float attackRange;
    [SerializeField] private TextMeshProUGUI stateIndicator;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Vector2 Direction; 

    [Header("Movement")]
    [SerializeField] private Transform[] points;
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float ChaseDistance;

    private CharacterController controller;

    private int indexOfTarget;
    private Vector3 targetPosition;

    NavMeshAgent agent;

    private bool playerIsMoving = false;

    private State state = State.Patrol;

    float GetDistanceToPlayer()
    {
        return
            (player.transform.position - transform.position)
            .magnitude;
    }

    float GetAngleToPlayer()
    {
        Vector3 directionToPlayer =
            (player.transform.position - transform.position)
            .normalized;
        return Vector3.Angle(transform.forward, directionToPlayer);
    }

    bool SightLineObstructed()
    {
        Vector3 vectorToPlayer = player.transform.position - transform.position;
        Ray ray = new Ray(
            transform.position,
            vectorToPlayer);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, vectorToPlayer.magnitude))
        {
            GameObject obj = hitInfo.collider.gameObject;
            return obj != player;
        }
        return false;
    }

    bool CanSeePlayer()
    {
        if (GetDistanceToPlayer() < visionRange && !SightLineObstructed() && GetAngleToPlayer() < visionConeAngle)
        {
            return true;
        }
        return false;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        indexOfTarget = -1;
        NextTarget();
        LookAtTarget();
    }


    // Update is called once per frame
    void Update()
    {
        Direction = playerMovement.Direction;
        switch (state)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Confused:
                Confused();
                break;
            case State.Attack:
                Attack();
                break;
        }
        if (Direction.magnitude >= 0.1f)
        {
            playerIsMoving = true;
        }
        else
        {
            playerIsMoving = false;
        }
    }

    void NextTarget()
    {
        indexOfTarget = (indexOfTarget + 1) % points.Length;
        targetPosition = points[indexOfTarget].position;
        targetPosition.y = transform.position.y;
    }
    void LookAtPlayer()
    {
        Vector3 lookAtP = player_pos.position;
        lookAtP.y = transform.position.y;

        Vector3 PlookDir = (lookAtP - transform.position).normalized;
        transform.forward = PlookDir;
    }
    void Patrol()
    {
        if (CanSeePlayer())
        {
            state = State.Chase;
        }
        if (!CanSeePlayer() && GetDistanceToPlayer() < hearingRange && playerIsMoving)
        {
            state = State.Confused;
        }

        stateIndicator.text = "Patrolling...";

        if (agent.remainingDistance < agent.radius)
        {
            NextTarget();
        }
        agent.SetDestination(targetPosition);
    }

    void LookAtTarget()
    {
        Vector3 lookAt = targetPosition;
        lookAt.y = transform.position.y;

        Vector3 lookDir = (lookAt - transform.position).normalized;
        transform.forward = lookDir;
    }

    void Chase()
    {
        if (!CanSeePlayer())
        {
            state = State.Patrol;
        }

        stateIndicator.text = "Chase!";
        LookAtPlayer();
        Vector3 velocity = player_pos.position - transform.position;
        velocity.Normalize();
        velocity *= moveSpeed * Time.deltaTime;
        controller.Move(velocity);

        if ((transform.position - player_pos.position).magnitude > ChaseDistance)
        {
            state = State.Patrol;
            LookAtTarget();
        }
        if ((transform.position - player_pos.position).magnitude < attackRange)
        {
            state = State.Attack;
            LookAtTarget();
        }
    }
    void Attack()
    {
        if (!CanSeePlayer())
        {
            state = State.Patrol;
        }

        stateIndicator.text = "Attack!";
        LookAtPlayer();
        Vector3 velocity = player_pos.position - transform.position;
        velocity.Normalize();
        velocity *= moveSpeed * Time.deltaTime;
        controller.Move(velocity);

        if ((transform.position - player_pos.position).magnitude > attackRange)
        {
            state = State.Chase;
            LookAtTarget();
        }
    }

    void Confused()
    {
        if (!CanSeePlayer() && !playerIsMoving)
        {
            state = State.Patrol;
        }
        if (CanSeePlayer())
        {
            state = State.Chase;
        }
        stateIndicator.text = "Confused...";
    }

    enum State
    {
        Patrol,
        Chase,
        Attack,
        Confused
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Player Weapon")
        {
            Destroy(gameObject);
        }
    }
}
