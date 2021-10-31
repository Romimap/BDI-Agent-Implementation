using Godot;
using System;

public class PackageNode : Spatial
{
	[Export] Curve _animationCurve;
	Vector3 _scaleFrom;
	Vector3 _scaleTo;
	float _timer = 1;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		_scaleFrom = Scale;
		_scaleTo = Scale;
		_timer = 0f;
	}

	public void PickUp() {
		GD.Print("PackageNode PickUp");
		_scaleFrom = ((Spatial)MapViewer.PACKAGE.Instance()).Scale;
		_scaleTo = new Vector3(0, 0, 0);
		_timer = 0f;
	}

	public void DropOff() {
		GD.Print("PackageNode DropOff");
		_scaleFrom = new Vector3(0, 0, 0);
		_scaleTo = ((Spatial)MapViewer.PACKAGE.Instance()).Scale;
		_timer = 0f;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		_timer += delta * 2;
		float t = _animationCurve.Interpolate(_timer);
		Vector3 scale = _scaleTo * t + _scaleFrom * (1 - t);
		Scale = scale;
	}
}
