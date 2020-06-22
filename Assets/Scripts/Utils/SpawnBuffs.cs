using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBuffs : MonoBehaviour
{

    [Tooltip("Prefab to spawn.")]
    public Transform prefab;

    [Tooltip("Multiplier for the spawn shape in each axis.")]
    public Vector3 shapeModifiers = Vector3.one;

    [Tooltip("How many prefab to spawn.")]
    public int asteroidCount = 50;

    [Tooltip("Distance from the center of the gameobject that prefabs will spawn")]
    public float range = 1000.0f;

    [Tooltip("Random min/max scale to apply.")]
    public Vector2 scaleRange = new Vector2(1.0f, 3.0f);


    void Start()
    {
        if (prefab != null)
        {
            for (int i = 0; i < asteroidCount; i++)
                CreateAsteroid();
        }
    }

private void CreateAsteroid()
    {
        Vector3 spawnPos = Vector3.zero;
         
        // Create random position based on specified shape and range.
            spawnPos.x = Random.Range(-range, range) * shapeModifiers.x;
            spawnPos.y = Random.Range(-range, range) * shapeModifiers.y;
            spawnPos.z = Random.Range(-range, range) * shapeModifiers.z;

        // Offset position to match position of the parent gameobject.
        spawnPos += transform.position;

        Quaternion spawnRot = (true) ? Random.rotation : Quaternion.identity;

        // Create the object and set the parent to this gameobject for scene organization.
        Transform t = Instantiate(prefab, spawnPos, spawnRot) as Transform;
        t.SetParent(transform);

        // Apply scaling.
        float scale = Random.Range(scaleRange.x, scaleRange.y);
        t.localScale = Vector3.one * scale;

        // Apply rigidbody values.
        Rigidbody r = t.GetComponent<Rigidbody>();
        if (r)
        {
            r.AddRelativeForce(Random.insideUnitSphere * 1.0f, ForceMode.VelocityChange);
            r.AddRelativeTorque(Random.insideUnitSphere * 1.0f * Mathf.Deg2Rad, ForceMode.VelocityChange);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
