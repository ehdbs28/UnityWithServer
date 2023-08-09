using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LunchUI : WindowUI
{
    private TextField _dateTextField;
    private Label _lunchLabel;
    
    public LunchUI(VisualElement root) : base(root)
    {
        _dateTextField = root.Q<TextField>("DateTextField");
        _lunchLabel = root.Q<Label>("LunchLabel");
        
        root.Q<Button>("LoadBtn").RegisterCallback<ClickEvent>(OnLoadBtnHandle);
        root.Q<Button>("CloseBtn").RegisterCallback<ClickEvent>(OnCloseBtnHandle);
    }

    private void OnLoadBtnHandle(ClickEvent evt)
    {
        string dateStr = _dateTextField.value;
        Regex regex = new Regex(@"20[0-9]{2}[0-1][0-9][0-3][0-9]");
        if(!regex.IsMatch(dateStr))
        {
            UIController.Instance.MessageSystem.AddMessage("올바른 날짜 형식을 입력하세요. (ex. 20230704)", 3f);
            return;
        }
    
        NetworkManager.Instance.GetRequest("lunch", $"?date={dateStr}", (type, json) =>
        {
            if (type == MessageType.SUCCESS)
            {
                LunchVO vo = JsonUtility.FromJson<LunchVO>(json);
                string menuStr = vo.menus.Aggregate("", (sum, x) => sum + x + "\n");
                _lunchLabel.text = menuStr;
            }
            else
            {
                Debug.LogError(json);
            }
        });
    }

    private void OnCloseBtnHandle(ClickEvent evt)
    {
        Close();
    }
}
