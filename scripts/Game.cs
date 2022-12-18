using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Game : Node
{
	PackedScene cardScene;
	PackedScene cardBaseScene;
	List<Card> hand = new List<Card>();
	List<Card> basedCards = new List<Card>();
	List<Card> enemyBasedCards = new List<Card>();
	List<CardBase> bases = new List<CardBase>();
	List<CardBase> enemyBases = new List<CardBase>();
	Button goButton;
	Card focusedCard = null;
	[Export]
	float closeEnough = 100;

	void focusCard(Card card){
		if(focusedCard != null) dropCard(focusedCard);
		if(focusedCard == card) {
			focusedCard = null;
		}
		else{
			focusedCard = card;
			if(basedCards.Contains(card)){
				var idx = basedCards.IndexOf(card);
				basedCards[idx] = null;
			}
		}
	}

	void dropCard(Card card){
		CardBase cardBase = onCardBase();
		if(cardBase == null) return;
		int idx = bases.IndexOf(cardBase);
		basedCards[idx] = card;
		card.Position = cardBase.Position;
	}

	CardBase onCardBase(){
		if(focusedCard == null) return null; // yay for null checks everywhere
		// int closeIdx = 0;
		CardBase closest = null;
		float dis = closeEnough * 2;
		foreach (var cardBase in bases)
		{
			float tempDis = cardBase.Position.DistanceTo(focusedCard.Position);
			if (tempDis < dis){
				closest = cardBase;
				dis = tempDis;
			}
		}
		if(dis > closeEnough) return null;
		return closest;
	}

	public override void _Ready()
	{
		// Instantiate and place some cards
		cardScene = ResourceLoader.Load("./entities/Card.tscn") as PackedScene;
		cardBaseScene = ResourceLoader.Load("./entities/CardBase.tscn") as PackedScene;
		goButton = GetChild(0) as Button;
		goButton.Disabled = true;
		
		for (int i = 0; i < 4; i++)
		{
			var newCardBase = cardBaseScene.Instance<CardBase>();
			AddChild(newCardBase);
			bases.Add(newCardBase);
			basedCards.Add(null);
			newCardBase.Position = new Vector2(100 + i * 150, 540);
			newCardBase = cardBaseScene.Instance<CardBase>();
			AddChild(newCardBase);
			newCardBase.Position = new Vector2(1920 - 100 - i * 150, 540);
			enemyBases.Add(newCardBase);
			var newCard = cardScene.Instance<Card>();
			AddChild(newCard);
			newCard.type = (CardType)(Math.Floor(GD.Randf() * 3));
			newCard.Position = newCardBase.Position;
			newCard.clickable = false;
		}

		for (int i = 0; i < 6; i++)
		{
			var newCard = cardScene.Instance<Card>();
			hand.Add(newCard);
			AddChild(newCard);
			newCard.type = (CardType)(Math.Floor(GD.Randf() * 3));
			newCard.Position = new Vector2(100 + i * 100, 900);
			newCard.onClick = (Card card) => focusCard(card);
		}
	}
	public override void _Process(float delta){
		goButton.Disabled = basedCards.Any((card) => card == null);
		foreach (var cardBase in bases)
		{
			cardBase.highlighted = false;
		}
		if(focusedCard != null){
			focusedCard.Position = GetViewport().GetMousePosition();
			CardBase cardBase = onCardBase();
			if(cardBase != null){
				cardBase.highlighted = true;
			}
		}
	}
	private void _on_Button_button_down()
	{
		// Replace with function body.
	}
}

