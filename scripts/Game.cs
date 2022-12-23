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
	Button mulButton;
	Dancer player;
	Dancer enemy;
	// Control victory;
	// Control defeat;
	public Action victory;
	public Action defeat;
	int gameStatus = 0;
	public int mulligans = 3;
	HashSet<Card> pending = new HashSet<Card>();
	Card focusedCard = null;
	[Export]
	float closeEnough = 100;
	public int handSize = 6;
	public int numBases = 4;
	public Queue<Action> actionQueue = new Queue<Action>();

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
		card.AnimateSpeed(cardBase.Position, 300f, 4.8f);
		pending.Add(card);
	}

	CardBase onCardBase(){
		if(focusedCard == null) return null; // yay for null checks everywhere
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

	void Mulligan(){
		actionQueue.Enqueue(() => {
			foreach (var card in hand)
			{
				if(basedCards.Contains(card)) continue;
				card.Animate(new Vector2(-200, card.Position.y), 0.3f);
			}
		});
		actionQueue.Enqueue(() => {DrawCards();});
	}

	void SetButtonText(){
		mulButton.Text = "Mulligans: " + mulligans.ToString();
	}

	void Go(){
		// run the round
		// for each pair of cards
		for (int i = 0; i < numBases; i++)
		{
			var playerCard = basedCards[numBases - 1 - i];
			var enemyCard = enemyBasedCards[numBases - 1 - i];
			int status = 0; // status tracks the winner
			if(playerCard.type == enemyCard.type) status = 0;
			else if(playerCard.type == CardType.Spin && enemyCard.type == CardType.Kick) status = 1;
			else if(playerCard.type == CardType.Kick && enemyCard.type == CardType.Jump) status = 1;
			else if(playerCard.type == CardType.Jump && enemyCard.type == CardType.Spin) status = 1;
			else status = 2;
			actionQueue.Enqueue(()=>{
				// hold up two cards
				playerCard.Animate(new Vector2(playerCard.Position.x, playerCard.Position.y - 150), 0.3f);
				enemyCard.Animate(new Vector2(enemyCard.Position.x, enemyCard.Position.y - 150), 0.3f);
			});
			actionQueue.Enqueue(()=>{
				// collide the cards
				playerCard.Animate(new Vector2(960, playerCard.Position.y), 0.3f);
				enemyCard.Animate(new Vector2(960, enemyCard.Position.y), 0.3f);
			});
			actionQueue.Enqueue(()=>{
				// apply damage and status effectspending.Count > 0 ||
				if(status != 1){
					playerCard.Animate(new Vector2(-200, -200), 0.8f);
				}
				if(status != 2){
					enemyCard.Animate(new Vector2(2120, -200), 0.8f);
				}
				if(status == 1){
					enemy.health--;
					// check for victory
					if(enemy.health == 0){
						gameStatus = 1;
						victory();
					}
				}
				if (status == 2){
					player.health--;
					// check for defeat
					if(player.health == 0){
						gameStatus = 2;
						defeat();
					}
				}
			});
			actionQueue.Enqueue(()=>{
				// cleanup
				playerCard.Animate(new Vector2(960, -200), 0.3f);
				enemyCard.Animate(new Vector2(960, -200), 0.3f);
			});
		}
		actionQueue.Enqueue(() => {
			foreach (var card in hand)
			{
				if(basedCards.Contains(card)) continue;
				card.Animate(new Vector2(-200, card.Position.y), 0.3f);
			}
			for (int i = 0; i < basedCards.Count; i++)
			{
				basedCards[i] = null;
			}
		});
		actionQueue.Enqueue(()=>{
			// begin new round
			foreach (var card in hand)
			{
				card.QueueFree();
			}
			hand.Clear();
			foreach (var card in enemyBasedCards)
			{
				card.QueueFree();
			}
			BeginRound();
		});
	}

	Card InitCard(CardType type){
		var newCard = cardScene.Instance<Card>();
		AddChild(newCard);
		newCard.type = type;
		newCard.onClick = (Card card) => focusCard(card);
		newCard.animStart = (Card card) => pending.Add(card);
		newCard.animComplete = (Card card) => pending.Remove(card);
		return newCard;
	}

	void DrawCards(){
		foreach (var card in hand)
		{
			if(!basedCards.Contains(card)) card.QueueFree();
		}
		hand.Clear();
		foreach (var card in basedCards)
		{
			if(card == null) continue;
			hand.Add(card);
		}
		for (int i = 0; i < handSize - basedCards.Where(card => card!=null).Count(); i++)
		{
			var type = (CardType)(Math.Floor(GD.Randf() * 3));
			var newCard = InitCard(type);
			newCard.Position = new Vector2(-200, 800);
			newCard.AnimateSpeed(new Vector2(100 + i * 100, 800), 600);
			hand.Add(newCard);
		}
	}

	void DrawEnemyCards(){
		enemyBasedCards.Clear();
		foreach (var cardBase in enemyBases)
		{
			var type = (CardType)(Math.Floor(GD.Randf() * 3));
			var newCard = InitCard(type);
			newCard.Position = new Vector2(2000, cardBase.Position.y);
			newCard.AnimateSpeed(cardBase.Position, 600);
			newCard.clickable = false;
			enemyBasedCards.Add(newCard);
		}
	}

	void BeginRound(){
		actionQueue.Enqueue(() => {
			DrawCards();
			DrawEnemyCards();
			SetButtonText();
		});
		
	}

	public void LoadMission(int missionIdx, Mission mission){
		numBases = mission.numBases;
		handSize = mission.handSize;
		mulligans = mission.numMulligans;
		actionQueue.Enqueue(() => {
			player.name = "Neptune";
			player.health = mission.playerHealth;
			enemy.name = mission.name;
			enemy.health = mission.health;
			enemy.maxHealth = mission.health;
			enemy.SetSprite(mission.sprPath);

			var text = GetChild<RichTextLabel>(4);
			text.Clear();
			text.AddText(mission.brief);
		});
	}

	public override void _Ready()
	{
		// Instantiate and place some cards
		GD.Randomize();
		cardScene = ResourceLoader.Load("res://entities/Card.tscn") as PackedScene;
		cardBaseScene = ResourceLoader.Load("res://entities/CardBase.tscn") as PackedScene;
		goButton = GetChild<Button>(0);
		mulButton = GetChild<Button>(1);
		player = GetChild<Dancer>(2);
		// player.health = 1;
		enemy = GetChild<Dancer>(3);
		enemy.Flip(true);
		goButton.Disabled = true;
		
		for (int i = 0; i < numBases; i++)
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
		}

		BeginRound();

		// DrawCards();
		// DrawEnemyCards();
		// SetButtonText();
	}
	public override void _Process(float delta){
		if(gameStatus != 0) return;
		if(pending.Count == 0 && actionQueue.Count > 0){
			actionQueue.Dequeue()();
		}
		goButton.Disabled = pending.Count > 0 || basedCards.Any((card) => card == null);
		mulButton.Disabled = pending.Count > 0 || mulligans == 0;
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
		Go();
	}
	private void _on_Mulligan_button_down()
	{
		Mulligan();
		mulligans--;
		SetButtonText();
	}
}
