#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 0429, 0162 // Unreachable expression code detected (because of Noise3D.isSupported on mobile)

namespace VLB
{
    [CustomEditor(typeof(VolumetricLightBeam))]
    [CanEditMultipleObjects]
    public class VolumetricLightBeamEditor : EditorCommon
    {
        SerializedProperty trackChangesDuringPlaytime;
        SerializedProperty colorFromLight, colorMode, color, colorGradient;
        SerializedProperty intensityFromLight, intensityModeAdvanced, intensityInside, intensityOutside;
        SerializedProperty blendingMode;
        SerializedProperty fresnelPow, glareFrontal, glareBehind;
        SerializedProperty spotAngleFromLight, spotAngle;
        SerializedProperty coneRadiusStart, geomMeshType, geomCustomSides, geomCustomSegments, geomCap;
        SerializedProperty fadeEndFromLight, fadeStart, fadeEnd;
        SerializedProperty attenuationEquation, attenuationCustomBlending;
        SerializedProperty depthBlendDistance, cameraClippingDistance;
        SerializedProperty noiseEnabled, noiseIntensity, noiseScaleUseGlobal, noiseScaleLocal, noiseVelocityUseGlobal, noiseVelocityLocal;
        SerializedProperty sortingLayerID, sortingOrder;

        List<VolumetricLightBeam> m_Entities;
        string[] m_SortingLayerNames;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Entities = new List<VolumetricLightBeam>();
            foreach (var ent in targets)
            {
                if (ent is VolumetricLightBeam)
                    m_Entities.Add(ent as VolumetricLightBeam);
            }
            Debug.Assert(m_Entities.Count > 0);

            colorFromLight = FindProperty((VolumetricLightBeam x) => x.colorFromLight);
            color = FindProperty((VolumetricLightBeam x) => x.color);
            colorGradient = FindProperty((VolumetricLightBeam x) => x.colorGradient);
            colorMode = FindProperty((VolumetricLightBeam x) => x.colorMode);

            intensityFromLight = FindProperty((VolumetricLightBeam x) => x.intensityFromLight);
            intensityModeAdvanced = FindProperty((VolumetricLightBeam x) => x.intensityModeAdvanced);
            intensityInside = FindProperty((VolumetricLightBeam x) => x.intensityInside);
            intensityOutside = FindProperty((VolumetricLightBeam x) => x.intensityOutside);

            blendingMode = FindProperty((VolumetricLightBeam x) => x.blendingMode);

            fresnelPow = FindProperty((VolumetricLightBeam x) => x.fresnelPow);

            glareFrontal = FindProperty((VolumetricLightBeam x) => x.glareFrontal);
            glareBehind = FindProperty((VolumetricLightBeam x) => x.glareBehind);

            spotAngleFromLight = FindProperty((VolumetricLightBeam x) => x.spotAngleFromLight);
            spotAngle = FindProperty((VolumetricLightBeam x) => x.spotAngle);

            coneRadiusStart = FindProperty((VolumetricLightBeam x) => x.coneRadiusStart);

            geomMeshType = FindProperty((VolumetricLightBeam x) => x.geomMeshType);
            geomCustomSides = FindProperty((VolumetricLightBeam x) => x.geomCustomSides);
            geomCustomSegments = FindProperty((VolumetricLightBeam x) => x.geomCustomSegments);
            geomCap = FindProperty((VolumetricLightBeam x) => x.geomCap);

            fadeEndFromLight = FindProperty((VolumetricLightBeam x) => x.fadeEndFromLight);
            fadeStart = FindProperty((VolumetricLightBeam x) => x.fadeStart);
            fadeEnd = FindProperty((VolumetricLightBeam x) => x.fadeEnd);

            attenuationEquation = FindProperty((VolumetricLightBeam x) => x.attenuationEquation);
            attenuationCustomBlending = FindProperty((VolumetricLightBeam x) => x.attenuationCustomBlending);

            depthBlendDistance = FindProperty((VolumetricLightBeam x) => x.depthBlendDistance);
            cameraClippingDistance = FindProperty((VolumetricLightBeam x) => x.cameraClippingDistance);

