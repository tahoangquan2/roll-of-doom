extends CanvasLayer

var mainGame:PackedScene = preload("res://game/Main.tscn")

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	pass

func _on_quit_button_pressed() -> void: # quit button
	get_tree().quit()	

func _on_continue_button_pressed() -> void:
	#pause for .5 seconds
	await get_tree().create_timer(0.25).timeout
	get_tree().change_scene_to_file("res://game/Main.tscn")

func _on_settings_button_pressed() -> void:
	pass # Replace with function body.

func _on_new_game_button_pressed() -> void:
	pass # Replace with function body.
