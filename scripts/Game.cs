using Godot;
using System;

public class Game : Node
{
    PackedScene cardScene;
    public override void _Ready()
    {
        // Instantiate and place some cards
        cardScene = ResourceLoader.Load("./entities/Card.tscn") as PackedScene;
        for (int i = 0; i < 4; i++)
        {
            var newCard = cardScene.Instance() as Card;
            AddChild(newCard);
            newCard.type = CardType.Spin;
            newCard.Position = new Vector2(100 + i * 200, 300);
        }
    }
}