            // NOISE
            noiseEnabled = FindProperty((VolumetricLightBeam x) => x.noiseEnabled);
            noiseIntensity = FindProperty((VolumetricLightBeam x) => x.noiseIntensity);
            noiseScaleUseGlobal = FindProperty((VolumetricLightBeam x) => x.noiseScaleUseGlobal);
            noiseScaleLocal = FindProperty((VolumetricLightBeam x) => x.noiseScaleLocal);
            noiseVelocityUseGlobal = FindProperty((VolumetricLightBeam x) => x.noiseVelocityUseGlobal);
            noiseVelocityLocal = FindProperty((VolumetricLightBeam x) => x.noiseVelocityLocal);

            trackChangesDuringPlaytime = serializedObject.FindProperty("_TrackChangesDuringPlaytime");

            // 2D
            sortingLayerID = serializedObject.FindProperty("_SortingLayerID");
            sortingOrder = serializedObject.FindProperty("_SortingOrder");
            m_SortingLayerNames = SortingLayer.layers.Select(l => l.name).ToArray();
        }

        static void PropertyThickness(SerializedProperty sp)
        {
            sp.FloatSlider(
                EditorStrings.SideThickness,
                0, 1,
                (value) => Mathf.Clamp01(1 - (value / Consts.FresnelPowMaxValue)),    // conversion value to slider
                (value) => (1 - value) * Consts.FresnelPowMaxValue                    // conversion slider to value
                );
        }


        class ButtonToggleScope : System.IDisposable
        {
            SerializedProperty m_Property;
            bool m_DisableGroup = false;
            GUIContent m_Content = null;
            float m_Width = 75.0f;

            void Enable()
            {
                EditorGUILayout.BeginHorizontal();
                if(m_DisableGroup)
                    EditorGUI.BeginDisabledGroup(m_Property.HasAtLeastOneValue(true));
            }

            void Disable()
            {
                if(m_DisableGroup)
                    EditorGUI.EndDisabledGroup();
                DrawToggleButton();
                EditorGUILayout.EndHorizontal();
            }

            public ButtonToggleScope(SerializedProperty prop, bool disableGroup, GUIContent content, float width)
            {
                m_Property = prop;
                m_DisableGroup = disableGroup;
                m_Content = content;
                m_Width = width;
                Enable();
            }

            public void Dispose() { Disable(); }

            static GUIStyle ms_ToggleButtonStyleNormal = null;
            static GUIStyle ms_ToggleButtonStyleToggled = null;
            static GUIStyle ms_ToggleButtonStyleMixedValue = null;

            void DrawToggleButton()
            {
                if (ms_ToggleButtonStyleNormal == null)
                {
                    ms_ToggleButtonStyleNormal = new GUIStyle(EditorStyles.miniButton);
                    ms_ToggleButtonStyleToggled = new GUIStyle(ms_ToggleButtonStyleNormal);
                    ms_ToggleButtonStyleToggled.normal.background = ms_ToggleButtonStyleToggled.active.background;
                    ms_ToggleButtonStyleMixedValue = new GUIStyle(ms_ToggleButtonStyleToggled);
                    ms_ToggleButtonStyleMixedValue.fontStyle = FontStyle.Italic;
                }

                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = m_Property.hasMultipleDifferentValues;

                GUILayout.Button(
                    m_Content,
                    EditorGUI.showMixedValue ? ms_ToggleButtonStyleMixedValue : (m_Property.boolValue ? ms_ToggleButtonStyleToggled : ms_ToggleButtonStyleNormal),
                    GUILayout.MaxWidth(m_Width));

                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                    m_Property.boolValue = !m_Property.boolValue;
            }
        }

        static ButtonToggleScope ButtonToggleScopeFromLight(SerializedProperty prop, bool visible)
        {
            if (!visible) return null;

            return new ButtonToggleScope(prop,
                true,   // disableGroup
                EditorStrings.FromSpotLight,
                65f);
        }

