using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Northwind.Essentials
{
    [System.Serializable]
    public class SwizzleVector3
    {

        public float x;
        public float y;
        public float z;

        public SwizzleContent this[string val]
        {
            get
            {
                SwizzleContent result = new SwizzleContent(val.Length);
                Iterator.Iterate(val.Length, delegate (int x)
                {
                    switch (val[x])
                    {
                        case 'x': result[x] = this.x; break;
                        case 'y': result[x] = this.y; break;
                        case 'z': result[x] = this.z; break;
                        case 'w': result[x] = 0f; break;
                    }
                });
                return result;
            }
            set
            {
                float rX = 0f;
                float rY = 0f;
                float rZ = 0f;
                Iterator.Iterate(3, delegate (int x)
                {
                    if (val.Length > x)
                    {
                        switch (val[x])
                        {
                            case 'x': rX = value[x]; break;
                            case 'y': rY = value[x]; break;
                            case 'z': rZ = value[x]; break;
                        }
                    }
                });
                x = rX;
                y = rY;
                z = rZ;
            }
        }

        public SwizzleVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static SwizzleVector3 operator +(SwizzleVector3 v1, SwizzleVector3 v2)
        {
            return new SwizzleVector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static SwizzleVector3 operator -(SwizzleVector3 v1, SwizzleVector3 v2)
        {
            return new SwizzleVector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static implicit operator Vector2(SwizzleVector3 c)
        {
            return new Vector2(c.x, c.y);
        }

        public static implicit operator Vector3(SwizzleVector3 c)
        {
            return new Vector3(c.x, c.y, c.z);
        }

        public static implicit operator Vector4(SwizzleVector3 c)
        {
            return new Vector4(c.x, c.y, c.z, 0f);
        }

        public static implicit operator SwizzleVector2(SwizzleVector3 c)
        {
            return new SwizzleVector2(c.x, c.y);
        }

        public static implicit operator SwizzleVector4(SwizzleVector3 c)
        {
            return new SwizzleVector4(c.x, c.y, c.z, 0f);
        }

        public static implicit operator string(SwizzleVector3 c)
        {
            return "(" + c.x + "|" + c.y + "|" + c.z + ")";
        }
    }
}