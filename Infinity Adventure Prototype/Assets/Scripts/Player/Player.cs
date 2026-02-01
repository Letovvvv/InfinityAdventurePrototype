using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;

    public static Player Instance { get; private set; }
    public event EventHandler OnPlayerDeath;
    public event EventHandler OnFlashBlink;

    [SerializeField] private float initialMovingSpeed  = 4f;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float damageRecoveryTime = 0.3f;
    [Space( height: 10 )]
    [SerializeField] private int dashSpeeedMultiplier = 5;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private float dashCooldown = 1f;

    Vector2 inputVector;

    private Rigidbody2D _rigidBody2D;
    private KnockBack _knockBack;

    private float _minMovingSpeed = 0.1f;
    private bool _isRunning = false;
    private bool _isTurnLeft = false;
    private bool _isDashing = false;

    private int _currentHealth;
    private bool _canTakeDamage = true;
    private bool _isDeath = false;
    private float _movingSpeed;


    private void Awake()
    {
        Instance = this;
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _knockBack = GetComponent<KnockBack>();

        _movingSpeed = initialMovingSpeed;
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
        _currentHealth = maxHealth;

        GameInput.Instance.OnPlayerDash += GameInput_OnPlayerDash;
    }

    private void GameInput_OnPlayerDash(object sender, EventArgs e)
    {
        Dash();
    }

    private void Update()
    {
        inputVector = GameInput.Instance.GetMovementVector();
    }

    private void FixedUpdate()
    {
        if (_knockBack.IsGettingKnockedBack)
        {
            return;
        }
        HandleMovement();
    }

    public void TakeDamage(Transform damageSource, int damage)
    {
        if (_canTakeDamage && !_isDeath)
        {
            _canTakeDamage = false;
            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            Debug.Log(_currentHealth);
            _knockBack.GetKnockedBack(damageSource);

            OnFlashBlink?.Invoke(this, EventArgs.Empty);

            StartCoroutine(DamageRecoveryRoutine()); // Ќе останавливает весь код, но через заданное в корутине врем€ сделаетс€ то что в корутине
        }

        DetectDeath();
    }

    public bool IsDeath() => _isDeath;

    private void Dash()
    {
        if (!_isDashing) { StartCoroutine(DashRoutine());  }
    }

    private IEnumerator DashRoutine()
    {
        _isDashing = true;
        _movingSpeed *= dashSpeeedMultiplier;
        trailRenderer.emitting = true;

        yield return new WaitForSeconds(dashTime);

        _movingSpeed = initialMovingSpeed;
        trailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCooldown);

        _isDashing = false;

    }

    private void DetectDeath()
    {
        if(_currentHealth == 0 && !_isDeath)
        {
            _knockBack.StopKnockBackMovement();
            _canTakeDamage = false;
            _isDeath = true;
            GameInput.Instance.DisabledMovement();

            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        }
    }

    private IEnumerator DamageRecoveryRoutine() //  орутина
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        _canTakeDamage = true;
    }

    public bool IsRunning()
    {
        return _isRunning;
    }

    public bool IsTurnLeft()
    {
        return _isTurnLeft;
    }

    private void HandleMovement() //ќбновл€ет позицию игрока
    {

        _rigidBody2D.MovePosition(_rigidBody2D.position + inputVector * (_movingSpeed * Time.fixedDeltaTime));

        if (Mathf.Abs(inputVector.x) > _minMovingSpeed || Mathf.Abs(inputVector.y) > _minMovingSpeed)
        {
            _isRunning = true;
        }
        else
        {
            _isRunning = false;
        }

        if (inputVector.x > 0.1f)
        {
            _isTurnLeft = false;
        }
        else if (inputVector.x < -0.1f)
        {
            _isTurnLeft = true;
        }
    }

    private void GameInput_OnPlayerAttack(object sender, System.EventArgs e)
    {
        ActiveWeapon.Instance.GetActiveWeapon().Attack();
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnPlayerAttack -= GameInput_OnPlayerAttack;
    }
}