        static ButtonToggleScope ButtonToggleScopeAdvanced(SerializedProperty prop, bool visible)
        {
            if (!visible) return null;

            return new ButtonToggleScope(prop,
                false,  // disableGroup
                EditorStrings.IntensityModeAdvanced,
                30f);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Debug.Assert(m_Entities.Count > 0);
            EditorGUILayout.Separator();

            bool hasLightSpot = false;
            var light = m_Entities[0].GetComponent<Light>();
            if (light)
            {
                hasLightSpot = light.type == LightType.Spot;
                if (!hasLightSpot)
                {
                    EditorGUILayout.HelpBox(EditorStrings.HelpNoSpotlight, MessageType.Warning);
                }
            }

            if (HeaderFoldable(EditorStrings.HeaderBasic))
            {
                // Color
                using (ButtonToggleScopeFromLight(colorFromLight, hasLightSpot))
                using (new EditorGUILayout.HorizontalScope()) // mandatory to have the color picker on the same line
                {
                    EditorGUILayout.PropertyField(colorMode, EditorStrings.ColorMode);

                    if (colorMode.enumValueIndex == (int)ColorMode.Gradient)
                        EditorGUILayout.PropertyField(colorGradient, EditorStrings.ColorGradient);
                    else
                        EditorGUILayout.PropertyField(color, EditorStrings.ColorFlat);
                }

                // Blending Mode
                EditorGUILayout.PropertyField(blendingMode, EditorStrings.BlendingMode);

                EditorGUILayout.Separator();

                // Intensity
                bool advancedModeEnabled = false;
                using (ButtonToggleScopeFromLight(intensityFromLight, hasLightSpot))
                {
                    bool advancedModeButton = !hasLightSpot || intensityFromLight.HasAtLeastOneValue(false);
                    using (ButtonToggleScopeAdvanced(intensityModeAdvanced, advancedModeButton))
                    {
                        advancedModeEnabled = intensityModeAdvanced.HasAtLeastOneValue(true);
                        EditorGUILayout.PropertyField(intensityOutside, advancedModeEnabled ? EditorStrings.IntensityOutside : EditorStrings.IntensitySimple);
                    }
                }

                if (advancedModeEnabled)
                    EditorGUILayout.PropertyField(intensityInside, EditorStrings.IntensityInside);
                else
                    intensityInside.floatValue = intensityOutside.floatValue;

                EditorGUILayout.Separator();

                // Spot Angle
                using (ButtonToggleScopeFromLight(spotAngleFromLight, hasLightSpot))
                {
                    EditorGUILayout.PropertyField(spotAngle, EditorStrings.SpotAngle);
                }

                PropertyThickness(fresnelPow);

                EditorGUILayout.Separator();

                EditorGUILayout.PropertyField(glareFrontal, EditorStrings.GlareFrontal);
                EditorGUILayout.PropertyField(glareBehind, EditorStrings.GlareBehind);

                EditorGUILayout.Separator();

                trackChangesDuringPlaytime.ToggleLeft(EditorStrings.TrackChanges);
                DrawAnimatorWarning();
            }
            DrawLineSeparator();

            if(HeaderFoldable(EditorStrings.HeaderAttenuation))
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(attenuationEquation, EditorStrings.AttenuationEquation);
                    if (attenuationEquation.enumValueIndex == (int)AttenuationEquation.Blend)
                        EditorGUILayout.PropertyField(attenuationCustomBlending, EditorStrings.AttenuationCustomBlending);
                }
                EditorGUILayout.EndHorizontal();

                // Fade End
                using (ButtonToggleScopeFromLight(fadeEndFromLight, hasLightSpot))
                {
                    EditorGUILayout.PropertyField(fadeEnd, EditorStrings.FadeEnd);
                }

