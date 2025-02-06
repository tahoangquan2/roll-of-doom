extends Node

@export var DiscardAmount: int = 1
@export var DrawAmount: int = 2
var hand

func ApplyEffect(_target):
	hand = GlobalAccessPoint.GetHand()

	if hand.startDiscard(DiscardAmount): # If discard selection starts successfully
		hand.connect("SelectionCompleted", Callable(self, "_on_discard_complete"))
		hand.connect("SelectionCancelled", Callable(self, "_on_discard_cancel"))

func _on_discard_complete(_selectedCards):
	hand.drawFromDeck(DrawAmount)
	s_disconnect()
	get_parent().KillCard()

func _on_discard_cancel():
	s_disconnect()
	hand.AddCard(get_parent())

func s_disconnect():
	hand.disconnect("SelectionCompleted", Callable(self, "_on_discard_complete"))
	hand.disconnect("SelectionCancelled", Callable(self, "_on_discard_cancel"))