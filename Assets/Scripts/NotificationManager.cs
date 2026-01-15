using UnityEngine;
using UnityEngine.Events;

public class NotificationManager : MonoBehaviour
{
    private static NotificationManager _instance;
    
    public UnityEvent<string, string> OnNotificationReceived = new UnityEvent<string, string>();
    
    public static NotificationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("NotificationManager");
                _instance = obj.AddComponent<NotificationManager>();
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
    
    public void SendLocalNotification(string title, string message, int delaySeconds = 0)
    {
        #if UNITY_ANDROID
        SendAndroidNotification(title, message, delaySeconds);
        #endif
    }
    
    #if UNITY_ANDROID
    private void SendAndroidNotification(string title, string message, int delaySeconds)
    {
        var notification = new UnityEngine.Android.AndroidNotification();
        notification.Title = title;
        notification.Text = message;
        notification.FireTime = System.DateTime.Now.AddSeconds(delaySeconds);
        notification.SmallIcon = "icon_0";
        
        UnityEngine.Android.AndroidNotificationCenter.SendNotification(notification, "channel_0");
    }
    #endif
    
    public void OnNotificationReceived_Internal(string title, string message)
    {
        OnNotificationReceived.Invoke(title, message);
        Debug.Log($"Notificação recebida: {title} - {message}");
    }
}
