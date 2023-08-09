using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum Windows
{
    Lunch = 1,
    Login = 2,
}

public class UIController : MonoBehaviour
{
    public static UIController Instance = null;

    private UIDocument _uiDocument;
    private VisualElement _contentParent;
    
    private MessageSystem _messageSystem;
    public MessageSystem MessageSystem => _messageSystem;
    
    [SerializeField] 
    private VisualTreeAsset _lunchUIAsset;
    
    [SerializeField]
    private VisualTreeAsset _loginUIAsset;

    private Dictionary<Windows, WindowUI> _windowDictionary = new Dictionary<Windows, WindowUI>();

    private void Awake()
    {
        if (Instance != null)
            return;
        
        Instance = this;

        _uiDocument = GetComponent<UIDocument>();
        _messageSystem = GetComponent<MessageSystem>();
    }

    private void OnEnable()
    {
        var root = _uiDocument.rootVisualElement;
        
        Button lunchBtn = root.Q<Button>("LunchBtn");
        lunchBtn.RegisterCallback<ClickEvent>(OnOpenLunchHandle);

        Button loginBtn = root.Q<Button>("LoginBtn");
        loginBtn.RegisterCallback<ClickEvent>(OnOpenLoginHandle);

        _contentParent = root.Q("Content");
        var messageContainer = root.Q("MessageContainer");
        _messageSystem.SetContainer(messageContainer);
        
        #region add Window
        _windowDictionary.Clear();
        
        // lunch Ui 추가
        VisualElement lunchRoot = _lunchUIAsset.Instantiate();
        lunchRoot = lunchRoot.Q("LunchContainer");
        _contentParent.Add(lunchRoot);
        _windowDictionary.Add(Windows.Lunch, new LunchUI(lunchRoot));
        
        // login UI 추가
        VisualElement loginRoot = _loginUIAsset.Instantiate();
        loginRoot = loginRoot.Q("LoginWindow");
        _contentParent.Add(loginRoot);
        _windowDictionary.Add(Windows.Login, new LoginUI(loginRoot));
        #endregion
    }

    private void OnOpenLunchHandle(ClickEvent evt)
    {
        foreach (var pair in _windowDictionary)
        {
            if (pair.Key == Windows.Lunch)
            {
                pair.Value.Open();
                continue;
            }
            
            pair.Value.Close();
        }
    }

    private void OnOpenLoginHandle(ClickEvent evt)
    {
        foreach (var pair in _windowDictionary)
        {
            if (pair.Key == Windows.Login)
            {
                pair.Value.Open();
                continue;
            }
            
            pair.Value.Close();
        }
    }
}
