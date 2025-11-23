using UnityEngine;

public class CannonController : MonoBehaviour
{
    [SerializeField] private GameManager.Turn owner;
    [SerializeField] private Transform barrelTransform;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float maxForce = 20f;
    [SerializeField] private GameObject turnIndicator;
    [SerializeField] private float indicatorSpeed = 3f;
    [SerializeField] private float indicatorAmplitude = 0.3f;

    private Vector3 _indicatorInitialPos;

    private void Start()
    {
        if (turnIndicator != null)
        {
            _indicatorInitialPos = turnIndicator.transform.localPosition;
        }

        if (GameManager.Instance == null) return;

        UpdateTurnIndicator(GameManager.Instance.CurrentTurn);
        if (GameManager.Instance.CurrentTurn == owner)
        {
            UpdateRotation(GameManager.Instance.CurrentAngle);
        }

        GameManager.Instance.OnAngleChanged += UpdateRotation;
        GameManager.Instance.OnFire += Fire;
        GameManager.Instance.OnTurnChanged += UpdateTurnIndicator;
    }

    private void Update()
    {
        if (turnIndicator != null && turnIndicator.activeSelf)
        {
            float yOffset = Mathf.Sin(Time.time * indicatorSpeed) * indicatorAmplitude;
            turnIndicator.transform.localPosition = _indicatorInitialPos + Vector3.up * yOffset;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnAngleChanged -= UpdateRotation;
        GameManager.Instance.OnFire -= Fire;
        GameManager.Instance.OnTurnChanged -= UpdateTurnIndicator;
    }

    private void UpdateTurnIndicator(GameManager.Turn turn)
    {
        if (turnIndicator != null)
        {
            turnIndicator.SetActive(turn == owner);
        }
    }

    private void Fire()
    {
        if (GameManager.Instance.CurrentTurn != owner) return;
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
        if (GameManager.Instance.CurrentTurn != owner) return;

        if (barrelTransform != null)
        {
            barrelTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
