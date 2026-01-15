using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetManager : MonoBehaviour
{
    private static AssetManager _instance;
    private Dictionary<string, GameObject> _cachedAssets = new Dictionary<string, GameObject>();
    private Dictionary<string, Texture2D> _cachedTextures = new Dictionary<string, Texture2D>();
    
    public static AssetManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("AssetManager");
                _instance = obj.AddComponent<AssetManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public GameObject LoadAsset(string assetPath)
    {
        if (_cachedAssets.ContainsKey(assetPath))
        {
            return _cachedAssets[assetPath];
        }
        
        GameObject asset = Resources.Load<GameObject>(assetPath);
        if (asset != null)
        {
            _cachedAssets[assetPath] = asset;
        }
        return asset;
    }
    
    public Texture2D LoadTexture(string texturePath)
    {
        if (_cachedTextures.ContainsKey(texturePath))
        {
            return _cachedTextures[texturePath];
        }
        
        Texture2D texture = Resources.Load<Texture2D>(texturePath);
        if (texture != null)
        {
            _cachedTextures[texturePath] = texture;
        }
        return texture;
    }
    
    public IEnumerator LoadAssetAsync(string assetPath, System.Action<GameObject> callback)
    {
        if (_cachedAssets.ContainsKey(assetPath))
        {
            callback(_cachedAssets[assetPath]);
            yield break;
        }
        
        ResourceRequest request = Resources.LoadAsync<GameObject>(assetPath);
        yield return request;
        
        if (request.asset != null)
        {
            GameObject asset = request.asset as GameObject;
            _cachedAssets[assetPath] = asset;
            callback(asset);
        }
    }
    
    public void UnloadAsset(string assetPath)
    {
        if (_cachedAssets.ContainsKey(assetPath))
        {
            Resources.UnloadAsset(_cachedAssets[assetPath]);
            _cachedAssets.Remove(assetPath);
        }
    }
    
    public void ClearCache()
    {
        _cachedAssets.Clear();
        _cachedTextures.Clear();
        Resources.UnloadUnusedAssets();
    }
}
