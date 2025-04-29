extends Node 

var hand
var deck
var stats

func ApplyEffect(_target): # Forget(burn) half of the hand and the deck
	hand = GlobalAccessPoint.GetHand()
	deck = GlobalAccessPoint.GetDeck()

	#get half of the hand randomly
	var hand_size = hand.GetHandSize()

	#RemoveCard function take index and return the card
	var cards_to_discard = [] #index of the cards to discard cannot be the same
	for i in hand_size:
		#flip a coin to decide if the card is discarded or not
		if randi() % 2 == 0:
			cards_to_discard.append(i)

	for i in cards_to_discard:
		var card = hand.RemoveCard(i)
		if card != null:
			card.BurnCard()

	#get half of the deck randomly
	var deck_size = deck.GetDeckSize()

	#RemoveCard function take index and return the card

	cards_to_discard = [] #index of the cards to discard cannot be the same
	for i in deck_size:
		#flip a coin to decide if the card is discarded or not
		if randi() % 2 == 0:
			cards_to_discard.append(i)

	for i in cards_to_discard:
		deck.RemoveCard(i)

	# get the stats of the player
	stats = GlobalVariables.GetAllCharaterStat()
	for i in stats:
		var health = i.GetHealth()
		i.SetHealth(health / 2)

	get_parent().EffectFinished()
