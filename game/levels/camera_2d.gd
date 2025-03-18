extends Camera2D
@export var zoomSpeed : float = 10;

var zoomTarget :Vector2

var dragStartMousePos = Vector2.ZERO
var dragStartCameraPos = Vector2.ZERO
var isDragging : bool = false
var zoom_max:Vector2 = Vector2(1.32, 1.32)
var zoom_min:Vector2 = Vector2(0.63, 0.63)


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	zoomTarget = zoom
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	Zoom(delta)
	ClickAndDrag()

func Zoom(delta):
	if Input.is_action_just_pressed("camera_zoom_in"):
		zoomTarget *= 1.1
		
	if Input.is_action_just_pressed("camera_zoom_out"):
		zoomTarget *= 0.9

	zoomTarget = clamp(zoomTarget, zoom_min, zoom_max)
		
	zoom = zoom.slerp(zoomTarget, zoomSpeed * delta)



func ClickAndDrag():
	if !isDragging and Input.is_action_just_pressed("camera_pan"):
		dragStartMousePos = get_viewport().get_mouse_position()
		dragStartCameraPos = position
		isDragging = true
		
	if isDragging and Input.is_action_just_released("camera_pan"):
		isDragging = false
		
	if isDragging:
		var moveVector = get_viewport().get_mouse_position() - dragStartMousePos
		position = dragStartCameraPos - moveVector * 1/zoom.x	
