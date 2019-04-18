#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;

namespace VLB
{
    public class EditorCommon : Editor
    {
        protected virtual void OnEnable() {}

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        }

        protected static void Header(string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }

        protected bool HeaderFoldable(string label)
        {
            return HeaderFoldable(new GUIContent(label));
        }

        protected bool HeaderFoldable(GUIContent label)
        {
            if (ms_StyleHeaderFoldable == null)
            {
                ms_StyleHeaderFoldable = new GUIStyle(EditorStyles.foldout);
                ms_StyleHeaderFoldable.fontStyle = FontStyle.Bold;
            }

            var uniqueString = this.ToString() + label.text;
            bool folded = ms_FoldedHeaders.Contains(uniqueString);

#if UNITY_5_5_OR_NEWER
            folded = !EditorGUILayout.Foldout(!folded, label, toggleOnLabelClick: true, style: ms_StyleHeaderFoldable);
#else
            folded = !EditorGUILayout.Foldout(!folded, label, ms_StyleHeaderFoldable);
#endif

            if (folded) ms_FoldedHeaders.Add(uniqueString);
            else ms_FoldedHeaders.Remove(uniqueString);

            return !folded;
        }

        protected static void DrawLineSeparator()
        {
            DrawLineSeparator(Color.grey, 1, 10);
        }

        static void DrawLineSeparator(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));

            r.x = 0;
            r.width = EditorGUIUtility.currentViewWidth;

            r.y += padding / 2;
            r.height = thickness;

            EditorGUI.DrawRect(r, color);
        }

        protected SerializedProperty FindProperty<T, TValue>(Expression<Func<T, TValue>> expr)
        {
            Debug.Assert(serializedObject != null);
            return serializedObject.FindProperty(ReflectionUtils.GetFieldPath(expr));
        }

        protected void ButtonOpenConfig()
        {
            if (GUILayout.Button(EditorStrings.ButtonOpenGlobalConfig, EditorStyles.miniButton))
                Config.EditorSelectInstance();
        }

        static HashSet<String> ms_FoldedHeaders = new HashSet<String>();
        static GUIStyle ms_StyleHeaderFoldable = null;
    }
}
#endif
