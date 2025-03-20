extends Control

var isSetting:bool=0

enum Orentation {
	left=0,drop=1
}

@export var isPausing:bool = false
@export var orientation:Orentation = Orentation.left

@export var hasPause:bool = true

#var Data = preload("res://UI/Data.gd").new()

var pos:Array [Vector2] = [Vector2(-309.0,0.0),Vector2(0.0,100.0)]
var siz:Array [Vector2] = [Vector2(309.0,100.0),Vector2(100.0,309.0)]
var sizeEach: Vector2 = Vector2(102.0,102.0)
var count:int=0

var buttonContainer:Control

static var loaded: bool=false

func _ready():
	if !loaded:
		#Data.load_Data()
		loaded=1

	buttonContainer=$Control
		
	#var j:int =0
	# for i in $Poppin/Popup/Volume.get_children():
	# 	i.value_changed.connect(_on_value_changed.bind(i.get_meta("busIndex")))
	# 	#i.value=Global.Volume[j]
	# 	#_on_value_changed(Global.Volume[j],i.get_meta("busIndex"))
	# 	j+=1

	setUp()

	pos[0]=Vector2(-(sizeEach.x+3)*(count+1),0.0)
	pos[1]=Vector2(-sizeEach.x,sizeEach.y)
	siz[0]=Vector2((sizeEach.x+3)*count,0.0)
	siz[1]=Vector2(0,(sizeEach.y+3)*count)

func setUp():
	if !hasPause:
		buttonContainer.get_node("PauseBtn").visible=false
		
	#get number of buttons that is visible
	count=0
	for i in buttonContainer.get_children():
		if i.visible:
			count+=1
			sizeEach=i.size

	var j:int=0	

	for i in buttonContainer.get_children():# set anchor of each button so that every evenly distributed, left to right, top to bottom
		if i.visible:
			i.anchor_left=0.0+(1.0/count)*j
			i.anchor_right=0.0+(1.0/count)*(j+1)
			i.anchor_top=0.0+(1.0/count)*j
			i.anchor_bottom=0.0+(1.0/count)*(j+1)
			j+=1

	
func _on_value_changed(value:float,bus_index:int):
	AudioServer.set_bus_volume_db(bus_index,linear_to_db(value))
	#Global.Volume[bus_index]=value
	
func _on_setting_btn_pressed():	
	stretch_retract()
		
	resetBtn()

func stretch_retract():
	
	if !isPausing:
		var tween:Tween = get_tree().create_tween()
		tween.set_parallel(true)
		
		if !isSetting:
			tween.tween_property(buttonContainer,"position" ,pos[orientation],0.5 ).set_trans(Tween.TRANS_LINEAR)
			tween.tween_property(buttonContainer,"size" ,siz[orientation],0.5 ).set_trans(Tween.TRANS_LINEAR)
			isSetting=1
			
		else:
			tween.tween_property(buttonContainer,"position" ,-sizeEach+Vector2(0.0,sizeEach.y),0.5 ).set_trans(Tween.TRANS_LINEAR)
			tween.tween_property(buttonContainer,"size" ,Vector2(0.0,0.0),0.5 ).set_trans(Tween.TRANS_LINEAR)
			isSetting=0
	else:
		if !isSetting:
			buttonContainer.position=pos[orientation]
			buttonContainer.size=siz[orientation]
			isSetting=1
			
		else:
			buttonContainer.position=-sizeEach+Vector2(0.0,sizeEach.y)
			buttonContainer.size=Vector2(0.0,0.0)
			isSetting=0

func resetBtn():
	
	for i in buttonContainer.get_children():
		if isSetting:
			i.disabled=0
		else: 
			i.disabled=0
			
	buttonContainer.get_node("VolumeBtn").button_pressed = false

func _on_volume_btn_toggled(toggled_on):
	$Poppin/Volume.visible=toggled_on	

func _on_ratio_btn_toggled(toggled_on):
	if (toggled_on):
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_FULLSCREEN)
	else:
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_MAXIMIZED)

func _on_tree_exited():
	pass
	#Data.save_Data()

func _on_info_btn_toggled(toggled_on):
	#quit
	if (toggled_on):
		get_tree().quit()
	pass
	# if toggled_on:
	# 	$Poppin/Credit.global_position=Global.ScreenSize/2.0-$Poppin/Credit.size/(2.0*$Poppin/Credit.scale)+Vector2(100.0,0)
	# else:
	# 	$Poppin/Credit.global_position=Vector2(-2000.0,-2000.0)


func _on_pause_btn_toggled(toggled_on:bool) -> void:
	get_tree().paused=toggled_on
	$Node/Pause.visible=toggled_on
