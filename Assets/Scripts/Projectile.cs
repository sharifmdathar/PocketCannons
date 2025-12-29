using UnityEngine;

public class Projectile : BaseProjectile
{
  public static System.Collections.Generic.List<Projectile> ActiveProjectiles = new System.Collections.Generic.List<Projectile>();

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