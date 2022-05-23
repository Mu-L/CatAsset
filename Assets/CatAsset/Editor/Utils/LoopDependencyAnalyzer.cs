﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CatAsset.Editor
{
    /// <summary>
    /// 循环依赖分析器
    /// </summary>
    public static class LoopDependencyAnalyzer
    {
        /// <summary>
        /// 分析循环依赖
        /// </summary>
        public static void Analyze(List<BundleBuildInfo> Bundles)
        {
            List<string> loops = new List<string>();
            
            foreach (BundleBuildInfo bundleBuildInfo in Bundles)
            {
                foreach (AssetBuildInfo assetBuildInfo  in bundleBuildInfo.Assets)
                {
                    string assetName = assetBuildInfo.AssetName;
                    
                    List<string> depChainList = new List<string>();  //记录依赖链的列表
                    depChainList.Add(assetName);

                    HashSet<string> depChainSet = new HashSet<string>();//记录依赖链的集合
                    depChainSet.Add(assetName);

                    if (!RecursiveDependencies(assetName,depChainSet,depChainList))
                    {
                        string loopLog = "     ";
                        HashSet<string> depLinkSet = new HashSet<string>();
                        foreach (string dep in depChainList)
                        {
                            loopLog += dep + "\n->";

                            if (depLinkSet.Contains(dep))
                            {
                                loopLog = loopLog.Replace(dep, "<color=#ff0000>" + dep + "</color>");
                            }
                            else
                            {
                               
                                depLinkSet.Add(dep);
                            }
                        }
                        loopLog += "\n--------------------";
                        loops.Add(loopLog);
                    }
                }
            }

            if (loops.Count > 0)
            {
                EditorUtility.DisplayDialog("提示", "检测到循环依赖,请查看控制台Log", "确认");
                string log = "检测到循环依赖：\n";
                foreach (string item in loops)
                {
                    log += item + "\n";
                }
                Debug.LogError(log);
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "未检测到循环依赖", "确认");
            }
        }
        
        /// <summary>
        /// 递归检查依赖
        /// </summary>
        private static bool RecursiveDependencies(string assetName,HashSet<string> depChainSet,List<string> depChainList)
        {
            //获取所有直接依赖
            List<string> dependencies = Util.GetDependencies(assetName, false);

            //递归检查依赖
            foreach (string item in dependencies)
            {
                if (depChainSet.Contains(item))
                {
                    //当前层级的资源有依赖到上一层级的资源 意味着出现循环依赖了
                    depChainList.Add(item);
                    return false;
                }

                depChainList.Add(item);
                depChainSet.Add(item);
                if (!RecursiveDependencies(item, depChainSet,depChainList))
                {
                    return false;
                }
                depChainList.RemoveAt(depChainList.Count - 1);
                depChainSet.Remove(item);
            }

            //把直接依赖都从set中移除
            foreach (string item in dependencies)
            {
                depChainSet.Remove(item);
            }

            return true;
        }
    }
}