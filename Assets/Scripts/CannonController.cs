using System;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [SerializeField] private GameManager.Turn owner;
    public GameManager.Turn Owner => owner;
    [SerializeField] private Transform barrelTransform;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject terrainDeformProjectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float maxForce = 20f;
    [SerializeField] private GameObject turnIndicator;
    [SerializeField] private float indicatorSpeed = 3f;
    [SerializeField] private float indicatorAmplitude = 0.3f;
    [SerializeField] private GameObject crosshairIndicator;
    [SerializeField] private float crosshairDistance = 2f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float groundOffset = 0.5f;
    [SerializeField] private float movementMargin = 2f;
    [SerializeField] private float tripleShotSpreadAngle = 15f;
    [SerializeField] private float tripleShotPositionOffset = 0.1f;

    private Vector3 _indicatorInitialPos;
    private bool _isFiring = false;

    private void Start()
    {
        if (turnIndicator != null)
        {
            _indicatorInitialPos = turnIndicator.transform.localPosition;
        }

        if (TerrainGenerator.Instance != null)
        {
            SnapToGround();
        }

        if (GameManager.Instance == null) return;

        UpdateTurnIndicator(GameManager.Instance.CurrentTurn);
        if (GameManager.Instance.CurrentTurn == owner)
        {
            UpdateRotation(GameManager.Instance.CurrentAngle);
        }

        GameManager.Instance.OnAngleChanged += UpdateRotation;
        GameManager.Instance.OnFire += Fire;
        GameManager.Instance.OnMove += Move;
        GameManager.Instance.OnTurnChanged += UpdateTurnIndicator;
        GameManager.Instance.OnHealthChanged += HandleHealthChanged;
    }

    private void Update()
    {
        if (turnIndicator == null || !turnIndicator.activeSelf) return;
        var yOffset = Mathf.Sin(Time.time * indicatorSpeed) * indicatorAmplitude;
        turnIndicator.transform.localPosition = _indicatorInitialPos + Vector3.up * yOffset;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnAngleChanged -= UpdateRotation;
        GameManager.Instance.OnFire -= Fire;
        GameManager.Instance.OnMove -= Move;
        GameManager.Instance.OnTurnChanged -= UpdateTurnIndicator;
        GameManager.Instance.OnHealthChanged -= HandleHealthChanged;
    }

    private void Move(float direction)
    {
        if (GameManager.Instance.CurrentTurn != owner) return;

        var moveAmount = direction * moveSpeed * Time.deltaTime;
        var targetPos = transform.position + Vector3.right * moveAmount;

        if (TerrainGenerator.Instance != null)
        {
            var minX = TerrainGenerator.Instance.GetMinX() + movementMargin;
            var maxX = TerrainGenerator.Instance.GetMaxX() - movementMargin;
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);

            var terrainHeight = TerrainGenerator.Instance.GetHeight(targetPos.x);
            targetPos.y = terrainHeight + groundOffset;

            var nextHeight = TerrainGenerator.Instance.GetHeight(targetPos.x + 0.1f);
            var angle = Mathf.Atan2(nextHeight - terrainHeight, 0.1f) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        transform.position = targetPos;
    }

    public void SnapToGround()
    {
        if (TerrainGenerator.Instance == null) return;

        var x = transform.position.x;
        var y = TerrainGenerator.Instance.GetHeight(x);
        transform.position = new Vector3(x, y + groundOffset, transform.position.z);

        const float sampleDelta = 0.1f;
        var nextHeight = TerrainGenerator.Instance.GetHeight(x + sampleDelta);
        var slopeAngle = Mathf.Atan2(nextHeight - y, sampleDelta) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, slopeAngle);
    }

    private void UpdateTurnIndicator(GameManager.Turn turn)
    {
        var isMyTurn = turn == owner;
        if (turnIndicator != null)
        {
            turnIndicator.SetActive(isMyTurn);
        }

        if (crosshairIndicator == null) return;
        crosshairIndicator.SetActive(isMyTurn);
        if (isMyTurn) UpdateCrosshair();
    }

    private void Fire()
    {
        if (GameManager.Instance.CurrentTurn != owner) return;
        if (projectilePrefab == null || firePoint == null) return;
        if (_isFiring) return;

        switch (GameManager.Instance.CurrentAttackType)
        {
            case GameManager.AttackType.SingleShot:
                FireSingleProjectile();
                break;
            case GameManager.AttackType.TripleShot:
                FireTripleShot();
                break;
            case GameManager.AttackType.TerrainDeform:
                FireTerrainDeform();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void FireSingleProjectile()
    {
        CreateAndLaunchProjectile(firePoint.position, firePoint.rotation);
    }

    private void FireTerrainDeform()
    {
        if (terrainDeformProjectilePrefab == null)
        {
            FireSingleProjectile();
            return;
        }

        CreateAndLaunchTerrainDeformProjectile(firePoint.position, firePoint.rotation);
    }

    private void FireTripleShot()
    {
        _isFiring = true;

        firePoint.GetPositionAndRotation(out var startPosition, out var baseRotation);
        var forceMagnitude = maxForce * (GameManager.Instance.CurrentPower / 100f);
        var spreadAngles = new[] { -tripleShotSpreadAngle, 0f, tripleShotSpreadAngle };

        for (var i = 0; i < 3; i++)
        {
            var shotRotation = baseRotation * Quaternion.Euler(0, 0, spreadAngles[i]);
            var shotDirection = shotRotation * Vector2.right;
            var perpendicular = new Vector2(-shotDirection.y, shotDirection.x);
            var shotPosition = startPosition + (Vector3)((i - 1) * tripleShotPositionOffset * perpendicular);

            CreateAndLaunchProjectile(shotPosition, shotRotation, shotDirection, forceMagnitude);
        }

        _isFiring = false;
    }

    private void CreateAndLaunchProjectile(Vector3 position, Quaternion rotation)
    {
        var powerPercentage = GameManager.Instance.CurrentPower / 100f;
        Vector2 force = firePoint.right * (maxForce * powerPercentage);
        CreateAndLaunchProjectile(position, rotation, firePoint.right, force.magnitude);
    }

    private void CreateAndLaunchProjectile(Vector3 position, Quaternion rotation, Vector2 direction, float forceMagnitude)
    {
        var projectile = Instantiate(projectilePrefab, position, rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb == null) return;
        var force = direction * forceMagnitude;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void CreateAndLaunchTerrainDeformProjectile(Vector3 position, Quaternion rotation)
    {
        var powerPercentage = GameManager.Instance.CurrentPower / 100f;
        Vector2 force = firePoint.right * (maxForce * powerPercentage);
        CreateAndLaunchTerrainDeformProjectile(position, rotation, firePoint.right, force.magnitude);
    }

    private void CreateAndLaunchTerrainDeformProjectile(Vector3 position, Quaternion rotation, Vector2 direction, float forceMagnitude)
    {
        var projectile = Instantiate(terrainDeformProjectilePrefab, position, rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb == null) return;
        var force = direction * forceMagnitude;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void UpdateRotation(int angle)
    {
        if (GameManager.Instance.CurrentTurn != owner) return;

        if (barrelTransform != null)
        {
            barrelTransform.rotation = Quaternion.Euler(0, 0, angle);
        }

        UpdateCrosshair();
    }

    private void UpdateCrosshair()
    {
        if (crosshairIndicator == null || firePoint == null) return;

        crosshairIndicator.transform.position = firePoint.position + firePoint.right * crosshairDistance;
        crosshairIndicator.transform.rotation = firePoint.rotation;
    }

    public void TakeDamage(float damage)
    {
        GameManager.Instance.TakeDamage(owner, damage);
    }

    private void HandleHealthChanged(GameManager.Turn turn, float health)
    {
        if (turn == owner && health <= 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        GameObject explosion = new GameObject("Explosion");
        explosion.transform.position = transform.position;

        ParticleSystem ps = explosion.AddComponent<ParticleSystem>();
        var renderer = explosion.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));

        var main = ps.main;
        main.startLifetime = 0.6f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 7f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.8f);
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.5f, 0f), Color.yellow);
        main.loop = false;
        main.playOnAwake = true;
        
        var emission = ps.emission;
        emission.enabled = true;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 30) });
        
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        
        var colLifetime = ps.colorOverLifetime;
        colLifetime.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.red, 0.6f), new GradientColorKey(Color.gray, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 0.6f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        colLifetime.color = grad;
        
        Destroy(explosion, 1.5f);

        var renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var r in renderers)
        {
            r.enabled = false;
        }

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (turnIndicator != null) turnIndicator.SetActive(false);
        if (crosshairIndicator != null) crosshairIndicator.SetActive(false);
        
        enabled = false;
    }
}
