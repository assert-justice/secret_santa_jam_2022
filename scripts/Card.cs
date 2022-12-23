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
	[Export]
	public bool clickable = true;
	AnimatedSprite cardBack;
	RichTextLabel titleLabel;
	RichTextLabel textLabel;
	void SetCardLabels(){
		cardBack.Frame = (int) _type;
		if(cardBack.Frame == 2) cardBack.Frame++;
		titleLabel.Clear();
		textLabel.Clear();
		titleLabel.AddText(_title);
		textLabel.AddText(_type.ToString());
		textLabel.AddText(" ");
		textLabel.AddText(_value.ToString());
	}
	public Action<Card> onClick;
	Vector2 lastPos = new Vector2();
	Vector2 nextPos = new Vector2();
	float animClock = 0;
	float animTime = 0;
	float animCurve = 0;
	public Action<Card> animComplete;
	public Action<Card> animStart;
	public void Animate(Vector2 nextPos, float time, float curve = 1.0f){
		animTime = time;
		animClock = time;
		this.nextPos = nextPos;
		lastPos = Position;
		animCurve = curve;
		animStart(this);
	}
	public void AnimateSpeed(Vector2 nextPos, float speed, float curve = 1.0f){
		float time = Position.DistanceTo(nextPos) / speed;
		Animate(nextPos, time, curve);
	}
	public override void _Ready()
	{
		cardBack = GetChild(0) as AnimatedSprite;
		var textContainer = GetChild(1);
		titleLabel = textContainer.GetChild(0) as RichTextLabel;
		textLabel = textContainer.GetChild(1) as RichTextLabel;
	}
	public override void _Process(float delta)
	{
		if(animClock > 0){
			animClock -= delta;
			animClock = Math.Max(animClock, 0);
			float weight = Mathf.Ease(1.0f - animClock / animTime, animCurve);
			Position = lastPos.LinearInterpolate(nextPos, weight);
			if(animClock == 0 && animComplete != null) animComplete(this);
		}
	}
	private void _on_TextContainer_gui_input(object @event)
	{
		if(!clickable || onClick == null || animClock > 0) return;
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed){
			if (mouseEvent.ButtonIndex == 1) this.onClick(this);
		}
	}
}
