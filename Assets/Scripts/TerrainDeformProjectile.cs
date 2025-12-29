using UnityEngine;

public class TerrainDeformProjectile : BaseProjectile
{
    [SerializeField] private float deformationRadius = 3f;

    public static System.Collections.Generic.List<TerrainDeformProjectile> ActiveProjectiles = new System.Collections.Generic.List<TerrainDeformProjectile>();

    protected override void Awake()
    {
        base.Awake();
        ActiveProjectiles.Add(this);
    }

    private void OnDestroy()
    {
        if (ActiveProjectiles.Contains(this))
        {
            ActiveProjectiles.Remove(this);
        }
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

