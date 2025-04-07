extends Node

@export var DiscardAmount: int = 1
@export var DrawAmount: int = 2
var hand

func ApplyEffect(_target):
	hand = GlobalAccessPoint.GetHand()

	if hand.startDiscard(DiscardAmount): # If discard selection starts successfully
		hand.connect("ActionCompleted", Callable(self, "_on_discard_complete"))
		hand.connect("ActionCancelled", Callable(self, "_on_discard_cancel"))

func _on_discard_complete(_selectedCards):
	hand.drawFromDeck(DrawAmount)
	s_disconnect()
	print("Discarded cards: ", _selectedCards)
	get_parent().EffectFinished()
	
func _on_discard_cancel():
	s_disconnect()
	hand.AddCard(get_parent())

func s_disconnect():
	hand.disconnect("ActionCompleted", Callable(self, "_on_discard_complete"))
	hand.disconnect("ActionCancelled", Callable(self, "_on_discard_cancel"))
