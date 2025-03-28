extends Node

var hand
var amount:int = 1

func ApplyEffect(_target):
	hand = GlobalAccessPoint.GetHand()
	amount = hand.GetHandSize()
	hand.ShuffleHandtoDeck()	
	hand.drawFromDeck(amount)
	get_parent().EffectFinished()
