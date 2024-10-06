using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Boids
{
    public sealed class QBoid : MonoBehaviour
    {
        #region Settings

            [Header("Boid Movement")]
            public float MoveSpeed = 1.5f;

            [Header("Boid Detection")]
            public float VisionAngle = 180.0f;
            public float DetectionRadius = 5.0f;
            public float SeparationRadius = 2.0f;
            public float AlignmentRadius = 4.0f;
            public float CohesionRadius = 4.0f;


            [Header("Fine Tuning")]
            public bool UseSeparation = true;
            public float SeparationStrength = 1.0f;
            public bool UseAlignment = true;
            public float AlignmentStrength = 1.0f;
            public bool UseCohesion = true;
            public float CohesionStrength = 1.0f;


            [Header("References")]
            public GameObject Sprite;

            
            // Properties.
            public Vector3 Direction { get; private set; }
            public List<QBoid> Boids { get; private set; } = new List<QBoid>();

            private bool _bounds = true;



        #endregion


        #region Unity Events

            void Start()
            {
                // Start with a random movement direction.
                Direction = Random.insideUnitCircle.normalized;
            }

            void FixedUpdate()
            {
                PerceiveBoids();

                MoveBoid();
            }

        #endregion


        #region Movement Functions
            
            /// <summary>
            /// Move the boid in a specific direction, influenced by
            /// multiple movement rules.
            /// </summary>
            void MoveBoid()
            {
                if (_bounds)
                {
                    if (UseSeparation)
                    {
                        // Add separation
                        Separation(SeparationStrength);
                    }

                    if (UseAlignment)
                    {
                        // Add Alignment
                        Alignment(AlignmentStrength);
                    }

                    if (UseCohesion)
                    {
                        // Add Cohesion
                        Cohesion(CohesionStrength);
                    }
                }

                // Check if the boid is still in allowed bounds (Last override before apply).
                CheckBounds();

                // Move boid towards direction
                transform.Translate(Direction * MoveSpeed * Time.deltaTime);

                // Rotate sprite towards direction
                RotateBoidToDirection();
            }
            
            /// <summary>
            /// Rotate the sprite object into the movement direction.
            /// </summary>
            void RotateBoidToDirection()
            {
                // Calculate the angle from direction using Mathf library.
                float angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;

                // Rotate the sprite object towards the direction using calculated angle.
                Sprite.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            }

            /// <summary>
            /// Checks if the boid is outside of bounds and overrides the direction
            /// accordingly.
            /// </summary>
            void CheckBounds()
            {
                // Get bounds from spawner.
                Vector2 bounds = BoidSpawner.Bounds;    

                // Check if the boid is outside of the allowed bounds.
                if (transform.position.x < -((bounds.x / 2) - 4) || transform.position.x > (bounds.x / 2) - 4 ||
                    transform.position.y < -((bounds.y / 2) - 4) || transform.position.y > (bounds.y / 2) - 4)
                {
                    _bounds = false;

                    // 1. Get the direction to the center of the scene (0, 0, 0)
                    Vector3 centerDir = (Vector3.zero - transform.position).normalized;

                    // 2. Calculate a random angle for direction randomization.
                    float rndAngle = Random.Range(-45, 45);
                    Quaternion rotation = Quaternion.Euler(0, 0, rndAngle);

                    // 3. New direction to center using random rotation values.
                    Vector3 dir = rotation * centerDir;

                    // 4. Set final rotation using lerp (smooth movement).
                    Direction = Vector3.Lerp(Direction, dir, 0.1f).normalized;
                }
                else
                {
                    _bounds = true;
                }
            }


        #endregion


        #region Rules


            /// <summary>
            /// Takes a list of known boids and applies negative force to direction of this boid
            /// in order to move away from those other boids.
            /// </summary>
            private void Separation(float strenght)
            {
                // Placeholder for the target direction.
                Vector3 direction = Vector3.zero;

                // Loop all known boids.
                foreach (QBoid boid in Boids)
                {
                    if (boid)
                    {
                        // Get distance from this boid to target boid.
                        float dist = (transform.position - boid.transform.position).magnitude;

                        if (dist <= SeparationRadius)
                        {
                            // Calculate the strenght of the separation based on how close the boid is to the other boids.
                            float rate = Mathf.Clamp01((boid.transform.position - transform.position).magnitude / SeparationRadius);
                            // Apply negative direction (Away from other boid) to target direction based on rate / strength.
                            direction -= rate * (boid.transform.position - transform.position) * strenght;
                        }
                    }
                }

                // Apply the final direction (Either null or the combined "away" direction from all known boids).
                Direction = Vector3.Lerp(Direction, (Direction + direction).normalized, 0.1f);
            }

            /// <summary>
            /// Gathers the average alignment direction from all boids in alignment radius and
            /// and applies it to the movement direction of a single boid.
            /// </summary>
            private void Alignment(float strenght)
            {
                Vector3 alignmentDirection = Vector3.zero;
                int num = 0;

                foreach (QBoid boid in Boids)
                {
                    if (boid)
                    {
                        // Get distance from this boid to target boid.
                        float dist = (transform.position - boid.transform.position).magnitude;

                        // Check if current boid is in alignment radius.
                        if (dist <= AlignmentRadius)
                        {
                            // Add other boids direction to total alignment direction.
                            alignmentDirection += boid.Direction;

                            // Increase number of added directions.
                            num++;
                        }
                    }
                }

                // Avoid 0 divide.
                if (num > 0)
                {
                    // Get the average direction.
                    alignmentDirection /= num;

                    // Apply strength to alignment direction.
                    alignmentDirection *= strenght;

                    // Apply the alignment direction to the general movement direction.
                    // Direction = (Direction + alignmentDirection).normalized;
                    Direction = Vector3.Lerp(Direction, (Direction + alignmentDirection).normalized, .1f);
                }
            }

            /// <summary>
            /// Gathers the central point of all boids infront of this boid that
            /// are in cohesion radius. Manipulates the boid to move towards center point.
            /// </summary>
            private void Cohesion(float strength)
            {
                Vector3 center = Vector3.zero;
                int num = 0;

                foreach (QBoid boid in Boids)
                {
                    if (boid)
                    {
                        // Get distance from this boid to target boid.
                        float dist = (transform.position - boid.transform.position).magnitude;

                        // Check if the boid is in cohesion radius.
                        if (dist <= CohesionRadius)
                        {
                            // Add to average center position.
                            center += boid.transform.position;

                            // Increase number of added positions.
                            num++;
                        }
                    }
                }

                // Avoid 0 divide.
                if (num > 0)
                {
                    // Calculate the average center position.
                    center /= num;

                    // Calculate the direction to the center position.
                    Vector3 dirToCenter = (center - transform.position).normalized;

                    // Apply strength to direction.
                    dirToCenter *= strength;

                    // Add the direction to the general movement direction.
                    // Direction = (Direction + dirToCenter).normalized;
                    Direction = Vector3.Lerp(Direction, (Direction + dirToCenter).normalized, .1f);
                }
            }


        #endregion


        #region Perception Functions

            /// <summary>
            /// Called to get all boids that are in range & perceived by the boid (using vision).
            /// </summary>
            private void PerceiveBoids()
            {
                // Make sure boids from last check are removed.
                Boids.Clear();

                // Loop all boids stored in boid spawner.
                foreach (QBoid boid in BoidSpawner.Boids)
                {
                    // Make sure the boid is valid and not this boid.
                    if (boid != null && boid != this)
                    {
                        // Check if the boid is in the general detection range and vision.
                        if ((boid.transform.position - transform.position).magnitude <= DetectionRadius && InVision(boid))
                        {
                            // Add boid to register.
                            Boids.Add(boid);
                        }
                    }
                }
            }

            /// <summary>
            /// Used to check if a boid is inside the vision cone.
            /// </summary>
            /// <param name="boid"></param>
            /// <returns></returns>
            private bool InVision(QBoid boid)
            {
                if (boid)
                {
                    // 1. Get the direction to the other boid.
                    Vector3 boidDir = (boid.transform.position - transform.position).normalized;

                    // 2. Get angle between this boids direction & direction to other boid.
                    float angleTo = Vector3.Dot(Direction.normalized, boidDir);

                    // 3. Check if the other boid is inside of the vision cone.
                    float visTresh = Mathf.Cos(VisionAngle * 0.5f * Mathf.Deg2Rad);
                    return angleTo >= visTresh;
                }

                return false;
            }

        #endregion


        #region Debugging

            void OnDrawGizmosSelected()
            {
                // Draw General detection radius.
                DrawWireArc(transform.position, Direction, VisionAngle, DetectionRadius, Color.white);

                // Draw separation detection radius.
                DrawWireArc(transform.position, Direction, VisionAngle, SeparationRadius, Color.magenta);

                // Draw alignment detection radius.
                DrawWireArc(transform.position, Direction, VisionAngle, AlignmentRadius, Color.cyan);

                // Draw cohesion detection radius.
                DrawWireArc(transform.position, Direction, VisionAngle, CohesionRadius, Color.yellow);

                // Draw sensed boids.
                DrawSensed();
            }

            /// <summary>
            /// Source: https://gist.github.com/luisparravicini/50d044a20c67f0615fdd28accd939df4
            /// </summary>
            /// <param name="position"></param>
            /// <param name="dir"></param>
            /// <param name="anglesRange"></param>
            /// <param name="radius"></param>
            /// <param name="maxSteps"></param>
            public static void DrawWireArc(Vector3 position, Vector3 dir, float anglesRange, float radius, Color color, float maxSteps = 20)
            {

                Gizmos.color = color;

                var srcAngles = GetAnglesFromDir(position, dir);
                var initialPos = position;
                var posA = initialPos;
                var stepAngles = anglesRange / maxSteps;
                var angle = srcAngles - anglesRange / 2;
                for (var i = 0; i <= maxSteps; i++)
                {
                    var rad = Mathf.Deg2Rad * angle;
                    var posB = initialPos;

                    // Use Y-Axis because 2D
                    posB += new Vector3(radius * Mathf.Cos(rad), radius * Mathf.Sin(rad), 0);

                    Gizmos.DrawLine(posA, posB);

                    angle += stepAngles;
                    posA = posB;
                }
                Gizmos.DrawLine(posA, initialPos);
            }

            /// <summary>
            /// https://gist.github.com/luisparravicini/50d044a20c67f0615fdd28accd939df4
            /// </summary>
            /// <param name="position"></param>
            /// <param name="dir"></param>
            /// <returns></returns>
            static float GetAnglesFromDir(Vector3 position, Vector3 dir)
            {
                var forwardLimitPos = position + dir;

                // Use y instead of z
                var srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.y - position.y, forwardLimitPos.x - position.x);

                return srcAngles;
            }

            /// <summary>
            /// Draw line to perceived boids.
            /// </summary>
            void DrawSensed()
            {
                Gizmos.color = Color.green;

                foreach (QBoid obj in Boids)
                {
                    Gizmos.DrawLine(transform.position, obj.transform.position);
                }
            }

        #endregion
    }
}

