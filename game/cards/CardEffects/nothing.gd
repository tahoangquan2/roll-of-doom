extends Node 

var hand
var deck

func ApplyEffect(_target): # Forget(burn) half of the hand and the deck
	hand = GlobalAccessPoint.GetHand()
	deck = GlobalAccessPoint.GetDeck()
	get_parent().EffectFinished()
