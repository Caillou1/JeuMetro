using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;

public sealed class Amplitude 
{
    private bool sendFromEditor = true;
    private const string apiUrl = "https://api.amplitude.com/httpapi";
    private static volatile Amplitude _instance;
	private string _UniqueUserId;
    private string _sessionId;
    private UnityWebRequest _www;

	public string apikey = "";

    public static Amplitude instance
	{
		get
		{
			if (Amplitude._instance == null)
				Amplitude._instance = new Amplitude();
			return Amplitude._instance;
		}
	}

    public Amplitude()
    {
        _UniqueUserId = SystemInfo.deviceUniqueIdentifier;
        if(_UniqueUserId == SystemInfo.unsupportedIdentifier)
        {
            if (!PlayerPrefs.HasKey("GUID"))
                PlayerPrefs.SetString("GUID", Guid.NewGuid().ToString());
            _UniqueUserId = PlayerPrefs.GetString("GUID");
        }

        _sessionId = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
    }

    public void SendEvent(string eventName)
    {
        sendEventPrivate(eventName, null);
    }

    public void SendEvent(string eventName, object data) // data must be a serialisable type
    {
        sendEventPrivate(eventName, JsonUtility.ToJson(data));
    }

    private void sendEventPrivate(string eventName, string data)
    {
        if(!sendFromEditor)
        {
            switch(Application.platform)
            {
                case RuntimePlatform.LinuxEditor :
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsEditor:
                    Debug.Log("Events from editor are disabled - " + eventName);
                    return;
                default:
                    break;
            }
        }

		string platform = Application.platform.ToString();
		int secondsSinceEpoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

		string eventData = "[{";
		eventData += "\"user_id\":\"" + _UniqueUserId + "\"";
		eventData += ", \"event_type\":\"" + eventName + "\"";
        if(data != null)
            eventData += ", \"user_properties\":" + data;
		eventData += ", \"platform\":\"" + platform + "\"";
		eventData += ", \"time\":" + secondsSinceEpoch.ToString();
		eventData += ", \"session_id\":" + _sessionId;
		eventData += "}]";
		Debug.Log(eventName + " " + eventData);

		WWWForm wwwf = new WWWForm();
		wwwf.AddField("api_key", apikey);
		wwwf.AddField("event", eventData);
		_www = UnityWebRequest.Post(apiUrl, wwwf);
		//_www.downloadHandler = new DownloadHandlerBuffer();
		_www.Send();
    }
}

