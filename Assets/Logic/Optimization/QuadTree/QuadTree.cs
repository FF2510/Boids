using System.Collections.Generic;
using UnityEngine;


namespace Boids.Optimisation
{
    public class QuadTree : MonoBehaviour
    {
        /// <summary>
        /// The first section created in the quad-tree.
        /// </summary>
        public Section Root { get; private set;}


        #region Functions

            /// <summary>
            /// Function to setup & create the quad-tree.
            /// </summary>
            /// <param name="bounds"></param>
            /// <param name="capacity"></param>
            public void Create(Rect bounds, int capacity)
            {
                // Do sum ...
            }

            /// <summary>
            /// Function to add any object implementing the ITO2D interface.
            /// </summary>
            /// <param name="input"></param>
            public void Add(ITO2D input)
            {
                // Do sum ...
            }
            
            /// <summary>
            /// Returns all objects in the same section as the instigator.
            /// </summary>
            /// <param name="instigator"></param>
            /// <returns></returns>
            public List<ITO2D> GetNeighbours(ITO2D instigator)
            {
                return new List<ITO2D>();
            }

        #endregion
    }
}