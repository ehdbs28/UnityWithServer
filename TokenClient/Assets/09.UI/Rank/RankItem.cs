using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RankItem
{
    private VisualElement _root;

    private Label _rankLabel;
    private Label _nameLabel;
    private Label _scoreLabel;
    private Label _memoLabel;
    
    public RankItem(VisualElement root, string rank, string name, string score, string memo)
    {
        _rankLabel = root.Q<Label>("rankLabel");
        _nameLabel = root.Q<Label>("nameLabel");
        _scoreLabel = root.Q<Label>("scoreLabel");
        _memoLabel = root.Q<Label>("memoLabel");

        _rankLabel.text = rank;
        _nameLabel.text = name;
        _scoreLabel.text = score;
        _memoLabel.text = memo;
    }
}
