using Godot;
using System;

public class AgentNode : Spatial
{
	[Export] Curve _animationCurve;
	Vector3 _from;
	Vector3 _to;
	float _timer = 1;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		_from = GlobalTransform.origin;
		_to = _from;
		_timer = 0f;
	}

	public void MoveTo (Vector3 position) {
		_from = GlobalTransform.origin;
		_to = position;
		_timer = 0f;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		_timer += delta * 2;
		float t = _animationCurve.Interpolate(_timer);
		Vector3 p = _to * t + _from * (1 - t);
		Transform tmpTransform = GlobalTransform;
		tmpTransform.origin = p;
		GlobalTransform = tmpTransform;
	}
}
