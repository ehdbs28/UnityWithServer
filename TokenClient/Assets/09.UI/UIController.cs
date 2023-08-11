using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public enum Windows
{
    Lunch = 1,
    Login = 2,
    Inventory = 3,
    Rank = 4,
    Ranking = 5,
}

public class UIController : MonoBehaviour
{
    public static UIController Instance = null;

    public List<ItemSO> _itemList = new List<ItemSO>();
    
    private VisualElement _root;

    private UIDocument _uiDocument;
    private VisualElement _contentParent;

    private Button _loginBtn;
    
    private UserInfoPanel _userInfoPanel;
    
    private MessageSystem _messageSystem;
    public MessageSystem MessageSystem => _messageSystem;
    
    [SerializeField] 
    private VisualTreeAsset _lunchUIAsset;
    
    [SerializeField]
    private VisualTreeAsset _loginUIAsset;

    [SerializeField] 
    private VisualTreeAsset _inventoryUIAsset;

    [SerializeField] 
    private VisualTreeAsset _itemUIAsset;

    [SerializeField] private VisualTreeAsset _rankRegisterAsset;
    [SerializeField] private VisualTreeAsset _rankingAsset;

    [SerializeField] private VisualTreeAsset _rankItem;

    private Label _scoreLabel;

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
        _root = _uiDocument.rootVisualElement;
        
        Button lunchBtn = _root.Q<Button>("LunchBtn");
        lunchBtn.RegisterCallback<ClickEvent>(OnOpenLunchHandle);

        _loginBtn = _root.Q<Button>("LoginBtn");
        _loginBtn.RegisterCallback<ClickEvent>(OnOpenLoginHandle);

        Button invenBtn = _root.Q<Button>("InventoryBtn");
        invenBtn.RegisterCallback<ClickEvent>(OnOpenInventoryHandle);
        
        _root.Q<Button>("RankBtn").RegisterCallback<ClickEvent>(e =>
        {
            foreach (var pair in _windowDictionary)
            {
                if (pair.Key == Windows.Ranking)
                {
                    pair.Value.Open();
                    continue;
                }
            
                pair.Value.Close();
            }
        });

        var userInfoElement = _root.Q<VisualElement>("UserInfoPanel");
        var userPopElement = _root.Q<VisualElement>("UserPopOver");
        _userInfoPanel = new UserInfoPanel(userInfoElement, userPopElement, OnLogoutHandle);

        _contentParent = _root.Q("Content");
        var messageContainer = _root.Q("MessageContainer");
        _messageSystem.SetContainer(messageContainer);

        _scoreLabel = _root.Q<Label>("scoreLabel");
        TimeController.Instance.TimeEvent += (time) =>
        {
            _scoreLabel.text = time.ToString("D3");
        };
        
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
        
        // inventory UI 추가
        VisualElement inventoryRoot = _inventoryUIAsset.Instantiate();
        inventoryRoot = inventoryRoot.Q("InventoryBody");
        _contentParent.Add(inventoryRoot);
        _windowDictionary.Add(Windows.Inventory, new InventoryUI(inventoryRoot, _itemUIAsset));
        
        VisualElement rankRoot = _rankRegisterAsset.Instantiate();
        rankRoot = rankRoot.Q("RankContainer");
        _contentParent.Add(rankRoot);
        _windowDictionary.Add(Windows.Rank, new RankRegister(rankRoot));
        
        VisualElement rankingRoot = _rankingAsset.Instantiate();
        rankingRoot = rankingRoot.Q("rankingContainer");
        _contentParent.Add(rankingRoot);
        _windowDictionary.Add(Windows.Ranking, new Ranking(rankingRoot, _rankItem));
        
        #endregion
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            int idx = Random.Range(0, _itemList.Count);
            var inventoryUI = _windowDictionary[Windows.Inventory] as InventoryUI;
            inventoryUI.AddItem(_itemList[idx], 3);
        }
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

    private void OnOpenInventoryHandle(ClickEvent evt)
    {
        foreach (var pair in _windowDictionary)
        {
            if (pair.Key == Windows.Inventory)
            {
                pair.Value.Open();
                continue;
            }
            
            pair.Value.Close();
        }
    }

    public void SetUserInfo(UserVO user)
    {
        _loginBtn.style.display = DisplayStyle.None;
        _userInfoPanel.Show(true);
        _userInfoPanel.User = user;
    }

    public void OnLogoutHandle(ClickEvent e)
    {
        GameManager.Instance.DistroyToken();
        _loginBtn.style.display = DisplayStyle.Flex;
        _userInfoPanel.Show(false);
    }

    public void GameOver()
    {
        _windowDictionary[Windows.Rank].Open();
        RankRegister rank = _windowDictionary[Windows.Rank] as RankRegister;
        rank.Score = TimeController.Instance.Time;
        rank.userName = _userInfoPanel.User.name;
    }
}
