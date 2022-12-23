using Godot;
using System.Collections.Generic;

public class Mission{
	public string name = "[no name assigned]";
	public string brief = "";
	public int health = 10;
	public int numBases = 4;
	public int handSize = 6;
	public int numMulligans = 3;
	public string sprPath = "";
	public string exit = "";
	public int playerHealth = 10;
	// public string intro = "[no intro assigned]";
	// public bool cleared = false;
	public Mission(string name, int health, string sprPath = "./characters/nebula.tres", 
	string brief = "", string exit = "", int numBases = 4, int handSize = 2, int numMulligans = 3, int playerHealth = 10){
		this.name = name; this.health = health; this.numBases = numBases; this.handSize = handSize; 
		this.numMulligans = numMulligans; this.sprPath = sprPath;
		this.brief = brief; this.exit = exit; this.playerHealth = playerHealth;
	}
}

/*
Script
[Intro]
In the distant year of 1997 war is history. The Earth is abandoned, the teeming masses of humanity writhe and cavort under a soot stained sky. Nation states have fallen by the wayside replaced by the very megacorps that blackened the firmament and boiled the oceans. Once the lives of workers were thrown away for the glory of tattered rags and vapid ideals. As Napoleon wrote "The Last Argument of Kings" on his cannons there is a new final weapon when politics fails. Now there is only one divider of weak and strong: 

Dance Battles

You are Neptune, a fierce battle dancer given a simple ultimatum: Defeat the six expert battle dancers of Olympus Mons, win the tournament of champions, and be reunited with your family in time for the solstice. The consequences of failure are dire...

[Tutorial]
Before the tournament proper starts you should learn how dance battles are fought. Your partner is Phlox the Unintimidating.

Drag a card from your hand (on the bottom left) to the base (on the middle left). 
When you do you can press the "Go!" button to start the turn.

Each turn is made of some number of rounds equal to the number of bases.
During each round the cards are compared from the middle out. If you win a round the opponent takes a point of damage. If you lose the round you take a point of damage. If you tie no damage is applied.

The cards represent dance moves. The following rules are completely obvious:
Kick beats Jump
Jump beats Spin
Spin beats Kick

If you're not happy with your hand you can use the Mulligan button to exchange the cards in your hand but not on bases! You (usually) get 3 mulligans per bout.

Good luck and on with the show!

[Defeat]
You were vanquished! You are doomed to forever serve your corpocratic masters! You know, like all the rest of us...

Fortunately you can try again.

[Win]
A winner is you!

You are tearfully reunited with your family just in time for synthetic eggnog and roasting space chestnuts. Life is good.

Thank you for playing <3
*/

public class Main : Node
{
	PackedScene gameScene;
	Game currentGame;
	Control gameAnchor;
	[Export]
	public int missionIdx = 0;
	List<Mission> missions = new List<Mission>();
	List<Control> menus = new List<Control>();
	AudioStreamPlayer music;

