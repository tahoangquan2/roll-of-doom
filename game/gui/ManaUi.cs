using Godot;
using System;

public partial class ManaUi : HBoxContainer
{
	private const int sizeOfContainer = 32;
	private Label manaLabel => GetNode<Label>("BasicMana/ManaLabel");
	private Label spellManaLabel => GetNode<Label>("SpellMana/ManaLabel");

	private TextureRect manaIcon => GetNode<TextureRect>("BasicMana/Mana");
	private TextureRect spellManaIcon => GetNode<TextureRect>("SpellMana/Mana");

	private Vector2 manaPosition;
	private Vector2 spellManaPosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		manaPosition = manaIcon.Position;
		spellManaPosition = spellManaIcon.Position;
		setMana(3);
		setSpellMana(2);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void setMana(int value){		
		manaLabel.Text = value.ToString();
		value = Mathf.Clamp(value, 0, 6);
		manaIcon.Position = manaPosition-new Vector2(0, sizeOfContainer*(value-1));		
		manaIcon.SetSize(new Vector2(sizeOfContainer, sizeOfContainer*value));
	}

	public void setSpellMana(int value){
		spellManaLabel.Text = value.ToString();
		value = Mathf.Clamp(value, 0, 4);

		spellManaIcon.Position = spellManaPosition-new Vector2(0, sizeOfContainer*(value-1));
		spellManaIcon.SetSize(new Vector2(sizeOfContainer, sizeOfContainer*value));
	}
}
