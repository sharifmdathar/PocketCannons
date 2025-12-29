using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TerrainDeformProjectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float deformationRadius = 3f;
    private Rigidbody2D _rb;

    public static System.Collections.Generic.List<TerrainDeformProjectile> ActiveProjectiles = new System.Collections.Generic.List<TerrainDeformProjectile>();

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
        if (collision.gameObject.GetComponent<Projectile>() != null ||
            collision.gameObject.GetComponent<TerrainDeformProjectile>() != null)
        {
            return;
        }

        var hitPoint = collision.contacts[0].point;

        if (TerrainGenerator.Instance != null)
        {
            TerrainGenerator.Instance.DeformTerrain(hitPoint, deformationRadius);
        }

        var cannon = collision.gameObject.GetComponentInParent<CannonController>();
        if (cannon != null)
        {
            cannon.TakeDamage(20f);
        }

        Destroy(gameObject);
    }
}

