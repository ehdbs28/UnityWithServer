using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WindowUI
{
    protected VisualElement _root;
    
    public WindowUI(VisualElement root)
    {
        _root = root;
    }
    
    public virtual void Open()
    {
        _root.RemoveFromClassList("fade");
    }

    public virtual void Close()
    {
        _root.AddToClassList("fade");
    }
}
