using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Northwind.Essentials;

public class ShapesTester : MonoBehaviour {

    [Header("Disc")]
    public Vector2 positionDisc = Vector2.zero;
    public float radiusDisc = 100f;
    public float blendDisc = 4f;

    [Header("Circle")]
    public Vector2 positionCircle = Vector2.zero;
    public float radiusCircle = 100f;
    public float widthCircle = 10f;
    public float blendCircle = 4f;

    [Header("Polygon")]
    public Vector2 positionPolygon = Vector2.zero;
    public float radiusPolygon = 100f;
    public int cornerCountPolygon = 5;
    public float edgeBend = 0.5f;
    public float edgeKnit = 0.5f;
    public float cornerBend = 0f;
    public float blendPolygon = 4f;



    void OnGUI() {
        GUIShapes.Disc(positionDisc, radiusDisc, blendDisc);
        GUIShapes.Circle(positionCircle, radiusCircle, widthCircle, blendCircle, Color.white);
        GUIShapes.Polygon(positionPolygon, radiusPolygon, cornerCountPolygon, edgeBend, edgeKnit, cornerBend, blendPolygon);

    }
}
