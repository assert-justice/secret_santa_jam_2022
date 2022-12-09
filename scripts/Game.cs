using Godot;
using System;
using System.Collections.Generic;

enum GameState{
	PickCard,
	PlaceCard,
	Inactive,
}

public class Game : Node
{
	PackedScene cardScene;
	PackedScene cardBaseScene;
	int hoverIdx = 0;
	int baseIdx = 0;
	Vector2 cardPos = new Vector2();
	Card focusedCard = null;
	List<Card> hand = new List<Card>();
	List<CardBase> bases = new List<CardBase>();

	GameState state = GameState.PickCard;

	void setHoveredCard(int idx)
	{
		// if(idx == )
		while(idx < 0) idx += hand.Count;

		hoverIdx = idx % hand.Count;
		placeHand();
	}
	void setBase(int idx){
		while(idx < 0) idx += bases.Count;

		baseIdx = idx % bases.Count;
		placeCard();
	}
	void placeHand()
	{
		for (int i = 0; i < hand.Count; i++)
		{
			var y = 900;
			if(i == hoverIdx) y -= 50;
			var card = hand[i];
			if(card == focusedCard) continue;
			card.Position = new Vector2(100 + i * 50, y);
		}
	}
	void placeCard(){
		cardPos = bases[baseIdx].Position;
		cardPos.y += 100;
		if(focusedCard != null) focusedCard.Position = cardPos;
	}
	public override void _Ready()
	{
		// Instantiate and place some cards
		cardScene = ResourceLoader.Load("./entities/Card.tscn") as PackedScene;
		cardBaseScene = ResourceLoader.Load("./entities/CardBase.tscn") as PackedScene;
		
		for (int i = 0; i < 4; i++)
		{
			var newCardBase = cardBaseScene.Instance<CardBase>();
			AddChild(newCardBase);
			bases.Add(newCardBase);
			newCardBase.Position = new Vector2(100 + i * 150, 540);
			newCardBase = cardBaseScene.Instance<CardBase>();
			AddChild(newCardBase);
			newCardBase.Position = new Vector2(1920 - 100 - i * 150, 540);
		}

		for (int i = 0; i < 6; i++)
		{
			var newCard = cardScene.Instance<Card>();
			hand.Add(newCard);
			AddChild(newCard);
			newCard.type = (CardType)(Math.Floor(GD.Randf() * 3));
			// newCard.Position = new Vector2(100 + i * 50, 900);
		}
		placeHand();
	}
	void setState(GameState newState){
		if(state == newState) return;
		switch (state)
		{
			case GameState.PickCard:
				if(newState == GameState.PlaceCard){
					focusedCard = hand[hoverIdx];
				}
				// hoverIdx = -1;
				placeCard();
				// baseIdx = 0;
				break;
			case GameState.PlaceCard:
				focusedCard = null;
				placeHand();
				break;
			default:
			break;
		}
		state = newState;
	}
	public override void _Process(float delta)
	{
		switch (state)
		{
			case GameState.PickCard:
				if (Input.IsActionJustPressed("ui_left"))setHoveredCard(hoverIdx - 1);
				if (Input.IsActionJustPressed("ui_right"))setHoveredCard(hoverIdx + 1);
				if (Input.IsActionJustPressed("ui_accept"))setState(GameState.PlaceCard);
				if (Input.IsActionJustPressed("ui_cancel"))setState(GameState.PlaceCard);
				
				break;
			case GameState.PlaceCard:
				if (Input.IsActionJustPressed("ui_left"))setBase(baseIdx - 1);
				if (Input.IsActionJustPressed("ui_right"))setBase(baseIdx + 1);
				// if (Input.IsActionJustPressed("ui_accept"))setState(GameState.PlaceCard);
				if (Input.IsActionJustPressed("ui_cancel"))setState(GameState.PickCard);
				break;
			default:
				// Inactive
			break;
		}
	}
}
