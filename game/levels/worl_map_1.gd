extends Node2D

var tileMap:TileMapLayer
var camera: Camera2D
var dragging: bool = false
var last_mouse_position: Vector2
var zoom_limit: Vector2 = Vector2(0.5, 2.0)

func _ready():
	tileMap = $TileMapLayer
	camera = $Camera2D


func _input(event: InputEvent) -> void:
	if event is InputEventMouseMotion:
			if dragging:
				var delta = event.position - last_mouse_position
				camera.position -= delta  # Move camera
				last_mouse_position = event.position
	
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT and event.pressed:
			var click = event.position
			var _pos_clicked = tileMap.local_to_map(to_local(click))
			# highlight the clicked tile
			#highlight_tile(_pos_clicked)
		#scroll for zoom
		if event.button_index == MOUSE_BUTTON_WHEEL_DOWN:
			if camera.zoom.x*0.97 > zoom_limit.x:
				camera.zoom = camera.zoom*0.97
		if event.button_index == MOUSE_BUTTON_WHEEL_UP:
			if camera.zoom.x*1.03 < zoom_limit.y:
				camera.zoom = camera.zoom*1.03		
		

		if event.button_index == MOUSE_BUTTON_LEFT:
			if event.pressed:
				dragging = true
				last_mouse_position = event.position
			else:
				dragging = false
			

func highlight_tile(pos: Vector2i) -> void:
	var tile = tileMap.map_to_local(pos)
	if tile == -1:
		return

	tileMap.set_tile(pos, tile + 1)
