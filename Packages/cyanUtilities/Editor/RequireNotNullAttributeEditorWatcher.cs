/*
Copyright (c) 2004 cyanseraph - This code is part of the CyanUtilities private Unity package
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
//using UnityEditor.UIElements;

namespace Cyan
{
    /// <summary>
    /// This editor script hooks into the EditorApplication.playModeStateChanged event and checks all monobehaviour scripts within the eactive scene for [RequiredNotNull] 
    /// attributes, it then ensures they are, shocker, not null.
    /// </summary>
    [InitializeOnLoad]
    internal class RequireNotNullAttributeEditorWatcher : EditorWindow
    {
        //static RequireNotNullAttributeEditorWatcher Instance;

        //static Vector2Int windowSize = new Vector2Int(300, 100);

        //static bool enforcePolicy = true;

        static RequireNotNullAttributeEditorWatcher()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        //[MenuItem("Cyan/RequiredNotNull Attribute Settings")]
        //public static void ShowSettingsPanel()
        //{
        //    RequireNotNullAttributeEditorWatcher window = (RequireNotNullAttributeEditorWatcher)EditorWindow.GetWindow(typeof(RequireNotNullAttributeEditorWatcher), true, "RequiredNotNull Attribute Settings");
        //    //window.titleContent = new GUIContent("RequiredNotNull Attribute Settings");
        //    window.minSize = windowSize;
        //    window.maxSize = windowSize;
        //    window.Show();
        //}

        //void OnGUI()
        //{
        //    EditorGUILayout.LabelField("Toggle off [RequiredNotNull] enforcement (Why?)");
        //    EditorGUILayout.Space();
        //    enforcePolicy = EditorGUILayout.Toggle("Enforce Attribute", enforcePolicy);

        //    SaveChanges();
        //}

        //private void OnDisable()
        //{
        //    SaveChanges();
        //}

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (//enforcePolicy && 
                (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.EnteredPlayMode))
            {
                HashSet<string> offendingInstances = new();
                uint offenses = 0;

                MonoBehaviour[] sceneActive = GameObject.FindObjectsOfType<MonoBehaviour>();

                foreach (MonoBehaviour mono in sceneActive)
                {
                    Type monoType = mono.GetType();

                    // Retreive the fields from the mono instance
#pragma warning disable S3011
                    FieldInfo[] objectFields = monoType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
#pragma warning restore S3011

                    for (int i = 0; i < objectFields.Length; i++)
                    {
                        if (RequireNotNullAttribute.GetCustomAttribute(objectFields[i], typeof(RequireNotNullAttribute)) is RequireNotNullAttribute
                            && objectFields[i].GetValue(mono).Equals(null))
                        {
                            offenses++;
                            offendingInstances.Add(mono.gameObject.name);
                        }
                    }
                }

                if (offenses != 0)
                {
                    EditorApplication.ExitPlaymode();
                    EditorApplication.isPlaying = false;
                    Debug.LogException(new InvalidOperationException("There are " + offenses.ToString() + " unset required references across GameObjects: " + string.Join(", ", offendingInstances)));
                }
            }
        }
    }
}