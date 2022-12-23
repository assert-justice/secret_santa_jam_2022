using Godot;
using System;

public class Menu : Node2D{
    Vector2 lastPos = new Vector2();
	Vector2 nextPos = new Vector2();
	float animClock = 0;
	float animTime = 0;
	float animCurve = 0;
	public Action<Card> animComplete;
	public Action<Card> animStart;
}