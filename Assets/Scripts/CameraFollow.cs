using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Cannon References")] [SerializeField]
    private CannonController cannon1;

    [SerializeField] private CannonController cannon2;

    [Header("Camera Settings")] [SerializeField]
    private float minOrthographicSize = 10f;

    [SerializeField] private float maxOrthographicSize = 30f;
    [SerializeField] private float sizePadding = 2f;
    [SerializeField] private float positionSmoothing = 5f;
    [SerializeField] private float sizeSmoothing = 5f;

    [Header("Camera Position")] [SerializeField]
    private float cameraZOffset = -10f;

    [SerializeField] private float cameraYOffset = 0f;

    private Camera _camera;
    private float _targetOrthographicSize;
    private Vector3 _targetPosition;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if (_camera == null)
        {
            Debug.LogError("CameraFollow: No Camera component found!");
            enabled = false;
            return;
        }

        if (cannon1 == null || cannon2 == null)
        {
            FindCannons();
        }
    }

    private void Start()
    {
        if (cannon1 == null || cannon2 == null)
        {
            Debug.LogWarning("CameraFollow: One or both cannons not found! Camera will not follow.");
            enabled = false;
            return;
        }

        UpdateCameraTargets();
        _camera.orthographicSize = _targetOrthographicSize;
        transform.position = _targetPosition;
    }

    private void LateUpdate()
    {
        if (cannon1 == null || cannon2 == null) return;

        UpdateCameraTargets();

        transform.position = Vector3.Lerp(transform.position, _targetPosition, positionSmoothing * Time.deltaTime);

        _camera.orthographicSize =
            Mathf.Lerp(_camera.orthographicSize, _targetOrthographicSize, sizeSmoothing * Time.deltaTime);
    }

    private void UpdateCameraTargets()
    {
        var minX = Mathf.Min(cannon1.transform.position.x, cannon2.transform.position.x);
        var maxX = Mathf.Max(cannon1.transform.position.x, cannon2.transform.position.x);
        var minY = Mathf.Min(cannon1.transform.position.y, cannon2.transform.position.y);
        var maxY = Mathf.Max(cannon1.transform.position.y, cannon2.transform.position.y);

        foreach (var projectile in Projectile.ActiveProjectiles)
        {
            if (projectile == null) continue;
            minX = Mathf.Min(minX, projectile.transform.position.x);
            maxX = Mathf.Max(maxX, projectile.transform.position.x);
            minY = Mathf.Min(minY, projectile.transform.position.y);
            maxY = Mathf.Max(maxY, projectile.transform.position.y);
        }

        foreach (var projectile in TerrainDeformProjectile.ActiveProjectiles)
        {
            if (projectile == null) continue;
            minX = Mathf.Min(minX, projectile.transform.position.x);
            maxX = Mathf.Max(maxX, projectile.transform.position.x);
            minY = Mathf.Min(minY, projectile.transform.position.y);
            maxY = Mathf.Max(maxY, projectile.transform.position.y);
        }

        var targetX = (minX + maxX) / 2f;
        var targetY = (minY + maxY) / 2f + cameraYOffset;

        var width = maxX - minX + sizePadding * 2f;
        var height = maxY - minY + sizePadding * 2f;

        var aspectRatio = (float)Screen.width / Screen.height;
        
        var sizeFromHeight = height / 2f;
        var sizeFromWidth = width / (aspectRatio * 2f);

        var calculatedSize = Mathf.Max(sizeFromHeight, sizeFromWidth, minOrthographicSize);

        if (TerrainGenerator.Instance != null)
        {
            var terrainMinX = TerrainGenerator.Instance.GetMinX();
            var terrainMaxX = TerrainGenerator.Instance.GetMaxX();
            var terrainWidth = terrainMaxX - terrainMinX;

            var maxSizeForTerrain = terrainWidth / (aspectRatio * 2f);

            calculatedSize = Mathf.Min(calculatedSize, maxSizeForTerrain);

            var cameraHalfWidth = calculatedSize * aspectRatio;
            targetX = Mathf.Clamp(targetX, terrainMinX + cameraHalfWidth, terrainMaxX - cameraHalfWidth);
        }

        _targetOrthographicSize = Mathf.Clamp(calculatedSize, minOrthographicSize, maxOrthographicSize);
        _targetPosition = new Vector3(targetX, targetY, cameraZOffset);
    }

    private void FindCannons()
    {
        var cannons = FindObjectsByType<CannonController>(FindObjectsSortMode.None);

        switch (cannons.Length)
        {
            case >= 2:
            {
                foreach (var cannon in cannons)
                {
                    if (cannon1 == null && cannon.Owner == GameManager.Turn.Player1)
                    {
                        cannon1 = cannon;
                    }
                    else if (cannon2 == null && cannon.Owner == GameManager.Turn.Player2)
                    {
                        cannon2 = cannon;
                    }
                }

                if (cannon1 == null) cannon1 = cannons[0];
                if (cannon2 == null && cannons.Length > 1) cannon2 = cannons[1];
                break;
            }
            case 1:
                Debug.LogWarning("CameraFollow: Only one cannon found. Need two cannons for camera follow.");
                break;
            default:
                Debug.LogWarning("CameraFollow: No cannons found in scene.");
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (cannon1 == null || cannon2 == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(cannon1.transform.position, cannon2.transform.position);

        var midpoint = (cannon1.transform.position + cannon2.transform.position) / 2f;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(midpoint, 0.5f);
    }
}

