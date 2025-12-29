using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(PolygonCollider2D))]
public class TerrainGenerator : MonoBehaviour
{
    public static TerrainGenerator Instance { get; private set; }

    [Header("Settings")] [SerializeField] private int width = 50;
    [SerializeField] private float heightMultiplier = 5f;
    [SerializeField] private float noiseFrequency = 0.15f;
    [SerializeField] private float groundDepth = 15f;
    [SerializeField] private int resolution = 2;

    private float _seed;
    private Mesh _mesh;
    private PolygonCollider2D _collider;
    private float[] _originalHeights;
    private int _verticesCount;
    private float _startX;
    private float _step;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        _collider = GetComponent<PolygonCollider2D>();

        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        _seed = Random.Range(-10000f, 10000f);

        _verticesCount = width * resolution;
        var vertices = new Vector3[_verticesCount * 2];
        var triangles = new int[(_verticesCount - 1) * 6];
        var path = new Vector2[_verticesCount + 2];
        _originalHeights = new float[_verticesCount];

        _startX = -width / 2f;
        _step = 1f / resolution;
        for (var i = 0; i < _verticesCount; i++)
        {
            var x = _startX + i * _step;
            var y = GetHeight(x);
            _originalHeights[i] = y;
            vertices[i] = new Vector3(x, y, 0);
            vertices[i + _verticesCount] = new Vector3(x, y - groundDepth, 0);

            path[i] = new Vector2(x, y);
        }

        path[_verticesCount] = new Vector2(_startX + width - _step, -groundDepth);
        path[_verticesCount + 1] = new Vector2(_startX, -groundDepth);
        _collider.SetPath(0, path);

        for (var i = 0; i < _verticesCount - 1; i++)
        {
            var root = i * 6;
            var tl = i;
            var tr = i + 1;
            var bl = i + _verticesCount;
            var br = i + _verticesCount + 1;

            triangles[root] = tl;
            triangles[root + 1] = br;
            triangles[root + 2] = bl;
            triangles[root + 3] = tl;
            triangles[root + 4] = tr;
            triangles[root + 5] = br;
        }

        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();

        var uvs = new Vector2[vertices.Length];
        for (var i = 0; i < vertices.Length; i++) uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
        _mesh.uv = uvs;
    }

    public float GetHeight(float x)
    {
        if (_mesh == null || _mesh.vertices == null || _mesh.vertices.Length == 0)
        {
            return Mathf.PerlinNoise((x + _seed) * noiseFrequency, 0) * heightMultiplier;
        }

        var vertices = _mesh.vertices;
        var closestIndex = 0;
        var minDistance = float.MaxValue;

        for (var i = 0; i < _verticesCount; i++)
        {
            var vertexX = vertices[i].x;
            var distance = Mathf.Abs(vertexX - x);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        return vertices[closestIndex].y;
    }

    public float GetMinX()
    {
        return -width / 2f;
    }

    public float GetMaxX()
    {
        var step = 1f / resolution;
        return -width / 2f + width - step;
    }

    public bool IsWithinBounds(float x)
    {
        return x >= GetMinX() && x <= GetMaxX();
    }

    public void DeformTerrain(Vector2 center, float radius)
    {
        if (_mesh == null || _originalHeights == null) return;

        var vertices = _mesh.vertices;
        var path = new Vector2[_verticesCount + 2];
        var modified = false;

        for (var i = 0; i < _verticesCount; i++)
        {
            var x = _startX + i * _step;
            var distance = Vector2.Distance(new Vector2(x, _originalHeights[i]), center);

            if (distance <= radius)
            {
                var depth = radius - distance;
                var newHeight = _originalHeights[i] - depth;
                vertices[i] = new Vector3(x, newHeight, 0);
                vertices[i + _verticesCount] = new Vector3(x, newHeight - groundDepth, 0);
                path[i] = new Vector2(x, newHeight);
                modified = true;
            }
            else
            {
                vertices[i] = new Vector3(x, _originalHeights[i], 0);
                vertices[i + _verticesCount] = new Vector3(x, _originalHeights[i] - groundDepth, 0);
                path[i] = new Vector2(x, _originalHeights[i]);
            }
        }

        if (!modified) return;

        path[_verticesCount] = new Vector2(_startX + width - _step, -groundDepth);
        path[_verticesCount + 1] = new Vector2(_startX, -groundDepth);
        _collider.SetPath(0, path);

        _mesh.vertices = vertices;
        _mesh.RecalculateNormals();

        var uvs = new Vector2[vertices.Length];
        for (var i = 0; i < vertices.Length; i++) uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
        _mesh.uv = uvs;

        var cannons = FindObjectsByType<CannonController>(FindObjectsSortMode.None);
        foreach (var cannon in cannons)
        {
            cannon.SnapToGround();
        }
    }
}
