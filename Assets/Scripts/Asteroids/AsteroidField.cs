using System.Collections.Generic;
using UnityEngine;

public enum FieldLayerShape {
    Scatter, // uniform in the whole arena
    Ring, // flat belt around the center
    Clusters, // dense clumps placed around the arena
    Core, // a few big rocks near the center, used as cover
}

[System.Serializable]
public class FieldLayer {
    public string name = "Layer";
    public FieldLayerShape shape = FieldLayerShape.Scatter;
    public int count = 100;
    public Vector2 scaleRange = new Vector2 (400f, 500f);

    [Tooltip ("Ring: belt radius. Clusters: radius of each clump. Core: max distance from center.")]
    public float radius = 500f;
    [Tooltip ("Ring: belt thickness (radial and vertical).")]
    public float thickness = 150f;
    [Tooltip ("Clusters: how many clumps.")]
    public int clusterCount = 6;
    [Tooltip ("Whether this layer respects the empty space around the center.")]
    public bool keepCenterClear = true;
}

/// <summary>
/// Builds the arena out of several layers (belt, clusters, scatter, central cover)
/// instead of a uniform box, leaving open space in the middle and around each ship.
/// </summary>
public class AsteroidField : MonoBehaviour {
    public Transform prefab;

    [Tooltip ("Half size of the playable cube. Asteroids are kept inside it.")]
    public float arenaHalfSize = 950f;

    [Tooltip ("Radius of the open dogfight area in the middle.")]
    public float centerClearRadius = 220f;

    [Tooltip ("No asteroid spawns closer than this to a ship's starting position.")]
    public float shipClearRadius = 160f;

    [Tooltip ("0 = different map every match.")]
    public int seed = 0;

    public float velocity = 1f;
    public float angularVelocity = 1f;
    public bool scaleMass = true;

    public List<FieldLayer> layers = new List<FieldLayer> ();

    private const int MaxAttempts = 20;

    private readonly List<Vector3> shipPositions = new List<Vector3> ();

    void Start () {
        if (prefab == null)
            return;

        if (seed != 0)
            Random.InitState (seed);

        foreach (FLFlight.Ship ship in FindObjectsOfType<FLFlight.Ship> ())
            shipPositions.Add (ship.transform.position);

        foreach (FieldLayer layer in layers)
            SpawnLayer (layer);
    }

    private void SpawnLayer (FieldLayer layer) {
        Vector3[] clusterCenters = null;
        if (layer.shape == FieldLayerShape.Clusters) {
            clusterCenters = new Vector3[Mathf.Max (1, layer.clusterCount)];
            for (int i = 0; i < clusterCenters.Length; i++)
                clusterCenters[i] = PickClusterCenter (layer);
        }

        for (int i = 0; i < layer.count; i++) {
            for (int attempt = 0; attempt < MaxAttempts; attempt++) {
                Vector3 pos = SamplePosition (layer, clusterCenters);
                if (IsValid (pos, layer)) {
                    CreateAsteroid (transform.position + pos, layer.scaleRange);
                    break;
                }
            }
        }
    }

    private Vector3 SamplePosition (FieldLayer layer, Vector3[] clusterCenters) {
        switch (layer.shape) {
            case FieldLayerShape.Ring:
                float angle = Random.Range (0f, Mathf.PI * 2f);
                float r = layer.radius + Random.Range (-0.5f, 0.5f) * layer.thickness;
                return new Vector3 (Mathf.Cos (angle) * r, Random.Range (-0.5f, 0.5f) * layer.thickness, Mathf.Sin (angle) * r);
            case FieldLayerShape.Clusters:
                Vector3 center = clusterCenters[Random.Range (0, clusterCenters.Length)];
                // The extra Random.value packs the clump tighter in its middle.
                return center + Random.insideUnitSphere * layer.radius * Random.value;
            case FieldLayerShape.Core:
                return Random.insideUnitSphere * layer.radius;
            default:
                return new Vector3 (
                    Random.Range (-arenaHalfSize, arenaHalfSize),
                    Random.Range (-arenaHalfSize, arenaHalfSize),
                    Random.Range (-arenaHalfSize, arenaHalfSize));
        }
    }

    private Vector3 PickClusterCenter (FieldLayer layer) {
        float limit = arenaHalfSize - layer.radius;
        Vector3 best = Vector3.zero;
        for (int attempt = 0; attempt < MaxAttempts; attempt++) {
            best = new Vector3 (Random.Range (-limit, limit), Random.Range (-limit, limit), Random.Range (-limit, limit));
            if (best.magnitude > centerClearRadius + layer.radius && FarFromShips (transform.position + best, shipClearRadius + layer.radius))
                break;
        }
        return best;
    }

    private bool IsValid (Vector3 localPos, FieldLayer layer) {
        if (Mathf.Abs (localPos.x) > arenaHalfSize || Mathf.Abs (localPos.y) > arenaHalfSize || Mathf.Abs (localPos.z) > arenaHalfSize)
            return false;
        if (layer.keepCenterClear && localPos.magnitude < centerClearRadius)
            return false;
        return FarFromShips (transform.position + localPos, shipClearRadius);
    }

    private bool FarFromShips (Vector3 worldPos, float minDistance) {
        foreach (Vector3 shipPos in shipPositions) {
            if ((worldPos - shipPos).sqrMagnitude < minDistance * minDistance)
                return false;
        }
        return true;
    }

    private void CreateAsteroid (Vector3 position, Vector2 scaleRange) {
        Transform t = Instantiate (prefab, position, Random.rotation, transform);

        float scale = Random.Range (scaleRange.x, scaleRange.y);
        t.localScale = Vector3.one * scale;

        Rigidbody r = t.GetComponent<Rigidbody> ();
        if (r) {
            if (scaleMass)
                r.mass *= scale * scale * scale;

            r.AddRelativeForce (Random.insideUnitSphere * velocity, ForceMode.VelocityChange);
            r.AddRelativeTorque (Random.insideUnitSphere * angularVelocity * Mathf.Deg2Rad, ForceMode.VelocityChange);
        }
    }
}
