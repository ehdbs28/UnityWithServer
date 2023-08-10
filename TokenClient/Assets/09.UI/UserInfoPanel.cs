using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UserInfoPanel
{
    private VisualElement _root;
    private Button _userBtn;

    public UserPopOver UserPopOver { get; private set; }

    private UserVO _user;
    public UserVO User
    {
        get => _user;
        set
        {
            _user = value;
            _userBtn.text = _user.name;
            UserPopOver.UserName = _user.name;
            UserPopOver.Email = _user.email;
            UserPopOver.Exp = _user.exp;
        }
    }
    
    public UserInfoPanel(VisualElement root, VisualElement popOverElem, EventCallback<ClickEvent> OnLogoutHandle)
    {
        _root = root;
        _userBtn = root.Q<Button>("infoBtn");

        UserPopOver = new UserPopOver(popOverElem);
        
        _userBtn.RegisterCallback<MouseEnterEvent>(e =>
        {
            Rect rect = _userBtn.worldBound;
            Vector2 pos = rect.position;
            pos.y += rect.height;
            UserPopOver.PopOver(pos);
        });
        
        _userBtn.RegisterCallback<MouseLeaveEvent>(e =>
        {
            UserPopOver.Hide();
        });
        
        root.Q<Button>("logoutBtn").RegisterCallback<ClickEvent>(OnLogoutHandle);
    }

    public void Show(bool val)
    {
        if (val)
        {
            _root.RemoveFromClassList("widthzero");
        }
        else
        {
            _root.AddToClassList("widthzero");
        }
    }
}
