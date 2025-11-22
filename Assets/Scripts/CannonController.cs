using UnityEngine;
using Assets.Scripts;

public class CannonController : MonoBehaviour
{
    [SerializeField] private Transform barrelTransform;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float maxForce = 20f;

    private void Start()
    {
        if (GameManager.Instance == null) return;
        UpdateRotation(GameManager.Instance.CurrentAngle);
        GameManager.Instance.OnAngleChanged += UpdateRotation;
        GameManager.Instance.OnFire += Fire;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnAngleChanged -= UpdateRotation;
            GameManager.Instance.OnFire -= Fire;
        }
    }

    private void Fire()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            float powerPercentage = GameManager.Instance.CurrentPower / 100f;
            Vector2 force = firePoint.right * (maxForce * powerPercentage);
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    private void UpdateRotation(int angle)
    {
        if (barrelTransform != null)
        {
            barrelTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
