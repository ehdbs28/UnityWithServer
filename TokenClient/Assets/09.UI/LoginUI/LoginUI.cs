using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoginUI : WindowUI
{
    private TextField _emailField;
    private TextField _passwordField;
    
    public LoginUI(VisualElement root) : base(root)
    {
        _emailField = root.Q<TextField>("EmailInput");
        _passwordField = root.Q<TextField>("PasswordInput");
        
        root.Q<Button>("OkBtn").RegisterCallback<ClickEvent>(OnLoginBtnHandle);
        root.Q<Button>("CancleBtn").RegisterCallback<ClickEvent>(OnCancleBtnHandle);
    }

    private void OnLoginBtnHandle(ClickEvent evt)
    {
        // 입력 값 검증이 있어야 함
        LoginDTO loginDto = new LoginDTO
        {
            email = _emailField.value,
            password = _passwordField.value
        };
        
        NetworkManager.Instance.PostRequest("user/login", loginDto, (type, json) =>
        {
            Debug.Log(type);
            Debug.Log(json);
        });
    }
    
    private void OnCancleBtnHandle(ClickEvent evt)
    {
        Close();
    }
}
