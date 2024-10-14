using UnityEngine;


namespace Boids.Optimisation
{
    /// <summary>
    /// Interface that contains all function for 2D objects to be integrated
    /// with the quad-tree structure. Interface Treeable Object 2-Dimensional (ITO2D).
    /// </summary>
    public interface ITO2D
    {
        /// <summary
        /// Function that returns the 2D position of objects that implement the interface.
        /// </summary>
        /// <returns>2D position of object.</returns
        Vector2 GetPosition();
    }
}
