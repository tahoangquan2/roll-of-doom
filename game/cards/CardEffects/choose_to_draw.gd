extends Node

@export var fishAmout: int = 3
var cardManager
var deck
var hand
var selectingScreen:PackedScene = preload("res://game/gui/middle_screen_selection.tscn")
var cardList:Array = []

func ApplyEffect(_target):
	deck = GlobalAccessPoint.GetDeck()
	cardManager = GlobalAccessPoint.GetCardManager()
	hand = GlobalAccessPoint.GetHand()

	var cards = deck.GetRandomCard(fishAmout)

	#list of cards to be created
	for card in cards:
		cardList.append(cardManager.createCard(card))

	var instance = selectingScreen.instantiate()

	var grandParent = cardManager.get_parent()

	grandParent.add_child(instance)
	grandParent.move_child(instance, 1)
	instance.set_position(Vector2(0, 0))
	
	instance.InitializeSelection(cardList, Vector2(138, 210), Callable(self, "_on_card_selected"),"Control/TextureRect")	

func _on_card_selected(index):
	hand.AddCard(cardList[index])

	#remove the card from the list
	cardList.remove_at(index)
	print(cardList)
	deck.ShuffleCardIntoDeck(cardList)

	get_parent().KillCard()