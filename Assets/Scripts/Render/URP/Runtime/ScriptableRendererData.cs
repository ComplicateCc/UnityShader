using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// Class <c>ScriptableRendererData</c> contains resources for a <c>ScriptableRenderer</c>.
    /// <seealso cref="ScriptableRenderer"/>
    /// </summary>
    [MovedFrom("UnityEngine.Rendering.LWRP")] public abstract class ScriptableRendererData : ScriptableObject
    {
        internal bool isInvalidated { get; set; }

        /// <summary>
        /// Creates the instance of the ScriptableRenderer.
        /// </summary>
        /// <returns>The instance of ScriptableRenderer</returns>
        protected abstract ScriptableRenderer Create();

        [SerializeField] public List<ScriptableRendererFeature> m_RendererFeatures = new List<ScriptableRendererFeature>(10);
        [SerializeField] public List<long> m_RendererFeatureMap = new List<long>(10);

        /// <summary>
        /// List of additional render pass features for this renderer.
        /// </summary>
        public List<ScriptableRendererFeature> rendererFeatures
        {
            get => m_RendererFeatures;
        }

        /// <summary>
        /// Use SetDirty when changing seeings in the ScriptableRendererData.
        /// It will rebuild the render passes with the new data.
        /// </summary>
        public new void SetDirty()
        {
            isInvalidated = true;
        }

        internal ScriptableRenderer InternalCreateRenderer()
        {
            isInvalidated = false;
            return Create();
        }

        protected virtual void OnValidate()
        {
            SetDirty();
#if UNITY_EDITOR
            if (m_RendererFeatures.Contains(null))
                ValidateRendererFeatures();
#endif
        }

        protected virtual void OnEnable()
        {
            SetDirty();
        }

#if UNITY_EDITOR
        public virtual Material GetDefaultMaterial(DefaultMaterialType materialType)
        {
            return null;
        }

        internal virtual Shader GetDefaultShader()
        {
            return null;
        }

        public bool ValidateRendererFeatures()
        {
            // Get all Subassets
            var subassets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
            var linkedIds = new List<long>();
            var loadedAssets = new Dictionary<long, object>();
            var mapValid = m_RendererFeatureMap != null && m_RendererFeatureMap?.Count == m_RendererFeatures?.Count;

            var debugOutput = $"{name}\nValid Sub-assets:\n";

            // Collect valid, compiled sub-assets
            foreach (var asset in subassets)
            {
                if (asset == null || asset.GetType().BaseType != typeof(ScriptableRendererFeature)) continue;
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out var guid, out long localId);
                loadedAssets.Add(localId, asset);
                debugOutput += $"-{asset.name}\n--localId={localId}\n";
            }

            // Collect assets that are connected to the list
            for (var i = 0; i < m_RendererFeatures?.Count; i++)
            {
                if(!m_RendererFeatures[i]) continue;
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(m_RendererFeatures[i], out var guid, out long localId))
                {
                    linkedIds.Add(localId);
                }
            }

            var mapDebug = mapValid ? "Linking" : "Map missing, will attempt to re-map";
            debugOutput += $"Feature List Status({mapDebug}):\n";

            // Try fix missing references
            for (var i = 0; i < m_RendererFeatures?.Count; i++)
            {
                if (m_RendererFeatures[i] == null)
                {
                    if (mapValid && m_RendererFeatureMap[i] != 0)
                    {
                        var localId = m_RendererFeatureMap[i];
                        loadedAssets.TryGetValue(localId, out var asset);
                        m_RendererFeatures[i] = (ScriptableRendererFeature)asset;
                    }
                    else
                    {
                        m_RendererFeatures[i] = (ScriptableRendererFeature)GetUnusedAsset(ref linkedIds, ref loadedAssets);
                    }
                }
                debugOutput += m_RendererFeatures[i] != null ? $"-{i}:Linked\n" : $"-{i}:Missing\n";
            }
            if(UniversalRenderPipeline.asset.debugLevel != PipelineDebugLevel.Disabled)
                Debug.LogWarning(debugOutput);

            UpdateMap();

            if (!m_RendererFeatures.Contains(null)) return true;

            Debug.LogError($"{name} is missing RendererFeatures\nThis could be due to missing scripts or compile error.", this);
            return false;
        }

        private static object GetUnusedAsset(ref List<long> usedIds, ref Dictionary<long, object> assets)
        {
            foreach (var asset in assets)
            {
                var alreadyLinked = usedIds.Any(used => asset.Key == used);

                if (alreadyLinked) continue;
                usedIds.Add(asset.Key);
                return asset.Value;
            }

            return null;
        }

        private void UpdateMap()
        {
            if (m_RendererFeatureMap.Count != m_RendererFeatures.Count)
            {
                m_RendererFeatureMap.Clear();
                m_RendererFeatureMap.AddRange(new long[m_RendererFeatures.Count]);
            }

            for (var i = 0; i < rendererFeatures.Count; i++)
            {
                if(m_RendererFeatures[i] == null) continue;
                if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(m_RendererFeatures[i], out var guid,
                    out long localId)) continue;
                m_RendererFeatureMap[i] = localId;
            }
        }
#endif
    }
}