	void LoadMission(){
		if(currentGame != null){
			currentGame.QueueFree();
		}
		currentGame = gameScene.Instance<Game>();
		currentGame.victory = () => Victory();
		currentGame.defeat = () => Defeat();
		currentGame.LoadMission(missionIdx, missions[missionIdx]);
		// currentGame.actionQueue.Enqueue(() => currentGame.LoadMission(missionIdx, missions[missionIdx]));
		gameAnchor.AddChild(currentGame);
		// set victory text
		var text = menus[1].GetChild(0).GetChild<RichTextLabel>(1);
		text.Clear();
		text.AddText(missions[missionIdx].exit);
	}
	public override void _Ready()
	{
		gameScene = ResourceLoader.Load("res://Game.tscn") as PackedScene;
		gameAnchor = GetChild<Control>(0);
		menus.Add(GetChild<Control>(1));
		menus.Add(GetChild<Control>(2));
		menus.Add(GetChild<Control>(3));
		menus.Add(GetChild<Control>(4));
		foreach (var menu in menus)
		{
			menu.Visible = true;
		}
		ShowMenu(0);
		music = GetChild<AudioStreamPlayer>(5);
		missions.Clear();
		missions.Add(new Mission("Phlox the Unintimidating", 3, "res://characters/phlox.tres",@"Before the tournament proper starts you should learn how dance battles are fought. Your partner is Phlox the Unintimidating.

Drag a card from your hand (on the bottom left) to the base (on the middle left). 
When you do you can press the 'Go!' button to start the turn.

Each turn is made of some number of rounds equal to the number of bases.
During each round the cards are compared from the middle out. If you win a round the opponent takes a point of damage. If you lose the round you take a point of damage. If you tie no damage is applied.

The cards represent dance moves. The following rules are completely obvious:
Kick beats Jump
Jump beats Spin
Spin beats Kick

If you're not happy with your hand you can use the Mulligan button to exchange the cards in your hand but not on bases! You (usually) get 3 mulligans per bout.

Good luck and on with the show!", @"With a glad heart Phlox sees you off. They know your destinies will collide again and have utmost faith in your success.", 1));
		missions.Add(new Mission("Frank the Irascible", 5, "res://characters/frank.tres", @"He isn't called 'The Irascible' without cause you know! 
He swaggers in place, snarling at the crowd and contestants alike.

'They will know my name from Phobos to Nemesis, tremble in fear of Frank!' he says.

A formidable adversary indeed.", @"Frank drops to his knees, not merely defeated but shattered.

'Is this truly what I have become?' he laments 'A mad dog, a rabid possum, an incontinent narwhal!?'

You decide not to point out that it's very rare for possums to carry rabies. You try to comfort him but he runs shrieking about idempotent giraffes. On to the next contestant.", 3, 4));
		missions.Add(new Mission("Zoe, Techno Priest of Phobos", 8, "res://characters/zoe.tres", @"Zoe's mastery of the machine is intimidating, she has stripped away your mulligans. The cards you get are the cards you get.
		
'Why do you fight Neptune?' she asks with a lazy drawl. 'Is it truly to free your family? Or do you fight for the thrill? The adoring crowds, raucous in their adoration? Has that fickle mistress fame seduced your heart?'. She pauses and draws in a deep breath.

'No matter. Your fame ends here.'

It begins.", @"Zoe is dazzled by your moves. Her technical wizardry was no match for your skill.

'What have I become?' she bemoans 'my place is out there, exploring the uncharted oceans of science. Forgive me teachers, I have been lead astray.'

It seems like she's being a little dramatic to you. No matter. Four contestants remain.", 4, 6, 0));
		missions.Add(new Mission("Wunderkammer, Devourer of Souls, Seeker of Mysteries", 10, "res://characters/wund.tres", @"Blessedly Wunderkammer doesn't have much to say. It's three A.M. and I don't know why I decided to add so many ******* characters.", @"Wunderkammer is sad they cannot devour your soul. They'll have to get over it.", 5, 5));
		missions.Add(new Mission("Lucretia the Space Privateer", 8, "res://characters/toby.tres", @"The routes between worlds are long and winding. and unwary ships make for prime targets. Lucretia was one such brigand once. Now she serves the megacorps but she enjoys a good dance battle to blow off steam. Have at you!", @"Lucretia has put up a good fight but is ultimately defeated by your superior dancing prowess. She takes her loss well, with a bow.
		
'I see you are what you claim you are. Best of luck to you young battle dancer.'
		
At least somebody has heard of good sportsmanship.", 4, 8, 1));
		missions.Add(new Mission("Dave the Not So Great", 5, "res://characters/dave.tres", @"Dave is a good dancer but is just kind of a jerk. Nobody likes him. Simply being near him makes you ill.", @"Good riddance.", 4, 6, 1, 5));
		missions.Add(new Mission("Exegesis", 20, "res://characters/exa.tres", @"Exegesis was there at the beginning and will be there at the end. It encompasses all things. It doesn't actually talk though. Seems like false advertizing.", @"Exegesis cannot be truly defeated, it was there when the first rug was cut and it will be there for the last. That said inspired by your moves it withdraws from the contest to its mountain citadel for silent meditation on the nature of rhythm and motion.
	
You have defeated the six champions! Victory is yours! But, emerging from the dust and shadows another figure approaches...", 6, 6, 1));
		missions.Add(new Mission("Phlox the Very Intimidating", 5, "res://characters/phlox.tres", @"'Phlox!' you say 'You would challenge me? Here, at the end of all things? When I am so close to victory?'

'Aye' says Phlox 'Your victory is ill deserved. The game is too easy. Show some semblance of skill!'

They stab you with a dirk. It's totes like that scene from Gladiator. You are reduced to ONE HITPOINT!

'Now, show me what you're made of. Do kicks beat spins? Or jumps!? Who can say.'", @"Phlox staggers back, aghast.
'Not a single hit. Maybe you *do* understand this incredibly deep and involved game. I cannot stand against you. Go! Take your boons and depart this place.'

At last.", 1, 2, 3, 1));

	}
	void HideMenus(){
		foreach (var menu in menus)
		{
			menu.AnchorTop = -1;
			menu.AnchorBottom = 0;
			// menu.Visible = false;
		}
	}
	bool isAMenuVisible(){
		return true;
		foreach (var menu in menus)
		{
			if(menu.Visible) return true;
		}
		return false;
	}
	void ShowMenu(int idx){
		HideMenus();
		var menu = menus[idx];
		// menu.Visible = true;
		menu.AnchorTop = 0;
		menu.AnchorBottom = 1;
	}
	void Victory(){
		if(missionIdx == missions.Count -1){
			// display outro
			ShowMenu(3);
		}
		else{
			// display victory
			ShowMenu(1);
		}
	}
	void Defeat(){
		// show defeat
		ShowMenu(2);
	}
	private void _on_Start_button_down()
	{
		// Replace with function body.
		// missionSelect.AnchorTop = -1;
		// missionSelect.AnchorBottom = 0;
		if(!isAMenuVisible()) return;
		missionIdx = 0;
		HideMenus();
		LoadMission();
	}
	
	private void _on_Quit_button_down()
	{
		if(!isAMenuVisible()) return;
		GetTree().Quit();
	}
	
	private void _on_Next_button_down()
	{
		if(!isAMenuVisible()) return;
		missionIdx++;
		HideMenus();
		LoadMission();
	}
	private void _on_HSlider_drag_ended(bool value_changed)
	{
		// Replace with function body.
		if(!isAMenuVisible()) return;
		var slider = GetChild(1).GetChild(0).GetChild(2).GetChild<HSlider>(3);
		// GD.Print(slider.Value);
		music.VolumeDb = GD.Linear2Db((float)slider.Value);
		// GD.Print(music);
	}
	private void _on_AudioStreamPlayer_finished()
	{
		// restart track
		music.Play();
	}
	private void _on_Retry_button_down()
	{
		// Replace with function body.
		if(!isAMenuVisible()) return;
		HideMenus();
		LoadMission();
	}
}
