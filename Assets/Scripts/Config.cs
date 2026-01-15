using UnityEngine;

public class Config : MonoBehaviour
{
    public static string ServerIP = "38.18.229.9";
    public static int ServerPort = 8080;
    public static bool UseSSL = false;
    public static string GameVersion = "1.0.0";
    public static string GameName = "Tales of Pirates Mobile";
    
    public static string GetServerURL()
    {
        string protocol = UseSSL ? "https" : "http";
        return $"{protocol}://{ServerIP}:{ServerPort}";
    }
    
    public static void LogConfig()
    {
        Debug.Log($"[Config] Server: {GetServerURL()}");
        Debug.Log($"[Config] Version: {GameVersion}");
        Debug.Log($"[Config] Game: {GameName}");
    }
}
