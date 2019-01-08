using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Northwind.Essentials
{
    public static class VectorMath
    {
        public static Vector2 Round(Vector2 vector)
        {
            vector.x = Mathf.Round(vector.x);
            vector.y = Mathf.Round(vector.y);
            return vector;
        }

        public static Vector3 Round(Vector3 vector)
        {
            vector.x = Mathf.Round(vector.x);
            vector.y = Mathf.Round(vector.y);
            vector.z = Mathf.Round(vector.z);
            return vector;
        }

        public static Vector4 Round(Vector4 vector)
        {
            vector.x = Mathf.Round(vector.x);
            vector.y = Mathf.Round(vector.y);
            vector.z = Mathf.Round(vector.z);
            vector.w = Mathf.Round(vector.w);
            return vector;
        }

        public static Vector2 Step(Vector2 vector, float stepSize)
        {
            if (stepSize == 0f)
            {
                return vector;
            }

            stepSize = 1f / stepSize;

            vector.x = Mathf.Round(vector.x * stepSize) / stepSize;
            vector.y = Mathf.Round(vector.y * stepSize) / stepSize;

            return vector;
        }

        public static Vector3 Step(Vector3 vector, float stepSize)
        {
            if (stepSize == 0f)
            {
                return vector;
            }

            stepSize = 1f / stepSize;

            vector.x = Mathf.Round(vector.x * stepSize) / stepSize;
            vector.y = Mathf.Round(vector.y * stepSize) / stepSize;
            vector.z = Mathf.Round(vector.z * stepSize) / stepSize;

            return vector;
        }

        public static Vector4 Step(Vector4 vector, float stepSize)
        {
            if (stepSize == 0f)
            {
                return vector;
            }

            stepSize = 1f / stepSize;

            vector.x = Mathf.Round(vector.x * stepSize) / stepSize;
            vector.y = Mathf.Round(vector.y * stepSize) / stepSize;
            vector.z = Mathf.Round(vector.z * stepSize) / stepSize;
            vector.w = Mathf.Round(vector.w * stepSize) / stepSize;

            return vector;
        }
        
        public static Vector3 ClosestPointOnLineSegment(Vector3 searchPoint, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 lClosestPoint = ClosestPointOnLine(searchPoint, lineStart, (lineEnd - lineStart).normalized);
            float lValue = InverseLerp3D(lineStart, lineEnd, lClosestPoint);
            lValue = Mathf.Clamp(lValue, 0f, 1f);
            return Vector3.Lerp(lineStart, lineEnd, lValue);
        }

        public static Vector3 ClosestPointOnLine(Vector3 searchPoint, Vector3 lineStart, Vector3 lineDir)
        {
            Vector3 lLinePoint = Vector3.zero;

            lLinePoint = lineStart + (Vector3.Dot((searchPoint - lineStart), lineDir) / (lineDir.magnitude * lineDir.magnitude)) * lineDir;

            return lLinePoint;
        }
        
        public static float InverseLerp3D(Vector3 a, Vector3 b, Vector3 value)
        {
            Vector3 AB = b - a;
            Vector3 AV = value - a;
            return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
        }

        public static Vector2 Max(params Vector2[] vectors)
        {
            float xMax = float.MinValue;
            float yMax = float.MinValue;

            for (int v = 0; v < vectors.Length; v++)
            {
                xMax = Mathf.Max(xMax, vectors[v].x);
                yMax = Mathf.Max(yMax, vectors[v].y);
            }

            return new Vector2(xMax, yMax);
        }
    }
}