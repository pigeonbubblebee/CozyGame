using Godot;
using System;

public partial class PlayerCamera : Camera2D
{
	// Called when the node enters the scene tree for the first time.
	// private CinemachineVirtualCamera _vcam;
	[Export] public float _Offset = -120f;
	
	private double _shakeTimer;
	private Vector2 _camBasePosition;
	private float _shakeMagnitude;
	
	private RandomNumberGenerator _rng;
	
	private FastNoiseLite _noise;
	private float _noiseI;

	public override void _Ready() {
		_noise = new();
		_noise.Seed = (int)GD.Randf();
		// _noise.Frequency = 0.01f;
		// _noise.Period = 2;
		//_noise.NoiseType = FastNoiseLite.TYPESIMPLEX;
		_rng = new();
	}

	public void Shake(float time, float magnitude) {
		_noise.Seed = (int)GD.Randf();
		_camBasePosition = this.GlobalPosition;
		_shakeTimer = time;
		_shakeMagnitude = magnitude * 0.12f;
	}

	public override void _Process(double delta) {
		_shakeMagnitude = Mathf.Lerp(_shakeMagnitude, 0f, 5f * (float)delta);
		
		if (_shakeTimer > 0f) {
			this.Offset = new Vector2(0, _Offset)
				+ _GetNoise(delta);
			_shakeTimer -= delta;
		}
		if(_shakeTimer <= 0f) {
			// this.Position = new Vector2(0f, 0f);
			this.Offset = new Vector2(0, _Offset);
		}
	}

	private Vector2 _GetNoise(double delta) {
		_noiseI += (float)delta * 60f;
		//GD.Print(_noise.GetNoise1D(
			// GD.Randf() * (float)_shakeTimer) * _shakeMagnitude);
		return new Vector2(
			_noise.GetNoise2D(1, _noiseI) * _shakeMagnitude, 
			_noise.GetNoise2D(100, _noiseI) * _shakeMagnitude
		);
	}
}
