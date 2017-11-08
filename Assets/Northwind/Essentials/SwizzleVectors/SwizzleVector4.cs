using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Northwind.Essentials
{
    [System.Serializable]
    public class SwizzleVector4
    {

        public float x;
        public float y;
        public float z;
        public float w;

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
                        case 'w': result[x] = this.w; break;
                    }
                });
                return result;
            }
            set
            {
                float rX = 0f;
                float rY = 0f;
                float rZ = 0f;
                float rW = 0f;
                Iterator.Iterate(4, delegate (int x)
                {
                    if (val.Length > x)
                    {
                        switch (val[x])
                        {
                            case 'x': rX = value[x]; break;
                            case 'y': rY = value[x]; break;
                            case 'z': rZ = value[x]; break;
                            case 'w': rW = value[x]; break;
                        }
                    }
                });
                x = rX;
                y = rY;
                z = rZ;
                w = rW;
            }
        }

        public SwizzleVector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static SwizzleVector4 operator +(SwizzleVector4 v1, SwizzleVector4 v2)
        {
            return new SwizzleVector4(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, v1.w + v2.w);
        }

        public static SwizzleVector4 operator -(SwizzleVector4 v1, SwizzleVector4 v2)
        {
            return new SwizzleVector4(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z, v1.w - v2.w);
        }

        public static implicit operator Vector2(SwizzleVector4 c)
        {
            return new Vector2(c.x, c.y);
        }

        public static implicit operator Vector3(SwizzleVector4 c)
        {
            return new Vector3(c.x, c.y, c.z);
        }

        public static implicit operator Vector4(SwizzleVector4 c)
        {
            return new Vector4(c.x, c.y, c.z, c.w);
        }

        public static implicit operator SwizzleVector2(SwizzleVector4 c)
        {
            return new SwizzleVector2(c.x, c.y);
        }

        public static implicit operator SwizzleVector3(SwizzleVector4 c)
        {
            return new SwizzleVector3(c.x, c.y, c.z);
        }

        public static implicit operator string(SwizzleVector4 c)
        {
            return "(" + c.x + "|" + c.y + "|" + c.z + "|" + c.w + ")";
        }
    }
}
