using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Northwind.Essentials
{
    public sealed class SwizzleContent
    {

        private float[] floats;

        public SwizzleContent(int length)
        {
            floats = new float[length];
        }

        public float this[int index]
        {
            get
            {
                return floats[index];
            }
            set
            {
                floats[index] = value;
            }
        }

        public int Length
        {
            get
            {
                return floats.Length;
            }
        }

        public static explicit operator Vector2(SwizzleContent c)
        {
            return new Vector2(c.Length > 0 ? c[0] : 0f, c.Length > 1 ? c[1] : 0f);
        }

        public static explicit operator Vector3(SwizzleContent c)
        {
            return new Vector3(c.Length > 0 ? c[0] : 0f, c.Length > 1 ? c[1] : 0f, c.Length > 2 ? c[2] : 0f);
        }

        public static explicit operator Vector4(SwizzleContent c)
        {
            return new Vector4(c.Length > 0 ? c[0] : 0f, c.Length > 1 ? c[1] : 0f, c.Length > 2 ? c[2] : 0f, c.Length > 3 ? c[3] : 0f);
        }

        public static explicit operator SwizzleVector2(SwizzleContent c)
        {
            return new SwizzleVector2(c.Length > 0 ? c[0] : 0f, c.Length > 1 ? c[1] : 0f);
        }

        public static explicit operator SwizzleVector3(SwizzleContent c)
        {
            return new SwizzleVector3(c.Length > 0 ? c[0] : 0f, c.Length > 1 ? c[1] : 0f, c.Length > 2 ? c[2] : 0f);
        }

        public static explicit operator SwizzleVector4(SwizzleContent c)
        {
            return new SwizzleVector4(c.Length > 0 ? c[0] : 0f, c.Length > 1 ? c[1] : 0f, c.Length > 2 ? c[2] : 0f, c.Length > 3 ? c[3] : 0f);
        }
    }
}