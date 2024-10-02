using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;


namespace Boids
{
    public sealed class Boid : MonoBehaviour
    {  
        public LayerMask Layers;
        public float Speed = 2f;
        public float Radius = 1f;
        public GameObject Sprite;
        private Vector2 direction;


        void Start()
        {
            // Start at center of map.
            transform.position = Vector2.zero;

            // Start with random direction.
            direction = Random.insideUnitCircle.normalized;
        }

        void FixedUpdate()
        {
            // Move boid towards current direction.
            transform.Translate(direction * Speed * Time.deltaTime);

            // Berechne den Winkel basierend auf der Bewegungsrichtung
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Setze die Rotation des GameObjects entsprechend dem Winkel
            Sprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle -90));

            // Check if the boid is in bounds.
            CheckBounds();
        }

        void CheckBounds()
        {
            // Draw debug ray.
            Debug.DrawRay(transform.position, direction * Radius, Color.red);

            // Cast ray.
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Radius, Layers);

            if (hit.collider != null)
            {
                // Calculate reflection direction with a slight random deviation.
                Vector2 reflectedDirection = Vector2.Reflect(direction, hit.normal);

                // Add a small random angle to the reflected direction (random between -30 and 30 degrees)
                float randomAngle = Random.Range(-30f, 30f);
                Quaternion rotation = Quaternion.Euler(0, 0, randomAngle);
                direction = rotation * reflectedDirection;
            }
        }

    }
}
