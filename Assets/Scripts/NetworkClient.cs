using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

public class NetworkClient : MonoBehaviour
{
    private static NetworkClient _instance;
    private TcpClient _client;
    private NetworkStream _stream;
    private Thread _receiveThread;
    private bool _isConnected = false;
    private Queue<string> _messageQueue = new Queue<string>();
    
    public static NetworkClient Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("NetworkClient");
                _instance = obj.AddComponent<NetworkClient>();
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
    
    public bool Connect()
    {
        try
        {
            _client = new TcpClient();
            _client.Connect(Config.ServerIP, Config.ServerPort);
            _stream = _client.GetStream();
            _isConnected = true;
            
            _receiveThread = new Thread(ReceiveMessages);
            _receiveThread.Start();
            
            Debug.Log($"[Network] Conectado a {Config.ServerIP}:{Config.ServerPort}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Network] Erro ao conectar: {e.Message}");
            _isConnected = false;
            return false;
        }
    }
    
    public void SendMessage(string message)
    {
        if (!_isConnected || _stream == null)
        {
            Debug.LogError("[Network] Não conectado ao servidor");
            return;
        }
        
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            _stream.Write(data, 0, data.Length);
            _stream.Flush();
            Debug.Log($"[Network] Mensagem enviada: {message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Network] Erro ao enviar: {e.Message}");
            Disconnect();
        }
    }
    
    private void ReceiveMessages()
    {
        try
        {
            byte[] buffer = new byte[4096];
            while (_isConnected)
            {
                int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    _isConnected = false;
                    break;
                }
                
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                lock (_messageQueue)
                {
                    _messageQueue.Enqueue(message);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[Network] Erro ao receber: {e.Message}");
            _isConnected = false;
        }
    }
    
    public string GetMessage()
    {
        lock (_messageQueue)
        {
            if (_messageQueue.Count > 0)
                return _messageQueue.Dequeue();
        }
        return null;
    }
    
    public void Disconnect()
    {
        _isConnected = false;
        if (_stream != null)
            _stream.Close();
        if (_client != null)
            _client.Close();
        Debug.Log("[Network] Desconectado do servidor");
    }
    
    public bool IsConnected => _isConnected;
    
    private void OnDestroy()
    {
        Disconnect();
    }
}
