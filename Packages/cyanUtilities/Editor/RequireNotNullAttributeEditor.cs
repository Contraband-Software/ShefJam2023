/*
Copyright (c) 2004 cyanseraph - This code is part of the CyanUtilities private Unity package
*/

using UnityEditor;
using UnityEngine;

namespace Cyan {
    /// <summary>
    /// This is the editor script responsible for highlighting the unset required attribute in red.
    /// </summary>
    [CustomPropertyDrawer(typeof(RequireNotNullAttribute))]
    internal class RequireNotNullAttributeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            label.text = "[" + ((RequireNotNullAttribute)attribute).message + "] " + label.text;

            if (property.objectReferenceValue == null)
            {
                GUI.color = Color.red;
                EditorGUI.PropertyField(position, property, label);
                //EditorGUI.HelpBox(new Rect(position.x, position.y + position.size.y + 2, position.size.x, position.size.y * 2), "Must be set", MessageType.Error);
            } else
            {
                EditorGUI.PropertyField(position, property, label);
            }
            GUI.color = Color.white;

            EditorGUI.EndProperty();
        }
    }
}