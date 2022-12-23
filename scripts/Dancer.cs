using Godot;
using System;
using System.Collections.Generic;

public enum StatusEffect{
	Inked,
	Dazzled,
	Weakened,
}

public class Dancer : Control
{
	string _name = "[no name assigned]";
	[Export]
	public string name {
		get{return _name;}
		set{_name = value; SetText();}
	}
	int _health = 10;
	[Export]
	public int health {
		get{return _health;}
		set{_health = value; SetText();}
	}
	int _maxHealth = 10;
	[Export]
	public int maxHealth {
		get{return _maxHealth;}
		set{_maxHealth = value; SetText();}
	}
	public void Flip(bool val){
		GetChild(0).GetChild<AnimatedSprite>(0).FlipH = val;
	}
	public void SetSprite(string fPath){
		GetChild(0).GetChild<AnimatedSprite>(0).Frames = ResourceLoader.Load<SpriteFrames>(fPath);
	}
	Dictionary<StatusEffect,int> statuses = new Dictionary<StatusEffect, int>();
	RichTextLabel textBox;
	public void AddStatus(StatusEffect status, int severity){
		if(!statuses.ContainsKey(status)) statuses.Add(status, 0);
		statuses[status] += severity;
		// if(statuses.ContainsKey(status)) val += statuses[status];
		// statuses.Add(status, val);
		SetText();
	}
	void SetText(){ 
		textBox.Clear();
		textBox.AddText(name);
		textBox.AddText("\nHealth: ");
		textBox.AddText(health.ToString());
		textBox.AddText("/");
		textBox.AddText(maxHealth.ToString());
		// textBox.AddText("\nStatus: ");
		// foreach (KeyValuePair<StatusEffect, int> elem in statuses)
		// {
		// 	textBox.AddText(elem.Key.ToString());
		// 	textBox.AddText("(x" + elem.Value.ToString() + ")\n");
		// }
	}
	public override void _Ready()
	{
		statuses.Clear();
		textBox = GetChild(0).GetChild<RichTextLabel>(1);
	}

	public override void _Process(float delta)
	{
		//
	}
}
