using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(PathGenerator))]
public class PathGenerator_Editor : Editor
{
    private PathGenerator container;

    private void OnEnable()
    {
        container = (PathGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (container.attachableItems != null && container.attachableItems.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Aesthetic Items Spawn Chance Curves", EditorStyles.boldLabel);

            Rect rect = GUILayoutUtility.GetRect(256, 128);
            DrawCurves(rect, container.attachableItems.ToArray());
        }
    }

    private void DrawCurves(Rect rect, AestheticItem[] items)
    {
       
        if (Event.current.type == EventType.Repaint)
        {
            for (int i = 0; i < container.attachableItems.Count; i++)
            {
                var item = container.attachableItems[i];
                DrawCurve(rect, item.spawnChanceCurve, item.prefab.name, i);
            }
        }
    }

    private void DrawCurve(Rect rect, AnimationCurve curve, string label, int index)
    {
        // Define a list of colors
        List<Color> colors = new List<Color>
        {
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.magenta,
            Color.cyan,
            Color.gray,
            Color.black,
            Color.white,
            new Color(1f, 0.5f, 0f) // Orange
        };

        // Ensure that index is within bounds of the colors list
        Color curveColor = colors[index % colors.Count];
        Handles.color = curveColor;

        int steps = 100;
        Vector3 prevPoint = Vector3.zero;

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            float value = curve.Evaluate(t);
            Vector3 point = new Vector3(t * rect.width, (1f - value) * rect.height) + (Vector3)rect.position;

            if (i > 0)
            {
                Handles.DrawLine(prevPoint, point);
            }

            prevPoint = point;
        }

        // Draw label in the same color
        Handles.color = curveColor;
        Handles.Label(rect.position + new Vector2(10, 10 + 20 * index), label);
    }
}
