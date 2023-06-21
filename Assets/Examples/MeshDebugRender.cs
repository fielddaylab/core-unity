using System.Collections;
using System.Collections.Generic;
using FieldDay.Debugging;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MeshDebugRender : MonoBehaviour
{
    public Color BoxColor = Color.white;
    public float LineWidth = 1;
    public float LineDuration;
    public bool DepthTest = true;

    // Update is called once per frame
    void Update()
    {
        Bounds bounds = GetComponent<Renderer>().bounds;
        DebugDraw.AddBounds(bounds, BoxColor, LineWidth, LineDuration, DepthTest);
    }
}
