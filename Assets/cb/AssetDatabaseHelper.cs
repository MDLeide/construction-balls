using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

static class AssetDatabaseHelper
{
    public static bool WarnOnNullAsset { get; set; } = true;

    //public static T[] LoadAssetsFromFolder<T>(string folderPath, string searchString, bool includeSubFolders = false)
    //    where T : UnityEngine.Object
    //{
    //    var folders = new List<string> {folderPath};
    //    if (includeSubFolders)
    //        foreach (var sub in AssetDatabase.GetSubFolders(folderPath))
    //            folders.Add(sub);

    //    var assetIds = AssetDatabase.FindAssets(searchString, folders.ToArray());

    //    var assets = new List<T>();
    //    foreach (var guid in assetIds)
    //    {
    //        var path = AssetDatabase.GUIDToAssetPath(guid);
    //        var asset = AssetDatabase.LoadAssetAtPath<T>(path);
    //        if (asset == null)
    //        {
    //            if (WarnOnNullAsset)
    //                Debug.LogWarning($"Failed to load asset at path: {path}. Expected type: {typeof(T).Name}");
    //        }
    //        else
    //        {
    //            assets.Add(asset);
    //        }
    //    }

    //    return assets.ToArray();
    //}

    public static T[] LoadAssetsFromFolder<T>(string folderPath, string searchString = null)
        where T : UnityEngine.Object
    {
        if (searchString == null)
            searchString = $"t:{typeof(T).Name}";

        var assetIds = AssetDatabase.FindAssets(searchString, new[] {folderPath});

        var assets = new List<T>();
        foreach (var guid in assetIds)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                if (WarnOnNullAsset)
                    Debug.LogWarning($"Failed to load asset at path: {path}. Expected type: {typeof(T).Name}");
            }
            else
            {
                assets.Add(asset);
            }
        }

        return assets.ToArray();
    }
}