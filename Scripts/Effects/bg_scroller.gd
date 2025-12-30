extends Node2D

@export var speed: float = 30.0
@export var bg1_path: NodePath
@export var bg2_path: NodePath

var bg1: Sprite2D
var bg2: Sprite2D
var tex_w := 0.0
var tex_h := 0.0
var scaled_w := 0.0

func _ready():
	process_mode = Node.PROCESS_MODE_ALWAYS  # tetap jalan meski scene lain ganti/pause

	bg1 = get_node(bg1_path) as Sprite2D
	bg2 = get_node(bg2_path) as Sprite2D

	if bg1 == null or bg2 == null or bg1.texture == null or bg2.texture == null:
		push_error("BgScroller: set bg1_path/bg2_path & pastikan kedua Sprite2D punya texture.")
		set_process(false)
		return

	bg1.centered = false
	bg2.centered = false

	tex_w = float(bg1.texture.get_width())
	tex_h = float(bg1.texture.get_height())

	_resize_fit()
	get_viewport().size_changed.connect(_resize_fit)

func _process(delta):
	var dx = speed * delta
	bg1.position.x -= dx
	bg2.position.x -= dx

	if bg1.position.x <= -scaled_w:
		bg1.position.x = bg2.position.x + scaled_w
	if bg2.position.x <= -scaled_w:
		bg2.position.x = bg1.position.x + scaled_w

func _resize_fit():
	var vp_h = float(get_viewport_rect().size.y)
	var s = vp_h / max(tex_h, 1.0)  # scale tinggi pas layar, jaga aspect ratio
	bg1.scale = Vector2(s, s)
	bg2.scale = Vector2(s, s)

	scaled_w = tex_w * s

	bg1.position = Vector2(0, 0)
	bg2.position = Vector2(scaled_w, 0)
