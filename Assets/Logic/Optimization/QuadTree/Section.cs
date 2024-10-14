using System.Collections.Generic;
using UnityEngine;


namespace Boids.Optimisation
{
    public class Section
    {
        /// <summary>
        /// Holds the boundaries of this section in the quad-tree.
        /// </summary>
        public Rect Bounds { get; private set; }

        /// <summary>
        /// Holds the layer of the quad-tree this section is placed in.
        /// (0 -> 1 -> 2 ...).
        /// </summary>
        public int Layer { get; private set; } = -1;

        /// <summary>
        /// Defines how many items can be stored in this section.
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// Holds the sections of this tree-section. Limited to a maximum of
        /// four (4) sections.
        /// </summary>
        public Section[] Sections { get; private set; } = new Section[4];

        /// <summary>
        /// Holds the items stored in this section
        /// (Hash Set instead of list for quicker operations).
        /// </summary>
        public HashSet<ITO2D> Items { get; private set; }


        // Constructor used to create a new section.
        public Section(Rect bounds, int layer, int capacity)
        {
            // Set values. 
            this.Bounds = bounds;
            this.Layer = layer;
            this.Capacity = capacity;
        }
    }
}


