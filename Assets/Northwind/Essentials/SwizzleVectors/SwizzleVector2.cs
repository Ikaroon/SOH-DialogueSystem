using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Northwind.Essentials
{
    [System.Serializable]
    public sealed class SwizzleVector2
    {

        public float x;
        public float y;

        public SwizzleContent this[string val]
        {
            get {
                SwizzleContent result = new SwizzleContent(val.Length);
                Iterator.Iterate(val.Length, delegate (int x)
                {
                    switch (val[x])
                    {
                        case 'x': result[x] = this.x; break;
                        case 'y': result[x] = this.y; break;
                        case 'z': result[x] = 0f; break;
                        case 'w': result[x] = 0f; break;
                    }
                });
                return result;
            }
            set
            {
                float rX = 0;
                float rY = 0;
                Iterator.Iterate(2, delegate (int x)
                {
                    if (val.Length > x)
                    {
                        switch (val[x])
                        {
                            case 'x': rX = value[x]; break;
                            case 'y': rY = value[x]; break;
                        }
                    }
                });
                x = rX;
                y = rY;
            }
        }

        public SwizzleVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        
        public static SwizzleVector2 operator +(SwizzleVector2 v1, SwizzleVector2 v2)
        {
            return new SwizzleVector2(v1.x + v2.x, v1.y + v2.y);
        }

        public static SwizzleVector2 operator -(SwizzleVector2 v1, SwizzleVector2 v2)
        {
            return new SwizzleVector2(v1.x - v2.x, v1.y - v2.y);
        }
        
        public static implicit operator Vector2(SwizzleVector2 c)
        {
            return new Vector2(c.x, c.y);
        }

        public static implicit operator Vector3(SwizzleVector2 c)
        {
            return new Vector3(c.x, c.y, 0f);
        }

        public static implicit operator Vector4(SwizzleVector2 c)
        {
            return new Vector4(c.x, c.y, 0f, 0f);
        }

        public static implicit operator SwizzleVector3(SwizzleVector2 c)
        {
            return new SwizzleVector3(c.x, c.y, 0f);
        }

        public static implicit operator SwizzleVector4(SwizzleVector2 c)
        {
            return new SwizzleVector4(c.x, c.y, 0f, 0f);
        }
        
        public static implicit operator string(SwizzleVector2 c)
        {
            return "(" + c.x + "|" + c.y + ")";
        }
    }
}