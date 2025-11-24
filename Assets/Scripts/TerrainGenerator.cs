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

        var verticesCount = width * resolution;
        var vertices = new Vector3[verticesCount * 2];
        var triangles = new int[(verticesCount - 1) * 6];
        var path = new Vector2[verticesCount + 2];

        var startX = -width / 2f;
        var step = 1f / resolution;
        for (var i = 0; i < verticesCount; i++)
        {
            var x = startX + i * step;
            var y = GetHeight(x);
            vertices[i] = new Vector3(x, y, 0);
            vertices[i + verticesCount] = new Vector3(x, y - groundDepth, 0);

            path[i] = new Vector2(x, y);
        }

        path[verticesCount] = new Vector2(startX + width - step, -groundDepth);
        path[verticesCount + 1] = new Vector2(startX, -groundDepth);
        _collider.SetPath(0, path);

        for (var i = 0; i < verticesCount - 1; i++)
        {
            var root = i * 6;
            var tl = i;
            var tr = i + 1;
            var bl = i + verticesCount;
            var br = i + verticesCount + 1;

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
        return Mathf.PerlinNoise((x + _seed) * noiseFrequency, 0) * heightMultiplier;
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
}
