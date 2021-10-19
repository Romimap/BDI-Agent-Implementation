shader_type spatial;

void vertex() {
	vec3 p = VERTEX;
	p.y += sin(TIME + (p.x + p.z) / 2.0) / 2.0;
	VERTEX = p;
}

void fragment() {
	ALPHA = 0.2;
}