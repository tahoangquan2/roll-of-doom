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

	hand.get_parent().add_child(instance)
	
	instance.InitializeSelection(cardList, Vector2(138, 210), Callable(self, "_on_card_selected"))	

func _on_card_selected(index):
	hand.AddCard(cardList[index])

	#remove the card from the list
	cardList.remove_at(index)
	print(cardList)
	deck.ShuffleCardIntoDeck(cardList)

	get_parent().KillCard()