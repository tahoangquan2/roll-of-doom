extends Node

var hand
var deck

func ApplyEffect(_target):
	hand = GlobalAccessPoint.GetHand()
	deck = GlobalAccessPoint.GetDeck()

	hand.DrawFromDeckSimple(3)

	get_parent().EffectFinished()
