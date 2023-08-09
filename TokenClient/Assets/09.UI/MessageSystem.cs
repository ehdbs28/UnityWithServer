using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MessageSystem : MonoBehaviour
{
    [SerializeField] 
    private VisualTreeAsset _messageTemplete;
    
    private VisualElement _container;

    private List<MessageTemplete> _messageList = new List<MessageTemplete>();

    public void SetContainer(VisualElement container)
    {
        _container = container;
    }

    private void Update()
    {
        for (int i = 0; i < _messageList.Count; i++)
        {
            _messageList[i].UpdateMessage();
            if (_messageList[i].IsComplete)
            {
                _messageList[i].Root.RemoveFromHierarchy();
                _messageList.RemoveAt(i);
                --i;
            }
        }
    }

    public void AddMessage(string text, float timer)
    {
        var messageElement = _messageTemplete.Instantiate().Q<VisualElement>("MessageBox");
        _container.Add(messageElement);

        var msg = new MessageTemplete(messageElement, timer);
        msg.Text = text;
        _messageList.Add(msg);
    }
}
