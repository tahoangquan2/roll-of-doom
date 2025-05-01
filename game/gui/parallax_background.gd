extends Node2D

#list of layers

var layers = []
var layer_speeds = []
var base_speeds = [10, 12, 15,18,20,-1] # Base speeds for each layer
var scroll_offsets = []

var screen_size = Vector2(1920, 1080) # Set the screen size to 1920x1080

# Called when the node enters the scene tree for the first time.

func _ready():
	# Initialize layers and speeds // the last 5 children of the node except the last one
	for i in range(get_child_count() - 6, get_child_count()):
		layers.append(get_child(i))
		layer_speeds.append(layers.back().autoscroll.x)
		scroll_offsets.append(layers.back().scroll_offset.x)

#base on the mouse position, set the autoscroll speed of the layers
func _process(delta):
	# Get the mouse position in the viewport
	var mouse_pos = get_global_mouse_position()
	
	var cScale = (mouse_pos.x - screen_size.x ) / (screen_size.x/2 )

	for i in range(layers.size()):
		scroll_offsets[i] += base_speeds[i] * cScale * delta
		layers[i].scroll_offset.x = scroll_offsets[i]