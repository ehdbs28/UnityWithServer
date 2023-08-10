using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    
    [SerializeField] 
    private string _host;

    [SerializeField] 
    private int _port;
    
    public const string TokenKey = "token";
    public string Token { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple GameManager is running!!!!!!");
            return;
        }

        Instance = this;

        NetworkManager.Instance = new NetworkManager(_host, _port.ToString());
        UpdateToken();
    }

    public void UpdateToken()
    {
        Token = PlayerPrefs.GetString(TokenKey, string.Empty);
        if (!string.IsNullOrEmpty(Token))
        {
            NetworkManager.Instance.DoAuth();
        }
    }

    public void DistroyToken()
    {
        PlayerPrefs.DeleteKey(TokenKey);
        Token = string.Empty;
    }
}
