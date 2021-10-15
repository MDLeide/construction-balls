using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

class BallCostDrawer : OdinValueDrawer<BallCost>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        var cost = ValueEntry.SmartValue;

        var w = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 50f;

        EditorGUILayout.BeginHorizontal(new GUIStyle(){});
        EditorGUILayout.BeginVertical(new GUIStyle{fixedWidth = 100});
        
        cost.Blue = EditorGUILayout.IntField(new GUIContent("Blue"), cost.Blue);
        cost.Green = EditorGUILayout.IntField("Green", cost.Green);

        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical(new GUIStyle { fixedWidth = 100 });

        cost.Red = EditorGUILayout.IntField("Red", cost.Red);
        cost.Purple = EditorGUILayout.IntField("Purple", cost.Purple);

        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical(new GUIStyle { fixedWidth = 100 });

        cost.Yellow = EditorGUILayout.IntField("Yellow", cost.Yellow); 
        cost.Orange = EditorGUILayout.IntField(new GUIContent("Orange"), cost.Orange);

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = w;
    }
}