extends Node2D

var tileMap:TileMapLayer

func _ready():
	tileMap = $TileMapLayer
	#generate_map()


func _input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT and event.pressed:
			var click = event.position
			var _pos_clicked = tileMap.local_to_map(to_local(click))
			# highlight the clicked tile
			#highlight_tile(_pos_clicked)
			

func highlight_tile(pos: Vector2i) -> void:
	var tile = tileMap.map_to_local(pos)
	if tile == -1:
		return

	tileMap.set_tile(pos, tile + 1)
