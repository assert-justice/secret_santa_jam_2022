using Godot;
using System;

public class CardBase : Node2D {
    Sprite highlightedSprite;
    bool _highlighted = false;
    [Export]
    public bool highlighted{
        get {return _highlighted;}
        set {
            _highlighted = value;
            highlightedSprite.Visible = _highlighted;
        }
    }

    public override void _Ready()
    {
        highlightedSprite = GetChild(1) as Sprite;
        highlighted = false;
    }
}