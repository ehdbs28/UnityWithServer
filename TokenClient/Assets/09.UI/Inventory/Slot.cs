using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Slot
{
    private VisualElement _root;
    public int slotNumber;

    public VisualElement Elem => _root;
    
    public VisualElement Child 
    {
        get => _root.childCount == 0 ? null : _root.Children().First();
    }

    public Slot(VisualElement root, int id)
    {
        _root = root;
        slotNumber = id;
    }
}
