extends TextureRect

@export var speed: float = 50.0
var offset_x: float = 0.0

func _process(delta):
	offset_x += speed * delta
	material.set_shader_parameter("scroll", offset_x)