                if (fadeEnd.hasMultipleDifferentValues)
                    EditorGUILayout.PropertyField(fadeStart, EditorStrings.FadeStart);
                else
                    fadeStart.FloatSlider(EditorStrings.FadeStart, 0f, fadeEnd.floatValue - Consts.FadeMinThreshold);
            }
            DrawLineSeparator();

            if(HeaderFoldable(EditorStrings.Header3DNoise))
            {
                EditorGUILayout.PropertyField(noiseEnabled, EditorStrings.NoiseEnabled);

                if (noiseEnabled.boolValue)
                {
                    EditorGUILayout.PropertyField(noiseIntensity, EditorStrings.NoiseIntensity);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUI.DisabledGroupScope(noiseScaleUseGlobal.boolValue))
                        {
                            EditorGUILayout.PropertyField(noiseScaleLocal, EditorStrings.NoiseScale);
                        }
                        noiseScaleUseGlobal.ToggleUseGlobalNoise();
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUI.DisabledGroupScope(noiseVelocityUseGlobal.boolValue))
                        {
                            EditorGUILayout.PropertyField(noiseVelocityLocal, EditorStrings.NoiseVelocity);
                        }
                        noiseVelocityUseGlobal.ToggleUseGlobalNoise();
                    }

                    ButtonOpenConfig();

                    if (Noise3D.isSupported && !Noise3D.isProperlyLoaded)
                        EditorGUILayout.HelpBox(EditorStrings.HelpNoiseLoadingFailed, MessageType.Error);

                    if (!Noise3D.isSupported)
                        EditorGUILayout.HelpBox(Noise3D.isNotSupportedString, MessageType.Info);
                }
            }
            DrawLineSeparator();

            if(HeaderFoldable(EditorStrings.HeaderBlendingDistances))
            {
                EditorGUILayout.PropertyField(cameraClippingDistance, EditorStrings.CameraClippingDistance);

                var content = new GUIContent(EditorStrings.DepthBlendDistance);
                if (depthBlendDistance.hasMultipleDifferentValues)
                    content.text += " (-)";
                else
                    content.text += depthBlendDistance.floatValue > 0.0 ? " (on)" : " (off)";
                EditorGUILayout.PropertyField(depthBlendDistance, content);
            }
            DrawLineSeparator();

            if(HeaderFoldable(EditorStrings.HeaderGeometry))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(coneRadiusStart, EditorStrings.ConeRadiusStart);
                    EditorGUI.BeginChangeCheck();
                    {
                        geomCap.ToggleLeft(EditorStrings.GeomCap, GUILayout.MaxWidth(40.0f));
                    }
                    if (EditorGUI.EndChangeCheck()) { SetMeshesDirty(); }
                }

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.PropertyField(geomMeshType, EditorStrings.GeomMeshType);
                }
                if (EditorGUI.EndChangeCheck()) { SetMeshesDirty(); }

                if (geomMeshType.intValue == (int)MeshType.Custom)
                {
                    EditorGUI.indentLevel++;

                    EditorGUI.BeginChangeCheck();
                    {
                        EditorGUILayout.PropertyField(geomCustomSides, EditorStrings.GeomSides);
                        EditorGUILayout.PropertyField(geomCustomSegments, EditorStrings.GeomSegments);
                    }
                    if (EditorGUI.EndChangeCheck()) { SetMeshesDirty(); }

                    EditorGUI.indentLevel--;
                }

                if (m_Entities.Count == 1)
                {
                    EditorGUILayout.HelpBox(m_Entities[0].meshStats, MessageType.Info);
                }
            }
            DrawLineSeparator();

            if (HeaderFoldable(EditorStrings.Header2D))
            {
        
                DrawSortingLayer();
                DrawSortingOrder();
            }

            DrawLineSeparator();

            if (DrawInfos())
            {
                DrawLineSeparator();
            }

            DrawCustomActionButtons();
            DrawAdditionalFeatures();

            serializedObject.ApplyModifiedProperties();
        }

        void SetMeshesDirty()
        {
            foreach (var entity in m_Entities) entity._EditorSetMeshDirty();
        }

        void DrawSortingLayer()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.showMixedValue = sortingLayerID.hasMultipleDifferentValues;
            int layerIndex = System.Array.IndexOf(m_SortingLayerNames, SortingLayer.IDToName(sortingLayerID.intValue));
            layerIndex = EditorGUILayout.Popup(EditorStrings.SortingLayer, layerIndex, m_SortingLayerNames);
            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(m_Entities.ToArray(), "Edit Sorting Layer");
                sortingLayerID.intValue = SortingLayer.NameToID(m_SortingLayerNames[layerIndex]);
                foreach (var entity in m_Entities) { entity.sortingLayerID = sortingLayerID.intValue; } // call setters
            }
        }

        void DrawSortingOrder()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(sortingOrder, EditorStrings.SortingOrder);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(m_Entities.ToArray(), "Edit Sorting Order");
                foreach (var entity in m_Entities) { entity.sortingOrder = sortingOrder.intValue; } // call setters
            }
        }

        bool HasAtLeastOneBeamWith(System.Func<VolumetricLightBeam, bool> lambda)
        {
            foreach (var entity in m_Entities)
            {
                if (lambda(entity))
                {
                    return true;
                }
            }
            return false;
        }

        void DrawAnimatorWarning()
        {
            var showAnimatorWarning = HasAtLeastOneBeamWith((VolumetricLightBeam beam) => { return beam.GetComponent<Animator>() != null && beam.trackChangesDuringPlaytime == false; });

            if (showAnimatorWarning)
                EditorGUILayout.HelpBox(EditorStrings.HelpAnimatorWarning, MessageType.Warning);
        }

        void DrawCustomActionButtons()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(EditorStrings.ButtonResetProperties, EditorStyles.miniButtonLeft))
                {
                    UnityEditor.Undo.RecordObjects(m_Entities.ToArray(), "Reset Light Beam Properties");
                    foreach (var entity in m_Entities) { entity.Reset(); entity.GenerateGeometry(); }
                }

                if (geomMeshType.intValue == (int)MeshType.Custom)
                {
                    if (GUILayout.Button(EditorStrings.ButtonGenerateGeometry, EditorStyles.miniButtonRight))
                    {
                        foreach (var entity in m_Entities) entity.GenerateGeometry();
                    }
                }
            }
        }

        void DrawAdditionalFeatures()
        {
#if UNITY_5_5_OR_NEWER
            bool showButtonDust         = HasAtLeastOneBeamWith((VolumetricLightBeam beam) => { return beam.GetComponent<VolumetricDustParticles>() == null; });
#else
            bool showButtonDust = false;
#endif
            bool showButtonOcclusion    = HasAtLeastOneBeamWith((VolumetricLightBeam beam) => { return beam.GetComponent<DynamicOcclusion>() == null; });
            bool showButtonTriggerZone  = HasAtLeastOneBeamWith((VolumetricLightBeam beam) => { return beam.GetComponent<TriggerZone>() == null; });

            if (showButtonDust || showButtonOcclusion || showButtonTriggerZone)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (showButtonDust && GUILayout.Button(EditorStrings.ButtonAddDustParticles, EditorStyles.miniButton))
                    {
                        Undo.RecordObjects(m_Entities.ToArray(), "Add Floating Dust Particles");
                        foreach (var entity in m_Entities) { entity.gameObject.AddComponent<VolumetricDustParticles>(); }
                    }

                    if (showButtonOcclusion && GUILayout.Button(EditorStrings.ButtonAddDynamicOcclusion, EditorStyles.miniButton))
                    {
                        Undo.RecordObjects(m_Entities.ToArray(), "Add Dynamic Occlusion");
                        foreach (var entity in m_Entities) { entity.gameObject.AddComponent<DynamicOcclusion>(); }
                    }

                    if (showButtonTriggerZone && GUILayout.Button(EditorStrings.ButtonAddTriggerZone, EditorStyles.miniButton))
                    {
                        Undo.RecordObjects(m_Entities.ToArray(), "Add Trigger Zone");
                        foreach (var entity in m_Entities) { entity.gameObject.AddComponent<TriggerZone>(); }
                    }
                }
            }
        }

        bool DrawInfos()
        {
            var tips = GetInfoTips();
            var gpuInstancingReport = GetGpuInstancingReport();

            if (tips.Count > 0 || !string.IsNullOrEmpty(gpuInstancingReport))
            {
                if (HeaderFoldable(EditorStrings.HeaderInfos))
                {
                    foreach (var tip in tips)
                        EditorGUILayout.HelpBox(tip, MessageType.Info);

                    if (!string.IsNullOrEmpty(gpuInstancingReport))
                        EditorGUILayout.HelpBox(gpuInstancingReport, MessageType.Warning);
                }
                return true;
            }
            return false;
        }

        List<string> GetInfoTips()
        {
            var tips = new List<string>();
            if (m_Entities.Count == 1)
            {
                if (depthBlendDistance.floatValue > 0f || !Noise3D.isSupported || trackChangesDuringPlaytime.boolValue)
                {
                    if (depthBlendDistance.floatValue > 0f)
                    {
                        tips.Add(EditorStrings.HelpDepthTextureMode);
#if UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID
                        tips.Add(EditorStrings.HelpDepthMobile);
#endif
                    }

                    if (trackChangesDuringPlaytime.boolValue)
                        tips.Add(EditorStrings.HelpTrackChangesEnabled);
                }
            }
            return tips;
        }

        string GetGpuInstancingReport()
        {
            if (m_Entities.Count > 1 && GpuInstancing.isSupported)
            {
                string reasons = "";
                for (int i = 1; i < m_Entities.Count; ++i)
                {
                    if (!GpuInstancing.CanBeBatched(m_Entities[0], m_Entities[i], ref reasons))
                    {
                        return "Selected beams can't be batched together:\n" + reasons;
                    }
                }
            }
            return null;
        }
    }
}
#endif
