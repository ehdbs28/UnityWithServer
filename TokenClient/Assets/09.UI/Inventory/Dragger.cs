using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Dragger : MouseManipulator
{
    private Action<MouseUpEvent, VisualElement, VisualElement> _dropCallback;

    private bool _isDrag = false;
    private Vector2 _startPos;
    private VisualElement _beforeSlot;

    public Dragger(Action<MouseUpEvent, VisualElement, VisualElement> DropCallback)
    {
        activators.Add(new ManipulatorActivationFilter{button = MouseButton.LeftMouse});
        _dropCallback = DropCallback;
    }
    
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected void OnMouseDown(MouseDownEvent e)
    {
        if (CanStartManipulation(e))
        {
            var x = target.layout.x;
            var y = target.layout.y;
            _beforeSlot = target.parent;
            var container = target.parent.parent;
            
            target.RemoveFromHierarchy();
            container.Add(target);
            
            _isDrag = true;
            target.CaptureMouse();
            _startPos = e.localMousePosition;

            Vector2 offset = e.mousePosition - container.worldBound.position - _startPos;

            target.style.position = Position.Absolute;
            target.style.left = offset.x;
            target.style.top = offset.y;
        }
    }

    protected void OnMouseMove(MouseMoveEvent e)
    {
        if (!_isDrag || !CanStartManipulation(e) || !target.HasMouseCapture())
            return;

        Vector2 diff = e.localMousePosition - _startPos;
        var x = target.layout.x;
        var y = target.layout.y;

        target.style.left = x + diff.x;
        target.style.top = y + diff.y;
    }

    protected void OnMouseUp(MouseUpEvent e)
    {
        if (!_isDrag || !target.HasMouseCapture())
            return;

        _isDrag = false;
        target.ReleaseMouse();

        target.style.position = Position.Relative;
        target.style.left = 0;
        target.style.top = 0;
        
        target.RemoveFromHierarchy();
        _beforeSlot.Add(target);

        // 이녀석을 다시 relative바꿔주고 DropCallback을 콜
    }
}
