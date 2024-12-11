using Godot;
using System;

public partial class PlayerCamera : Camera2D
{
	// Called when the node enters the scene tree for the first time.
	// private CinemachineVirtualCamera _vcam;
	
	private double _shakeTimer;
	private Vector2 _camBasePosition;
	private float _shakeMagnitude;
	
	private RandomNumberGenerator _rng;
	
	private FastNoiseLite _noise;

	public override void _Ready() {
		_noise = new();
		_rng = new();
	}

	public void Shake(float time, float magnitude) {
		_camBasePosition = this.GlobalPosition;
		_shakeTimer = time;
		_shakeMagnitude = magnitude;
	}

	public override void _Process(double delta) {
		if (_shakeTimer > 0f) {
			this.Offset = new Vector2(0, -120f)
				+ new Vector2(_GetNoise(0), _GetNoise(1));
			_shakeTimer -= delta;
		}
		if(_shakeTimer <= 0f) {
			// this.Position = new Vector2(0f, 0f);
			this.Offset = new Vector2(0, -120f);
		}
	}

	private float _GetNoise(int seed) {
		_noise.Seed = seed;
		//GD.Print(_noise.GetNoise1D(
			// GD.Randf() * (float)_shakeTimer) * _shakeMagnitude);
		return _noise.GetNoise1D(
			GD.Randf()) * _shakeMagnitude * _rng.RandfRange(-2, 2);
	}
}
