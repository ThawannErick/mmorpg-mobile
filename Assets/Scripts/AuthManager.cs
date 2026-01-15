using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LoginRequest
{
    public string type = "login";
    public LoginPayload payload;
}

[System.Serializable]
public class LoginPayload
{
    public string username;
    public string password;
}

[System.Serializable]
public class LoginResponse
{
    public bool success;
    public string message;
    public string token;
    public int user_id;
}

[System.Serializable]
public class RegisterRequest
{
    public string type = "register";
    public RegisterPayload payload;
}

[System.Serializable]
public class RegisterPayload
{
    public string username;
    public string email;
    public string password;
}

[System.Serializable]
public class RegisterResponse
{
    public bool success;
    public string message;
    public int user_id;
}

public class AuthManager : MonoBehaviour
{
    private static AuthManager _instance;
    private string _authToken;
    private int _userId;
    
    public UnityEvent<string> OnLoginSuccess = new UnityEvent<string>();
    public UnityEvent<string> OnLoginFailed = new UnityEvent<string>();
    public UnityEvent<string> OnRegisterSuccess = new UnityEvent<string>();
    public UnityEvent<string> OnRegisterFailed = new UnityEvent<string>();
    
    public static AuthManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("AuthManager");
                _instance = obj.AddComponent<AuthManager>();
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
    
    public void Login(string username, string password)
    {
        LoginRequest request = new LoginRequest
        {
            payload = new LoginPayload
            {
                username = username,
                password = password
            }
        };
        
        string json = JsonUtility.ToJson(request);
        NetworkClient.Instance.SendMessage(json);
    }
    
    public void Register(string username, string email, string password)
    {
        RegisterRequest request = new RegisterRequest
        {
            payload = new RegisterPayload
            {
                username = username,
                email = email,
                password = password
            }
        };
        
        string json = JsonUtility.ToJson(request);
        NetworkClient.Instance.SendMessage(json);
    }
    
    private void HandleServerResponse(string message)
    {
        try
        {
            if (message.Contains("\"token\""))
            {
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(message);
                if (response.success)
                {
                    _authToken = response.token;
                    _userId = response.user_id;
                    PlayerPrefs.SetString("AuthToken", _authToken);
                    PlayerPrefs.SetInt("UserId", _userId);
                    OnLoginSuccess.Invoke(response.message);
                    SceneManager.LoadScene("CharacterSelection");
                }
                else
                {
                    OnLoginFailed.Invoke(response.message);
                }
            }
            else if (message.Contains("\"user_id\""))
            {
                RegisterResponse response = JsonUtility.FromJson<RegisterResponse>(message);
                if (response.success)
                {
                    OnRegisterSuccess.Invoke(response.message);
                }
                else
                {
                    OnRegisterFailed.Invoke(response.message);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Erro ao processar resposta: {e.Message}");
        }
    }
    
    public string GetAuthToken() => _authToken;
    public int GetUserId() => _userId;
    public bool IsAuthenticated => !string.IsNullOrEmpty(_authToken);
}
