extends CanvasLayer

@export var photo: Sprite2D
@export var fit_width: bool = true      # true: lebar pas layar; false: tinggi pas layar
@export var duration: float = 0.8       # durasi tween default

# nyimpen info ukuran tekstur dan viewport biar gampang dipake ulang
var _tex_w := 1.0
var _tex_h := 1.0
var _scale := 1.0
var _vp := Vector2.ZERO

# buat inisialisasi background biar langsung ngepasin ukuran dan nyambung sama event resize
func _ready():
	process_mode = Node.PROCESS_MODE_ALWAYS
	if photo == null or photo.texture == null:
		push_error("Assign 'photo' ke Sprite2D Photo dan pastikan ada texture-nya.")
		return

	photo.centered = false
	_tex_w = float(photo.texture.get_width())
	_tex_h = float(photo.texture.get_height())

	_apply_scale_to_fit()
	get_viewport().size_changed.connect(_apply_scale_to_fit)

# buat geser foto ke titik Y (ngikut koordinat tekstur asli) pake tween halus
func slide_to_anchor_y(anchor_tex_y: float, dur := duration):
	if photo == null:
		return
	var target_y := -anchor_tex_y * _scale
	# (opsional) batasi supaya tidak “lepas” dari layar:
	# target_y = clamp(target_y, _min_y(), _max_y())

	var tw := get_tree().create_tween()
	tw.tween_property(photo, "position:y", target_y, dur)\
	  .set_trans(Tween.TRANS_QUAD).set_ease(Tween.EASE_OUT)

# ngitung ulang scale biar sprite bisa nutupin layar sesuai mode fit
func _apply_scale_to_fit():
	_vp = get_viewport().get_visible_rect().size  # penting: CanvasLayer pakai get_viewport()
	if fit_width:
		_scale = _vp.x / _tex_w
	else:
		_scale = _vp.y / _tex_h
	photo.scale = Vector2(_scale, _scale)
	photo.position = Vector2(0, 0)  # start dari bagian atas

# batas minimal buat clamp kalo mau biar layar gak kosong di bawah
func _min_y() -> float:
	return _vp.y - (_tex_h * _scale)  # batas paling atas
# batas maksimal buat clamp kalo mau biar bagian atas gak kelewatan
func _max_y() -> float:
	return 0.0                        # batas paling bawah (atas foto sejajar layar)
