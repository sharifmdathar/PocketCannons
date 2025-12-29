using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class BaseProjectile : MonoBehaviour
{
    [SerializeField] protected float lifeTime = 5f;
    [SerializeField] protected float windInfluence = 0.5f;
    protected Rigidbody2D _rb;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    protected virtual void FixedUpdate()
    {
        if (GameManager.Instance != null)
        {
            var windForce = new Vector2(GameManager.Instance.WindStrength * windInfluence, 0f);
            _rb.AddForce(windForce, ForceMode2D.Force);
        }
    }

    protected virtual void Update()
    {
        if (_rb.linearVelocity == Vector2.zero) return;
        var angle = Mathf.Atan2(_rb.linearVelocity.y, _rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}

