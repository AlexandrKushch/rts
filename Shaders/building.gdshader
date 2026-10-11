shader_type canvas_item;

uniform float progress : hint_range(0.0, 1.0) = 0.0;
uniform float faded_alpha : hint_range(0.0, 1.0) = 0.25;

void fragment() {
    vec4 c = COLOR;
    float fill = step(1.0 - progress, UV.y);
    float alpha = mix(0.25, c.a, fill);
    COLOR = vec4(c.rgb, c.a * alpha);
}