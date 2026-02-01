using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockBack : MonoBehaviour
{
    [SerializeField] private float knockBackForce = 3f;
    [SerializeField] private float knockBackMovingTimerMax = 0.3f;

    private float _knockBackMovingTimer;

    private Rigidbody2D _rigidbody2D;

    public bool IsGettingKnockedBack { get; private set; }

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _knockBackMovingTimer -= Time.deltaTime;
        if ( _knockBackMovingTimer < 0)
        {
            StopKnockBackMovement();
        }
    }

    public void GetKnockedBack(Transform damageSource)
    {
        IsGettingKnockedBack = true;
        _knockBackMovingTimer = knockBackMovingTimerMax;
        Vector2 difference = (transform.position - damageSource.position).normalized * knockBackForce / _rigidbody2D.mass;
        _rigidbody2D.AddForce(difference, ForceMode2D.Impulse);
    }

    public void StopKnockBackMovement()
    {
        IsGettingKnockedBack = false;
        _rigidbody2D.velocity = Vector2.zero;
    }
}
