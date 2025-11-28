using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
  [SerializeField] private float lifeTime = 5f;
  private Rigidbody2D _rb;

  public static System.Collections.Generic.List<Projectile> ActiveProjectiles = new System.Collections.Generic.List<Projectile>();

  private void Awake()
  {
    _rb = GetComponent<Rigidbody2D>();
    ActiveProjectiles.Add(this);
  }

  private void OnDestroy()
  {
      if (ActiveProjectiles.Contains(this))
      {
          ActiveProjectiles.Remove(this);
      }
  }

  private void Start()
  {
    Destroy(gameObject, lifeTime);
  }

  private void Update()
  {
    if (_rb.linearVelocity == Vector2.zero) return;
    var angle = Mathf.Atan2(_rb.linearVelocity.y, _rb.linearVelocity.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
  }

  private void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.gameObject.GetComponent<Projectile>() != null)
    {
      return;
    }

    var cannon = collision.gameObject.GetComponentInParent<CannonController>();
    if (cannon != null)
    {
      cannon.TakeDamage(20f);
    }

    Destroy(gameObject);
  }
}