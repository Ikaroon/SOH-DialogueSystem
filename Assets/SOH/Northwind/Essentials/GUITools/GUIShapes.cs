using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Northwind.Essentials
{
    public static class GUIShapes
    {

        private const float BORDER_SMOOTH_DEFAULT = 4f;
        private const float BORDER_SMOOTH_DEFAULT_CIRCLE = 20f;
        private const float BORDER_SMOOTH_DEFAULT_POLYGON = 4f;

        #region Disc

        private const string DISC_SHADER = "Hidden/Northwind/SP_Disc";

        public static void Disc(Vector2 position, float radius, float borderSmooth, Color innerColor, Color outerColor)
        {
            if (radius == 0f)
            {
                return;
            }

            Material discMaterial = new Material(Shader.Find(DISC_SHADER));

            SetDefaultProps(discMaterial, position, radius, borderSmooth, innerColor, outerColor);

            discMaterial.SetFloat("_Radius", radius);

            SetUIMatrix(discMaterial);

            discMaterial.SetPass(0);
            Graphics.DrawProcedural(MeshTopology.Points, 1);
        }

        public static void Disc(Vector2 position, float radius, float borderSmooth, Color innerColor)
        {
            Disc(position, radius, borderSmooth, innerColor, new Color(innerColor.r, innerColor.g, innerColor.b, 0f));
        }

        public static void Disc(Vector2 position, float radius, float borderSmooth = 4f)
        {
            Disc(position, radius, borderSmooth, Color.white);
        }

        public static void Disc(Vector2 position, float radius, Color color)
        {
            Disc(position, radius, BORDER_SMOOTH_DEFAULT, color);
        }

        #endregion

        #region Circle

        private const string CIRCLE_SHADER = "Hidden/Northwind/SP_Circle";

        public static void Circle(Vector2 position, float radius, float width, float borderSmooth, Color innerColor, Color outerColor)
        {
            if (radius == 0f)
            {
                return;
            }

            Material circleMaterial = new Material(Shader.Find(CIRCLE_SHADER));

            SetDefaultProps(circleMaterial, position, radius, borderSmooth, innerColor, outerColor);

            circleMaterial.SetFloat("_Radius", radius);
            circleMaterial.SetFloat("_Width", width / radius);

            SetUIMatrix(circleMaterial);

            circleMaterial.SetPass(0);
            Graphics.DrawProcedural(MeshTopology.Points, 1);
        }

        public static void Circle(Vector2 position, float radius, float width, float borderSmooth, Color innerColor)
        {
            Circle(position, radius, width, borderSmooth, innerColor, new Color(innerColor.r, innerColor.g, innerColor.b, 0f));
        }

        public static void Circle(Vector2 position, float radius, float width, float borderSmooth = 4f)
        {
            Circle(position, radius, width, borderSmooth, Color.white);
        }

        public static void Circle(Vector2 position, float radius, float width, Color color)
        {
            Circle(position, radius, width, BORDER_SMOOTH_DEFAULT_CIRCLE, color);
        }

        #endregion

        #region Polygon

        private const string POLYGON_SHADER = "Hidden/Northwind/SP_Polygon";

        public static void Polygon(Vector2 position, float radius, int cornerCount, float edgeBend, float edgeKnit, float conerBend, float borderSmooth, Color innerColor, Color outerColor)
        {
            if (radius == 0f)
            {
                return;
            }

            Material polygonMaterial = new Material(Shader.Find(POLYGON_SHADER));

            SetDefaultProps(polygonMaterial, position, radius, borderSmooth, innerColor, outerColor);

            polygonMaterial.SetInt("_CornerCount", cornerCount);
            polygonMaterial.SetFloat("_EdgeBend", edgeBend);
            polygonMaterial.SetFloat("_EdgeKnit", edgeKnit);
            polygonMaterial.SetFloat("_CornerBend", conerBend);

            SetUIMatrix(polygonMaterial);

            polygonMaterial.SetPass(0);
            Graphics.DrawProcedural(MeshTopology.Points, 1);
        }

        public static void Polygon(Vector2 position, float radius, int cornerCount, float edgeBend, float edgeKnit, float cornerBend, float borderSmooth, Color innerColor)
        {
            Polygon(position, radius, cornerCount, edgeBend, edgeKnit, cornerBend, borderSmooth, innerColor, new Color(innerColor.r, innerColor.g, innerColor.b, 0f));
        }

        public static void Polygon(Vector2 position, float radius, int cornerCount, float edgeBend, float edgeKnit, float cornerBend, float borderSmooth)
        {
            Polygon(position, radius, cornerCount, edgeBend, edgeKnit, cornerBend, borderSmooth, Color.white);
        }

        public static void Polygon(Vector2 position, float radius, int cornerCount, float edgeBend, float edgeKnit, float cornerBend, Color innerColor)
        {
            Polygon(position, radius, cornerCount, edgeBend, edgeKnit, cornerBend, BORDER_SMOOTH_DEFAULT_POLYGON, innerColor);
        }

        public static void Polygon(Vector2 position, float radius, int cornerCount, float edgeBend, float edgeKnit, float cornerBend)
        {
            Polygon(position, radius, cornerCount, edgeBend, edgeKnit, cornerBend, BORDER_SMOOTH_DEFAULT_POLYGON, Color.white);
        }

        #endregion

        private static void SetDefaultProps(Material material, Vector2 position, float radius, float borderSmooth, Color innerColor, Color outerColor)
        {
            material.SetColor("_Color", innerColor);
            material.SetColor("_EdgeColor", outerColor);
            material.SetFloat("_Border", borderSmooth);
            material.SetFloat("_Radius", radius);
            material.SetVector("_WorldPos", new Vector4(position.x, position.y, 0f, 1f));
        }

        private static void SetUIMatrix(Material material)
        {
            material.SetMatrix("_MAT_V", Matrix4x4.TRS(new Vector3(-Screen.width, Screen.height, 0f),
                Quaternion.identity,
                new Vector3(2f, -2f, 1f)));
            material.SetMatrix("_MAT_P", Matrix4x4.TRS(Vector3.zero,
                Quaternion.identity,
                new Vector3(1f / Screen.width, 1f / Screen.height, 1f)));

        }

    }
}
 