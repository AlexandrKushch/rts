shader_type canvas_item;

uniform bool on;
uniform vec4 color: source_color;

void fragment() {
	if (on) {
		COLOR = color * COLOR.a;
	}
}