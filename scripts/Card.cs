using Godot;
using System;

public enum CardType
{
	Jump,
	Spin,
	Kick
}
public struct CardData
{
	string title;
	float value;
	string text;
}
public class Card : Node2D
{
	string _title = "Title";
	[Export]
	public String title {
		get {return _title;}
		set {
			_title = value;
			SetCardLabels();
		}
	}
	CardType _type = CardType.Jump;
	[Export]
	public CardType type {
		get {return _type;}
		set {
			_type = value;
			SetCardLabels();
		}
	}
	float _value = 10;
	[Export]
	public float value {
		get {return _value;}
		set {
			_value = value;
			SetCardLabels();
		}
	}
	AnimatedSprite cardBack;
	RichTextLabel titleLabel;
	RichTextLabel textLabel;
	void SetCardLabels(){
		cardBack.Frame = (int) _type;
		titleLabel.Clear();
		textLabel.Clear();
		titleLabel.AddText(_title);
		textLabel.AddText(_type.ToString());
		textLabel.AddText(" ");
		textLabel.AddText(_value.ToString());
	}
	bool dragging = false;
	public override void _Ready()
	{
		cardBack = GetChild(0) as AnimatedSprite;
		var textContainer = GetChild(1);
		titleLabel = textContainer.GetChild(0) as RichTextLabel;
		textLabel = textContainer.GetChild(1) as RichTextLabel;
	}
	public override void _Process(float delta)
	{
		if(dragging){
			var mousePos = GetViewport().GetMousePosition();
			this.Position = mousePos;
		}
	}
	// private void _on_Card_input_event(object viewport, object @event, int shape_idx)
	// {
	// 	GD.Print("fttotott");
	// }
	private void _on_TextContainer_gui_input(object @event)
	{
		// Replace with function body.
		// GD.Print("yo");
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed){
			if (mouseEvent.ButtonIndex == 1) dragging = !dragging;
		}
	}
}
