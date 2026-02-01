using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using InfinityAdventure.Utils;
using System;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] GameObject _skeletonVisual;

    [SerializeField] private Player _player;

    [SerializeField] private State startingState;
    [SerializeField] private float roamindDistanceMax = 5f;
    [SerializeField] private float roamingDistanseMin = 1f;
    [SerializeField] private float roamingTimerMax = 5f;
    [SerializeField] private float roamingTimerMin = 1f;

    [SerializeField] private bool isChasingEnemy = false;
    [SerializeField] private float chasingDistance = 5f;
    [SerializeField] private float chasingSpeedMultipliyer = 2f;

    [SerializeField] private bool isAttackingEnemy;
    [SerializeField] private float attackingDistance = 1f;
    private float _attackRate = 2f;
    private float _nextAttackTime;

    private NavMeshAgent _navMeshAgent;
    private State _currentState;
    [SerializeField] private float roamingTimer;
    private Vector3 _roamPosition;
    private Vector3 _startingPosition;

    private float _roamingSpeed;
    private float _chasingSpeed;

    private float _nextCheckDirectionTime = 0f;
    private float _checkDirectionDuration = 0.1f;
    private Vector3 _lastPosition;

    public event EventHandler OnEnemyAttack;



    private enum State
    {
        Idle,
        Roaming,
        StaticRoaming,
        Chasing,
        Attacking,
        Death
    }

    private void Start()
    {
        _startingPosition = transform.position;
    }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _currentState = startingState;

        _roamingSpeed = _navMeshAgent.speed;
        _chasingSpeed = _navMeshAgent.speed * chasingSpeedMultipliyer;
    }

    private void Update()
    {
        StateHandler();
        MovementDirectionHandler();
    }

    public void SetDeathState()
    {
        _navMeshAgent.ResetPath();
        _currentState = State.Death;
    }

    public float GetRoamingAnimationSpeed()
    {
        return _navMeshAgent.speed / _roamingSpeed;
    }

    //public bool IsDeathEnemy()
    //{
    //    if (_currentState == State.Death)
    //    { 
    //        return true; 
    //    }

    //    return false;
    //}

    //private void SetStaticRoamingState()
    //{
    //    _currentState = State.StaticRoaming;
    //}

    private void StateHandler()
    {
        switch (_currentState)
        {
            case State.Roaming:
                roamingTimer -= Time.deltaTime;
                if (roamingTimer < 0)
                {
                    Roaming();
                    roamingTimer = UnityEngine.Random.Range(roamingTimerMin, roamingTimerMax);
                }
                CheckCurrentState();
                break;
            case State.StaticRoaming:
                roamingTimer -= Time.deltaTime;
                if (roamingTimer < 0)
                {
                    Roaming();
                    roamingTimer = UnityEngine.Random.Range(roamingTimerMin, roamingTimerMax);
                }
                break;
            case State.Chasing:
                ChasingTarget();
                CheckCurrentState();
                break;
            case State.Attacking:
                AttackingTarget();
                CheckCurrentState();
                break;
            case State.Death:
                DeathTarget();
                break;
            default:
            case State.Idle:
                break;
        }
    }

    private void CheckCurrentState()
    {
        State newState = State.Roaming;
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);

        //if (_player.IsDeath)
        //{
        //    newState = State.StaticRoaming;
        //}

        if (isChasingEnemy)
        {
            if (distanceToPlayer <= chasingDistance)
            {
                newState = State.Chasing;
            }
        }

        if (isAttackingEnemy)
        {
            if (distanceToPlayer <= attackingDistance)
            {
                if (!Player.Instance.IsDeath())
                {
                    newState = State.Attacking;
                }
                else
                {
                    newState = State.Roaming;
                }
            }
        }

        if (newState != _currentState)
        {
            if (newState == State.Chasing)
            {
                _navMeshAgent.ResetPath();
                _navMeshAgent.speed = _chasingSpeed;
            }
            else if (newState == State.Roaming)
            {
                //_roamingTimer = 0f;
                _navMeshAgent.speed = _roamingSpeed;
            }
            else if (newState == State.Attacking)
            {
                _navMeshAgent.ResetPath();
            }

            _currentState = newState;
        }
    }

    private void AttackingTarget()
    {
        if (Time.time > _nextAttackTime)
        {
            OnEnemyAttack.Invoke(this, EventArgs.Empty);

            _nextAttackTime = Time.time + _attackRate;
        }
    }

    private void MovementDirectionHandler()
    {
        if (Time.time > _nextCheckDirectionTime)
        {
            if (IsRunning())
            {
                ChangeFacingDirection(_lastPosition, transform.position);
            }
            else if (_currentState == State.Attacking)
            {
                ChangeFacingDirection(transform.position, Player.Instance.transform.position);
            }

            _lastPosition = transform.position;
            _nextCheckDirectionTime = Time.time + _checkDirectionDuration;
        }
    }

    private void ChasingTarget()
    {
        _navMeshAgent.SetDestination(Player.Instance.transform.position);
    }

    public bool IsRunning()
    {
        if (_navMeshAgent.velocity == Vector3.zero)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void Roaming()
    {
        _startingPosition = transform.position;
        _roamPosition = GetRoamingPosition();
        ChangeFacingDirection(_startingPosition, _roamPosition);
        _navMeshAgent.SetDestination(_roamPosition);
    }

    private Vector3 GetRoamingPosition()
    {
        return _startingPosition + Utils.GetRandomDir() * UnityEngine.Random.Range(roamingDistanseMin, roamindDistanceMax);
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        if (sourcePosition.x > targetPosition.x)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void DeathTarget()
    {
        
    }
}
