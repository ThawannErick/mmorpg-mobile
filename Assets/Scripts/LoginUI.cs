using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private InputField _usernameInput;
    [SerializeField] private InputField _passwordInput;
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _registerButton;
    [SerializeField] private Text _statusText;
    [SerializeField] private GameObject _loadingPanel;
    
    private bool _isRegistering = false;
    
    private void Start()
    {
        if (!NetworkClient.Instance.IsConnected)
        {
            NetworkClient.Instance.Connect(Config.ServerIP, Config.ServerPort);
        }
        
        _loginButton.onClick.AddListener(OnLoginClicked);
        _registerButton.onClick.AddListener(OnRegisterClicked);
        
        AuthManager.Instance.OnLoginSuccess.AddListener(OnLoginSuccess);
        AuthManager.Instance.OnLoginFailed.AddListener(OnLoginFailed);
        AuthManager.Instance.OnRegisterSuccess.AddListener(OnRegisterSuccess);
        AuthManager.Instance.OnRegisterFailed.AddListener(OnRegisterFailed);
        
        NetworkClient.Instance.OnConnected.AddListener(OnConnected);
        NetworkClient.Instance.OnDisconnected.AddListener(OnDisconnected);
    }
    
    private void OnLoginClicked()
    {
        if (string.IsNullOrEmpty(_usernameInput.text) || string.IsNullOrEmpty(_passwordInput.text))
        {
            _statusText.text = "Preencha todos os campos";
            return;
        }
        
        _isRegistering = false;
        _loadingPanel.SetActive(true);
        _statusText.text = "Conectando...";
        
        AuthManager.Instance.Login(_usernameInput.text, _passwordInput.text);
    }
    
    private void OnRegisterClicked()
    {
        if (string.IsNullOrEmpty(_usernameInput.text) || string.IsNullOrEmpty(_passwordInput.text))
        {
            _statusText.text = "Preencha todos os campos";
            return;
        }
        
        _isRegistering = true;
        _loadingPanel.SetActive(true);
        _statusText.text = "Registrando...";
        
        string email = _usernameInput.text + "@mmorpg.local";
        AuthManager.Instance.Register(_usernameInput.text, email, _passwordInput.text);
    }
    
    private void OnLoginSuccess(string message)
    {
        _loadingPanel.SetActive(false);
        _statusText.text = message;
        _statusText.color = Color.green;
    }
    
    private void OnLoginFailed(string message)
    {
        _loadingPanel.SetActive(false);
        _statusText.text = message;
        _statusText.color = Color.red;
    }
    
    private void OnRegisterSuccess(string message)
    {
        _loadingPanel.SetActive(false);
        _statusText.text = message;
        _statusText.color = Color.green;
    }
    
    private void OnRegisterFailed(string message)
    {
        _loadingPanel.SetActive(false);
        _statusText.text = message;
        _statusText.color = Color.red;
    }
    
    private void OnConnected()
    {
        _statusText.text = "Conectado ao servidor";
        _statusText.color = Color.green;
    }
    
    private void OnDisconnected()
    {
        _statusText.text = "Desconectado do servidor";
        _statusText.color = Color.red;
    }
}
