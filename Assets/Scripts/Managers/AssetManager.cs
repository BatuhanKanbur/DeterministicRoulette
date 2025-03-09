using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class AssetManager<T> where T : Object
{
    #region Initilzation

    private static bool IsInitialized
    {
        set
        {
            if (value && !Initialized)
                Init();
            Initialized = value;
        }
    }
    private static bool Initialized { get; set; }

    private static void Init()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
        IsInitialized = true;
    }

    private static void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        AssetCache.Clear();
    }

    #endregion

    private static readonly Dictionary<object, object> AssetCache = new Dictionary<object, object>();

    public static async Task<T> LoadObject(object assetReference)
    {
        if (AssetCache.TryGetValue(assetReference, out var assetObject))
            return (T) assetObject;
        T handleResult;
        try
        {
            var handle = Addressables.LoadAssetAsync<T>(assetReference);
            await handle.Task;
            handleResult = handle.Result;
            AssetCache[assetReference] = handle.Result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return handleResult;
    }

    public static async Task<List<T>> LoadObjects(object assetReference)
    {
        if (AssetCache.TryGetValue(assetReference, out var assetObject))
            return (List<T>) assetObject;
        var newAssetList = new List<T>();
        var handle = Addressables.LoadAssetsAsync<T>(assetReference, null);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            AssetCache[assetReference] = handle.Result;
            newAssetList.AddRange(handle.Result);
        }
        else
        {
            Debug.LogError(handle.OperationException);
        }

        return newAssetList;
    }

    public static void ReleaseAsset(List<object> assetReference)
    {
        if (assetReference is not {Count: > 0})
        {
            return;
        }

        foreach (var reference in assetReference)
        {
            if (!AssetCache.TryGetValue(reference, out var assetObject))
            {
                Debug.LogError("Asset not found in cache.");
                return;
            }

            Addressables.Release(assetObject);

            AssetCache.Remove(reference);

#if DEBUG_LOGS_ENABLED
                LoggerNS.Log($"{assetObject} Asset released!");
#endif
        }
    }
}