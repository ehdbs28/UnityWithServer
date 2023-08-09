using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MessageTemplete
{
    private VisualElement _root;
    public VisualElement Root => _root;

    private Label _label;
    
    private float _timer = 0f;
    private float _currentTime = 0f;

    private bool _fade = false;
    private bool _isComplete = false;
    public bool IsComplete => _isComplete;

    public string Text
    {
        get => _label.text;
        set => _label.text = value;
    }

    public MessageTemplete(VisualElement root, float timer)
    {
        _root = root;
        _label = root.Q<Label>("message");
        
        _timer = timer;
        _currentTime = 0f;

        _fade = false;
        _isComplete = false;
    }

    public void UpdateMessage()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime >= _timer && !_fade)
        {
            _root.AddToClassList("off");
            _fade = true;
        }
        
        // + transition
        if (_currentTime >= _timer + 0.6f)
        {
            _isComplete = true;
        }
    }
}
