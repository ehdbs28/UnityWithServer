using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RankRegister : WindowUI
{
    private Label _scoreLabel;
    private TextField _memoInput;

    public string userName;

    private int _score;
    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            _scoreLabel.text = $"점수: {_score.ToString("D3")}";
        }
    }

    public RankRegister(VisualElement root) : base(root)
    {
        _scoreLabel = root.Q<Label>("scoreLabel");
        _memoInput = root.Q<TextField>("memoInput");
        
        root.Q<Button>("registerBtn").RegisterCallback<ClickEvent>(e =>
        {
            saveToDB();
            Close();
        });
        
        root.Q<Button>("cancleBtn").RegisterCallback<ClickEvent>(e =>
        {
            Close();
        });
    }

    private void saveToDB()
    {
        RankVO vo = new RankVO { user_name = userName, score = _score, memo = _memoInput.text };
        NetworkManager.Instance.PostRequest("rank", vo, (type, msg) =>
        {
            if (type == MessageType.ERROR)
            {
                UIController.Instance.MessageSystem.AddMessage(msg, 3f);
            }
        });
    }
}
