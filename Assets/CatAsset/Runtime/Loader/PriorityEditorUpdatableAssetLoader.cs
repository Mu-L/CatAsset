﻿#if UNITY_EDITOR

using System;
using System.IO;
using System.Threading;

namespace CatAsset.Runtime
{
    /// <summary>
    /// 优先编辑器资源的可更新资源加载器
    /// </summary>
    public class PriorityEditorUpdatableAssetLoader : UpdatableAssetLoader
    {
        /// <inheritdoc />
        protected override AssetHandler<T> InternalLoadAssetAsync<T>(string assetName, CancellationToken token,
            TaskPriority priority)
        {
            
            
            AssetHandler<T> handler;

            if (string.IsNullOrEmpty(assetName))
            {
                handler = AssetHandler<T>.Create();
                handler.Error = "资源名为空";
                handler.SetAsset(null);
                return handler;
            }

            Type assetType = typeof(T);

            AssetCategory category = RuntimeUtil.GetAssetCategoryWithEditorMode(assetName, assetType);
            object asset;

            if (category == AssetCategory.InternalBundledAsset)
            {
                //加载资源包资源
                asset = UnityEditor.AssetDatabase.LoadAssetAtPath(assetName, assetType);
            }
            else
            {
                //加载原生资源
                if (category == AssetCategory.ExternalRawAsset)
                {
                    assetName = RuntimeUtil.GetReadWritePath(assetName);
                }

                asset = File.ReadAllBytes(assetName);
            }

            if (asset != null)
            {
                handler = AssetHandler<T>.Create(assetName,token, category);
                handler.SetAsset(asset);
                return handler;
            }

            return base.InternalLoadAssetAsync<T>(assetName, token, priority);
        }

        /// <inheritdoc />
        public override void UnloadAsset(object asset)
        {
            AssetRuntimeInfo info = CatAssetDatabase.GetAssetRuntimeInfo(asset);
            if (info == null)
            {
                //info允许为空
                return;
            }
            
            base.UnloadAsset(asset);
        }
    }
}

#endif

