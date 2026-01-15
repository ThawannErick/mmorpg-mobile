using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CharacterData
{
    public int id;
    public int user_id;
    public string name;
    public string @class;
    public int level;
    public long exp;
    public int hp;
    public int mp;
    public float x;
    public float y;
    public float z;
    public int map_id;
    public string created_at;
}

[System.Serializable]
public class CreateCharacterRequest
{
    public string type = "create_character";
    public CreateCharacterPayload payload;
}

[System.Serializable]
public class CreateCharacterPayload
{
    public int user_id;
    public string name;
    public string @class;
}

[System.Serializable]
public class CreateCharacterResponse
{
    public bool success;
    public string message;
    public int character_id;
}

[System.Serializable]
public class LoadCharacterRequest
{
    public string type = "load_character";
    public LoadCharacterPayload payload;
}

[System.Serializable]
public class LoadCharacterPayload
{
    public int character_id;
}

[System.Serializable]
public class LoadCharacterResponse
{
    public bool success;
    public string message;
    public CharacterData character;
}

public class CharacterManager : MonoBehaviour
{
    private static CharacterManager _instance;
    private CharacterData _currentCharacter;
    
    public UnityEvent<CharacterData> OnCharacterLoaded = new UnityEvent<CharacterData>();
    public UnityEvent<string> OnCharacterCreated = new UnityEvent<string>();
    public UnityEvent<string> OnError = new UnityEvent<string>();
    
    public static CharacterManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("CharacterManager");
                _instance = obj.AddComponent<CharacterManager>();
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
    
    private void Start()
    {
        NetworkClient.Instance.OnMessageReceived.AddListener(HandleServerResponse);
    }
    
    public void CreateCharacter(string name, string characterClass)
    {
        int userId = AuthManager.Instance.GetUserId();
        
        CreateCharacterRequest request = new CreateCharacterRequest
        {
            payload = new CreateCharacterPayload
            {
                user_id = userId,
                name = name,
                @class = characterClass
            }
        };
        
        string json = JsonUtility.ToJson(request);
        NetworkClient.Instance.SendMessage(json);
    }
    
    public void LoadCharacter(int characterId)
    {
        LoadCharacterRequest request = new LoadCharacterRequest
        {
            payload = new LoadCharacterPayload
            {
                character_id = characterId
            }
        };
        
        string json = JsonUtility.ToJson(request);
        NetworkClient.Instance.SendMessage(json);
    }
    
    private void HandleServerResponse(string message)
    {
        try
        {
            if (message.Contains("\"character_id\"") && !message.Contains("\"character\":"))
            {
                CreateCharacterResponse response = JsonUtility.FromJson<CreateCharacterResponse>(message);
                if (response.success)
                {
                    OnCharacterCreated.Invoke(response.message);
                    SceneManager.LoadScene("MainGame");
                }
                else
                {
                    OnError.Invoke(response.message);
                }
            }
            else if (message.Contains("\"character\":"))
            {
                LoadCharacterResponse response = JsonUtility.FromJson<LoadCharacterResponse>(message);
                if (response.success)
                {
                    _currentCharacter = response.character;
                    OnCharacterLoaded.Invoke(_currentCharacter);
                }
                else
                {
                    OnError.Invoke(response.message);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Erro ao processar resposta: {e.Message}");
        }
    }
    
    public CharacterData GetCurrentCharacter() => _currentCharacter;
}
