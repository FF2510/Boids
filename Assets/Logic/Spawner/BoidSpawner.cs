using System.Collections;
using System.Collections.Generic;
using Boids;
using Unity.VisualScripting;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    public GameObject Boid;
    public int GridSize = 10;
    public static Vector2 Bounds = new Vector2(55, 30.9375f);
    public static List<QBoid> Boids { get; private set; } = new List<QBoid>();
    private LineRenderer _lineRenderer;

    private void Start()
    {
        // Create line renderer.
        _lineRenderer = gameObject.AddComponent<LineRenderer>();

        // Setup line renderer valuew.
        _lineRenderer.positionCount = 5;
        _lineRenderer.startWidth = 0.25f;
        _lineRenderer.endWidth = 0.25f;
        _lineRenderer.loop = true;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = Color.yellow;
        _lineRenderer.endColor = Color.yellow;
        
        // Draw bonds.
        DrawBounds();
        
        // Spawn boids.
        Spawn();
    }

    private void DrawBounds()
    {
        // Calculate corners based on bounds.
        Vector3 bottomLeft = transform.position + new Vector3(-Bounds.x / 2, -Bounds.y / 2, -0.1f); // Z etwas vor Hintergrund
        Vector3 bottomRight = transform.position + new Vector3(Bounds.x / 2, -Bounds.y / 2, -0.1f);
        Vector3 topRight = transform.position + new Vector3(Bounds.x / 2, Bounds.y / 2, -0.1f);
        Vector3 topLeft = transform.position + new Vector3(-Bounds.x / 2, Bounds.y / 2, -0.1f);

        // Set corners in line renderer.
        Vector3[] corners = new Vector3[] { bottomLeft, bottomRight, topRight, topLeft, bottomLeft };
        _lineRenderer.SetPositions(corners);
    }



    private void Spawn()
    {
        Vector3 pos = Vector3.zero;

        float offset = (GridSize - 1) / 2f;

        for (int i = 0; i < GridSize; i++)
        {
            for (int j = 0; j < GridSize; j++)
            {
                GameObject obj = Instantiate(Boid);
                Boids.Add(obj.GetComponent<QBoid>());
                pos = new Vector3(j - offset, offset - i, 0);
                obj.transform.position = pos;
            }
        }
    }
}
