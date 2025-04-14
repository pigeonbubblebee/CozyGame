using Godot;
using System;

public partial class AdjustableSound : AudioStreamPlayer2D
{
    private float origPitch;
    public override void _Ready()
    {
        base._Ready();
        origPitch = this.PitchScale;
    }

    public void PlayPitchShifted(float pitchShift) {
        PitchScale = origPitch;
        PitchScale += (float)GD.RandRange(-pitchShift, pitchShift);

        Play();
    }
}
