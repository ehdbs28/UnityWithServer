using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum MessageType{
    ERROR = 1,
    SUCCESS = 2,
    EMPTY = 3
}

public class NetworkManager
{
    public static NetworkManager Instance = null;

    private string _host;
    private string _port;
    
    public NetworkManager(string host, string port)
    {
        _host = host;
        _port = port;
    }
    
    public void GetRequest(string uri, string query, Action<MessageType, string> Callback)
    {
        GameManager.Instance.StartCoroutine(GetCoroutine(uri, query, Callback));
    }

    private IEnumerator GetCoroutine(string uri, string query, Action<MessageType, string> Callback)
    {
        string url = $"{_host}:{_port}/{uri}{query}";
        UnityWebRequest req = UnityWebRequest.Get(url);
        // 해당 url로 웹브라우저 주소창에 친거랑 똑같음
        
        SetRequestToken(req);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            UIController.Instance.MessageSystem.AddMessage($"요청이 실패했습니다. {req.responseCode}", 3f);
            Callback?.Invoke(MessageType.ERROR, $"{req.responseCode}_Error on Get");
            yield break;
        }

        MessageDTO msg = JsonUtility.FromJson<MessageDTO>(req.downloadHandler.text);
        Callback?.Invoke(msg.type, msg.message);
        
        req.Dispose();
    }

    public void PostRequest(string uri, Payload payload, Action<MessageType, string> Callback)
    {
        GameManager.Instance.StartCoroutine(PostCoroutine(uri, payload, Callback));
    }

    private IEnumerator PostCoroutine(string uri, Payload payload, Action<MessageType, string> Callback)
    {
        string url = $"{_host}:{_port}/{uri}";

        UnityWebRequest req = UnityWebRequest.Post(url, payload.GetWWWForm());
        //req.SetRequestHeader("content-type", "application/json");
        
        // 만약 토큰이 없다면 토큰을 헤더에 셋팅
        SetRequestToken(req);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            UIController.Instance.MessageSystem.AddMessage($"요청이 실패했습니다. {req.responseCode}", 3f);
            yield break;
        }
        
        MessageDTO msg = JsonUtility.FromJson<MessageDTO>(req.downloadHandler.text);
        Callback?.Invoke(msg.type, msg.message);
        
        req.Dispose();
    }

    public void DoAuth()
    {
        GetRequest("user", "", (type, json) =>
        {
            if (type == MessageType.SUCCESS)
            {
                // 내가 토큰을 보유중이라면 로그인 정보를 받을 수 있다.
                Debug.Log(json);

                UserVO vo = JsonUtility.FromJson<UserVO>(json);
                UIController.Instance.SetUserInfo(vo);
            }
        });
    }

    private void SetRequestToken(UnityWebRequest req)
    {
        if (!string.IsNullOrEmpty(GameManager.Instance.Token))
        {
            req.SetRequestHeader("Authorization", $"Bearer{GameManager.Instance.Token}");
        }
    }
}
