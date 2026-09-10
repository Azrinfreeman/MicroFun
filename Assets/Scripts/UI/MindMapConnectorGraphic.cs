using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(CanvasRenderer))]
public sealed class MindMapConnectorGraphic : MaskableGraphic
{
    [SerializeField] private Vector2 startPoint;
    [SerializeField] private Vector2 endPoint;
    [SerializeField, Min(1f)] private float thickness = 5f;
    [SerializeField, Min(0f)] private float bend = 70f;
    [SerializeField, Range(4, 48)] private int segments = 20;

    public void Configure(Vector2 start, Vector2 end, Color lineColor, float lineThickness, float curveBend)
    {
        startPoint = start;
        endPoint = end;
        color = lineColor;
        thickness = Mathf.Max(1f, lineThickness);
        bend = Mathf.Max(0f, curveBend);
        raycastTarget = false;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vertexHelper)
    {
        vertexHelper.Clear();

        float direction = Mathf.Sign(endPoint.x - startPoint.x);
        if (Mathf.Approximately(direction, 0f))
            direction = 1f;

        Vector2 controlA = startPoint + Vector2.right * direction * bend;
        Vector2 controlB = endPoint - Vector2.right * direction * bend;
        int count = Mathf.Max(4, segments);
        float half = thickness * 0.5f;

        for (int index = 0; index < count; index++)
        {
            float t0 = index / (float)count;
            float t1 = (index + 1) / (float)count;
            Vector2 p0 = EvaluateBezier(startPoint, controlA, controlB, endPoint, t0);
            Vector2 p1 = EvaluateBezier(startPoint, controlA, controlB, endPoint, t1);
            Vector2 tangent = (p1 - p0).normalized;
            Vector2 normal = new Vector2(-tangent.y, tangent.x) * half;

            int baseIndex = vertexHelper.currentVertCount;
            AddVertex(vertexHelper, p0 + normal);
            AddVertex(vertexHelper, p0 - normal);
            AddVertex(vertexHelper, p1 - normal);
            AddVertex(vertexHelper, p1 + normal);
            vertexHelper.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
            vertexHelper.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
        }
    }

    private void AddVertex(VertexHelper helper, Vector2 position)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;
        vertex.position = position;
        vertex.uv0 = Vector2.zero;
        helper.AddVert(vertex);
    }

    private static Vector2 EvaluateBezier(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
    {
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * oneMinusT * a
             + 3f * oneMinusT * oneMinusT * t * b
             + 3f * oneMinusT * t * t * c
             + t * t * t * d;
    }
}
