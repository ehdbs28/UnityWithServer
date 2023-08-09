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

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple GameManager is running!!!!!!");
            return;
        }

        Instance = this;

        NetworkManager.Instance = new NetworkManager(_host, _port.ToString());
    }

    #region 디버그 코드
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            NetworkManager.Instance.GetRequest("lunch", "?date-20230704", (type, message) =>
            {
                if (type == MessageType.SUCCESS)
                {
                    LunchVO lunch = JsonUtility.FromJson<LunchVO>(message);

                    foreach (string menu in lunch.menus)
                    {
                        Debug.Log(menu);
                    }
                }
                else
                {
                    Debug.LogError(message);
                }
            });
        }
    }
    #endregion
}
