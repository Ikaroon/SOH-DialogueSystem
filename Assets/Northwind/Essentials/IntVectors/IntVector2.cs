using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Northwind.Essentials
{
    [System.Serializable]
    public sealed class IntVector2
    {
        /// <summary>The first dimension</summary>
        public int x;
        /// <summary>The second dimension</summary>
        public int y;

        /// <summary>
        /// Creates an IntVector2
        /// </summary>
        /// <param name="x">the first dimension</param>
        /// <param name="y">the second dimension</param>
        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static IntVector2 operator +(IntVector2 v1, IntVector2 v2)
        {
            return new IntVector2(v1.x + v2.x, v1.y + v2.y);
        }

        public static IntVector2 operator -(IntVector2 v1, IntVector2 v2)
        {
            return new IntVector2(v1.x - v2.x, v1.y - v2.y);
        }
    }
}