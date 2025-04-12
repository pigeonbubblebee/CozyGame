using Godot;
using System;

public partial class Summon : CharacterBody2D
{
	protected Player _player;
	protected Enemy _target;
	[Export] private NodePath _spritePath;
	protected AnimatedSprite2D _sprite;
	[Export] public string SummonID;
	[Export] public int Speed { get; private set; }
	[Export] public float IdleSpeed { get; private set; }
	[Export] public float Friction { get; private set; }
	[Export] public float BobSpeed { get; private set; }
	[Export] public float IdleHeight { get; private set; }
	[Export] public float FollowDist { get; private set; }
	[Export] public float TargetStepRange { get; private set; }
    private double idleTime;
	protected bool attacking = false;
	public void Initialize(Player p) {
		this._player = p;
		_player.AttackController.HitEvent += ChangeAggro;
	}

    public override void _Ready()
    {
        base._Ready();
		_sprite = GetNode<AnimatedSprite2D>(_spritePath);
		_sprite.Play("idle");
    }


    public override void _Process(double delta)
    {
        base._Process(delta);

		if(!IsInstanceValid(_target))
			_target = null;
		if(attacking)
			return;

		if(_target != null) {
			if(GlobalPosition.DistanceTo(_target.GlobalPosition) >= TargetStepRange) {
				Vector2 direction = (_target.GlobalPosition - GlobalPosition).Normalized();
				Velocity = direction * Speed;
				this._sprite.Scale = Velocity.X > 0 ? new Vector2(4, 4) : new Vector2(-4, 4);
			} else if(GlobalPosition.DistanceTo(_target.GlobalPosition) < TargetStepRange - 10f) {
				Vector2 direction = (_target.GlobalPosition - GlobalPosition).Normalized();
				Velocity = -direction * Speed;
				this._sprite.Scale = _target.GlobalPosition.X > GlobalPosition.X ? new Vector2(4, 4) : new Vector2(-4, 4);
			} else {
				Velocity = Vector2.Zero;
			}
						
		} else {
			idleTime += delta;

			// Base position near the player (e.g., offset to the right)
			Vector2 desiredPosition = _player.GlobalPosition + new Vector2(_player.MovementController.Direction == 1 ? -FollowDist : FollowDist, -100f);

			// Add bobbing effect (vertical sine wave)
			desiredPosition.Y += Mathf.Sin((float)idleTime * BobSpeed) * IdleHeight;

			// Smooth movement using interpolation
			GlobalPosition = GlobalPosition.Lerp(desiredPosition, IdleSpeed * (float)delta);

			this._sprite.Scale = desiredPosition.X > GlobalPosition.X ? new Vector2(4, 4) : new Vector2(-4, 4);

			// IdleFloat(delta);
		}

		MoveAndSlide();
    }

	public void ChangeAggro(IHittable i, int damage, int direction, int pDamage) {
		if(i is EnemyHitbox) {
			_target = ((EnemyHitbox)i).EnemyAIParent;
			// ((EnemyHitbox)i).EnemyAIParent.DeathEvent += _DeAggroHelper();
		}
	}


	public void DeAggro() {
		_target = null;
	}
}
