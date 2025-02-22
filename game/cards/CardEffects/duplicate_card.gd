extends Node

@export var DuplicateAmount: int = 3
var hand
var deck

func ApplyEffect(_target):
	hand = GlobalAccessPoint.GetHand()
	deck = GlobalAccessPoint.GetDeck()

	if hand.StartSelectionMode(1,1): # If discard selection starts successfully
		hand.connect("ActionCompleted", Callable(self, "_on_choose_complete"))
		hand.connect("ActionCancelled", Callable(self, "_on_choose_cancel"))
	else:
		hand.AddCard(get_parent())

func _on_choose_complete(_selectedCards):
	s_disconnect()
	for i in range(DuplicateAmount):
		deck.ShuffleIntoDeck(_selectedCards[0].GetCardData())
	get_parent().KillCard()

func _on_choose_cancel():
	s_disconnect()
	hand.AddCard(get_parent())

func s_disconnect():
	hand.disconnect("ActionCompleted", Callable(self, "_on_choose_complete"))
	hand.disconnect("ActionCancelled", Callable(self, "_on_choose_cancel"))