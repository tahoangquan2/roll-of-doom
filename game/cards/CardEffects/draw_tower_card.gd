extends Node

var hand
var deck

func ApplyEffect(_target):
	hand = GlobalAccessPoint.GetHand()
	deck = GlobalAccessPoint.GetDeck()

	var ind = deck.GetCardIndexFromType(0)

	if ind != -1:
		hand.drawFromDeckwithIndex(ind)

	get_parent().EffectFinished()
