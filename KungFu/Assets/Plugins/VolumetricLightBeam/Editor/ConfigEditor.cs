#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace VLB
{
    [CustomEditor(typeof(Config))]
    public class ConfigEditor : EditorCommon
    {
        SerializedProperty geometryOverrideLayer, geometryLayerID, geometryTag, geometryRenderQueue, renderPipeline, renderingMode;
        SerializedProperty beamShader1Pass, beamShader2Pass;
        SerializedProperty sharedMeshSides, sharedMeshSegments;
        SerializedProperty globalNoiseScale, globalNoiseVelocity;
        SerializedProperty noise3DData, noise3DSize;
        SerializedProperty dustParticlesPrefab;
        bool isRenderQueueCustom = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            geometryOverrideLayer = FindProperty((Config x) => x.geometryOverrideLayer);
            geometryLayerID = FindProperty((Config x) => x.geometryLayerID);
            geometryTag = FindProperty((Config x) => x.geometryTag);

            geometryRenderQueue = FindProperty((Config x) => x.geometryRenderQueue);
            renderPipeline = FindProperty((Config x) => x.renderPipeline);
            renderingMode = FindProperty((Config x) => x.renderingMode);

            beamShader1Pass = serializedObject.FindProperty("beamShader1Pass");
            beamShader2Pass = serializedObject.FindProperty("beamShader2Pass");

            sharedMeshSides = FindProperty((Config x) => x.sharedMeshSides);
            sharedMeshSegments = FindProperty((Config x) => x.sharedMeshSegments);

            globalNoiseScale = FindProperty((Config x) => x.globalNoiseScale);
            globalNoiseVelocity = FindProperty((Config x) => x.globalNoiseVelocity);

            noise3DData = FindProperty((Config x) => x.noise3DData);
            noise3DSize = FindProperty((Config x) => x.noise3DSize);

            dustParticlesPrefab = FindProperty((Config x) => x.dustParticlesPrefab);

            RenderQueueGUIInit();

            Noise3D.LoadIfNeeded(); // Try to load Noise3D, maybe for the 1st time
        }

        void RenderQueueGUIInit()
        {
            isRenderQueueCustom = true;
            foreach (RenderQueue rq in System.Enum.GetValues(typeof(RenderQueue)))
            {
                if (rq != RenderQueue.Custom && geometryRenderQueue.intValue == (int)rq)
                {
                    isRenderQueueCustom = false;
                    break;
                }
            }
        }

        void RenderQueueGUIDraw()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                RenderQueue rq = isRenderQueueCustom ? RenderQueue.Custom : (RenderQueue)geometryRenderQueue.intValue;
                rq = (RenderQueue)EditorGUILayout.EnumPopup(EditorStrings.ConfigGeometryRenderQueue, rq);
                if (EditorGUI.EndChangeCheck())
                {
                    isRenderQueueCustom = (rq == RenderQueue.Custom);

                    if (!isRenderQueueCustom)
                        geometryRenderQueue.intValue = (int)rq;
                }

                EditorGUI.BeginDisabledGroup(!isRenderQueueCustom);
                {
                    geometryRenderQueue.intValue = EditorGUILayout.IntField(geometryRenderQueue.intValue, GUILayout.MaxWidth(65.0f));
                }
                EditorGUI.EndDisabledGroup();
            }
        }

        protected override void OnHeaderGUI()
        {
            GUILayout.BeginVertical("In BigTitle");
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Volumetric Light Beam - Plugin Configuration", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(string.Format("Current Version: {0}", Version.Current), EditorStyles.miniBoldLabel);
            EditorGUILayout.Separator();
		    GUILayout.EndVertical();
        }

        void ReimportShaders()
        {
            ReimportAsset(beamShader1Pass.objectReferenceValue);
            ReimportAsset(beamShader2Pass.objectReferenceValue);
        }

        void ReimportAsset(Object obj)
        {
            if (obj)
            {
                var path = AssetDatabase.GetAssetPath(beamShader1Pass.objectReferenceValue);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.Default);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            bool reloadNoise = false;

            if ((Object)Config.Instance != this.target)
            {
                EditorGUILayout.HelpBox(EditorStrings.ConfigMultipleAssets, MessageType.Error);
                EditorGUILayout.Separator();
                ButtonOpenConfig();
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                {
                    if (HeaderFoldable("Beam Geometry"))
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            geometryOverrideLayer.boolValue = EditorGUILayout.Toggle(EditorStrings.ConfigGeometryOverrideLayer, geometryOverrideLayer.boolValue);
                            using (new EditorGUI.DisabledGroupScope(!geometryOverrideLayer.boolValue))
                            {
                                geometryLayerID.intValue = EditorGUILayout.LayerField(geometryLayerID.intValue);
                            }
                        }

                        geometryTag.stringValue = EditorGUILayout.TagField(EditorStrings.ConfigGeometryTag, geometryTag.stringValue);
                    }
                    DrawLineSeparator();

                    if (HeaderFoldable("Rendering"))
                    {
                        RenderQueueGUIDraw();

                        if (BeamGeometry.isCustomRenderPipelineSupported)
                        {
                            EditorGUI.BeginChangeCheck();
                            {
                                renderPipeline.CustomEnum<RenderPipeline>(EditorStrings.ConfigGeometryRenderPipeline, EditorStrings.ConfigGeometryRenderPipelineEnumDescriptions);
                            }
                            if (EditorGUI.EndChangeCheck())
                            {
                                Config.OnRenderPipelineChanged((RenderPipeline)renderPipeline.enumValueIndex);
                                VolumetricLightBeam._EditorSetAllBeamGeomDirty(); // need to fully reset the BeamGeom to update the shader
                                ReimportShaders();
                            }
                        }

                        EditorGUI.BeginChangeCheck();
                        {
                            EditorGUILayout.PropertyField(renderingMode, EditorStrings.ConfigGeometryRenderingMode);

                            if (renderPipeline.enumValueIndex == (int)RenderPipeline.SRP_4_0_0_or_higher && renderingMode.enumValueIndex == (int)RenderingMode.MultiPass)
                                EditorGUILayout.HelpBox(EditorStrings.ConfigSrpAndMultiPassNoCompatible, MessageType.Error);

#pragma warning disable 0162 // warning CS0162: Unreachable code detected
                            if (renderingMode.enumValueIndex == (int)RenderingMode.GPUInstancing && !GpuInstancing.isSupported)
                                EditorGUILayout.HelpBox(EditorStrings.ConfigGeometryGpuInstancingNotSupported, MessageType.Warning);
#pragma warning restore 0162
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            VolumetricLightBeam._EditorSetAllBeamGeomDirty(); // need to fully reset the BeamGeom to update the shader
                            GlobalMesh.Destroy();
                            ReimportShaders();
                        }
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    VolumetricLightBeam._EditorSetAllMeshesDirty();
                }
                DrawLineSeparator();

                if(HeaderFoldable("Shared Mesh"))
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(sharedMeshSides, EditorStrings.ConfigSharedMeshSides);
                    EditorGUILayout.PropertyField(sharedMeshSegments, EditorStrings.ConfigSharedMeshSegments);
                    if (EditorGUI.EndChangeCheck())
                    {
                        GlobalMesh.Destroy();
                        VolumetricLightBeam._EditorSetAllMeshesDirty();
                    }

                    var meshInfo = "These properties will change the mesh tessellation of each Volumetric Light Beam with 'Shared' MeshType.\nAdjust them carefully since they could impact performance.";
                    meshInfo += string.Format("\nShared Mesh stats: {0} vertices, {1} triangles", MeshGenerator.GetSharedMeshVertexCount(), MeshGenerator.GetSharedMeshIndicesCount() / 3);
                    EditorGUILayout.HelpBox(meshInfo, MessageType.Info);
                }
                DrawLineSeparator();

                if (HeaderFoldable("Global 3D Noise"))
                {
                    EditorGUILayout.PropertyField(globalNoiseScale, EditorStrings.ConfigGlobalNoiseScale);
                    EditorGUILayout.PropertyField(globalNoiseVelocity, EditorStrings.ConfigGlobalNoiseVelocity);
                }
                DrawLineSeparator();

                if (HeaderFoldable("Internal Data (do not change)"))
                {
                    EditorGUILayout.PropertyField(beamShader1Pass, EditorStrings.ConfigBeamShader1Pass);
                    EditorGUILayout.PropertyField(beamShader2Pass, EditorStrings.ConfigBeamShader2Pass);
                    EditorGUILayout.PropertyField(dustParticlesPrefab, EditorStrings.ConfigDustParticlesPrefab);

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(noise3DData, EditorStrings.ConfigNoise3DData);
                    EditorGUILayout.PropertyField(noise3DSize, EditorStrings.ConfigNoise3DSize);
                    if (EditorGUI.EndChangeCheck())
                        reloadNoise = true;

                    if (Noise3D.isSupported && !Noise3D.isProperlyLoaded)
                        EditorGUILayout.HelpBox(EditorStrings.HelpNoiseLoadingFailed, MessageType.Error);
                }
                DrawLineSeparator();

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(EditorStrings.ConfigOpenDocumentation, EditorStyles.miniButton))
                    {
                        UnityEditor.Help.BrowseURL(Consts.HelpUrlConfig);
                    }

                    if (GUILayout.Button(EditorStrings.ConfigResetToDefaultButton, EditorStyles.miniButton))
                    {
                        UnityEditor.Undo.RecordObject(target, "Reset Config Properties");
                        (target as Config).Reset();
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();

            if(reloadNoise)
                Noise3D._EditorForceReloadData(); // Should be called AFTER ApplyModifiedProperties so the Config instance has the proper values when reloading data
        }
    }
}
#endif
