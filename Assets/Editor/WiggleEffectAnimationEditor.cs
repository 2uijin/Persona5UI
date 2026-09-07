using UnityEngine;
using UnityEditor;

public class WiggleEffectAnimationEditor : Editor
{
    SerializedProperty animationTypeProp;

    private void OnEnable()
    {
        animationTypeProp = serializedObject.FindProperty("animationType");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(animationTypeProp);
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();
    }
}
