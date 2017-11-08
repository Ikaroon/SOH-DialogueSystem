using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Northwind.Essentials
{
    [System.Serializable]
    public sealed class IntVector3
    {

        public int x;
        public int y;
        public int z;

        public IntVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static IntVector3 operator +(IntVector3 v1, IntVector3 v2)
        {
            return new IntVector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static IntVector3 operator -(IntVector3 v1, IntVector3 v2)
        {
            return new IntVector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }
    }
}