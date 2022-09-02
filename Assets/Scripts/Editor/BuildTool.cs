using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using NUnit.Framework;
using PlasticGui;

public class BuildTool : Editor
{
    //标签
    [MenuItem("Tools/Build Windows Bundle")]
    static void BundleWindowsBuild()
    {
        Build(BuildTarget.StandaloneWindows);
    }
    
    [MenuItem("Tools/Build Android Bundle")]
    static void BundleAndroidBuild()
    {
        Build(BuildTarget.Android);
    }
    
    [MenuItem("Tools/Build iPhone Bundle")]
    static void BundleiPhoneBuild()
    {
        Build(BuildTarget.iOS);
    }
    
    [MenuItem("Tools/Build PS5 Bundle")]
    static void BundlePS5Build()
    {
        Build(BuildTarget.PS5);
    }
    
    static void Build(BuildTarget target)
    {
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
        
        //文件信息列表
        List<string> bundleInfos = new List<string>();
        
        //搜索文件
        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories);
        
        //因为meta文件也会找出来 排除meta
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".meta"))
            {
                continue;
            }
            
            
            AssetBundleBuild assetBundle = new AssetBundleBuild();

            string fileName = PathUtil.GetStandardPath(files[i]);
            Debug.Log("file:" + fileName);
            
            string assetName = PathUtil.GetUnityPath(files[i]); 
            assetBundle.assetNames = new string[] {assetName};

            string bundleName = fileName.Replace(PathUtil.BuildResourcesPath, "").ToLower();
            assetBundle.assetBundleName = bundleName + ".ab";
            
            assetBundleBuilds.Add(assetBundle);

            //获取依赖
            List<string> dependenceInfo = GetDependence(assetName);
            string bundleInfo = assetName + "|" + bundleName;

            if (dependenceInfo.Count > 0)
            {
                bundleInfo = bundleInfo + "|" + string.Join("|", dependenceInfo);
            }
            
            bundleInfos.Add(bundleInfo);
        }
        

        //如果存在就删除再创建一个 就不需要没次都创建一个了
        if (Directory.Exists(PathUtil.BundleOutPath))
        {
            Directory.Delete(PathUtil.BundleOutPath,true);
        }
        Directory.CreateDirectory(PathUtil.BundleOutPath);

        //构建Bundle
        //输出路径 数组 压缩格式 平台
        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, target);
        //输出路径 写入文件
        File.WriteAllLines(PathUtil.BundleOutPath + "/" + AppConst.FileListName,bundleInfos);
        
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 获取依赖
    /// </summary>
    /// <param name="curFile"></param>
    /// <returns></returns>
    static List<string> GetDependence(string curFile)
    {
        string[] files = AssetDatabase.GetDependencies(curFile);
        
        List<string> dependence = new List<string>();
        //排除不要把自身和脚本文件也放进去
        dependence = files.Where(file => !file.EndsWith(".cs") && !files.Equals(curFile)).ToList();
        return dependence;
    }
}
