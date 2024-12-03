using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public NodePath HitboxPath { get; private set; }
	private EnemyHitbox _hitbox;
	[Export] public NodePath HealthSystemPath { get; private set; }
	private HealthSystem _health;
	
	[Export] public int MaxHealth { get; protected set; }
	
	[Export] public int MaxPosture { get; protected set; }
	[Export] public float PostureRegenerationCooldownTime { get; protected set; }
	[Export] public float PostureRegenerationTime { get; protected set; }
	[Export] public int PostureRegenerationRate { get; protected set; }
	public int CurrentPosture { get; protected set; }
	private float _lastPostureRegeneration;
	
	[Export] private NodePath _postureRegenerationCooldownTimerPath;
	private Timer _postureRegenerationCooldownTimer;
	[Export] private NodePath _postureRegenerationTimerPath;
	private Timer _postureRegenerationTimer;
	
	[Export] private NodePath _attacksParentPath;
	public EnemyAttack[] Attacks { get; private set; }
	
	public EnemyAttack CurrentAttack { get; private set; }
	
	public event Action FinishAttackEvent;
	
	[Export] private NodePath _spritePath;
	public AnimatedSprite2D Sprite { get; private set; }
	
	public readonly float GRAVITY = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	[Export] public NodePath StateMachinePath { get; private set; }
	protected EnemyStateMachine StateMachine;
	
	public override void _EnterTree() {
		StateMachine = GetNode<EnemyStateMachine>(StateMachinePath);
		StateMachine.Initialize(this);
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_hitbox = GetNode<EnemyHitbox>(HitboxPath);
		_hitbox.OnHitEvent += OnHit;
		
		_health = GetNode<HealthSystem>(HealthSystemPath);
		_health.DeathEvent += OnDeath;
		CurrentPosture = MaxPosture;
		
		_health.MaxHealthPoints = MaxHealth;
		_health.ResetHealth();
		_health.Invincible = false;
		
		_postureRegenerationCooldownTimer = GetNode<Timer>(_postureRegenerationCooldownTimerPath);
		_postureRegenerationTimer = GetNode<Timer>(_postureRegenerationTimerPath);
		
		Sprite = GetNode<AnimatedSprite2D>(_spritePath);
		
		Node parent = GetNode<Node>(_attacksParentPath);
		Attacks = new EnemyAttack[parent.GetChildren().Count];
		int i = 0;
		
		foreach(Node n in parent.GetChildren()) {
			Attacks[i] = (EnemyAttack) n;
			Attacks[i].Initialize(this);
			if(Attacks[i].NotChainAttack) {
				Attacks[i].FinishAttackEvent += FinishAttackEvent;
			}
			i ++;
		}
	}
	
	public void ExecuteAttack(EnemyAttack e, Player p) {
		e.Execute(p, this);
	}
	
	public EnemyAttack GetCurrentAttack(Player p) {
		foreach(EnemyAttack e in Attacks) {
			if(e.GetCondition(p, this)) {
				return e;
			}
		}
		
		return null;
	}
	
	public void RegeneratePosture() {
		if(!_postureRegenerationTimer.IsStopped()) {
			return;
		}
		
		if(_postureRegenerationTimer.IsStopped()) {
			CurrentPosture += PostureRegenerationRate;
			if(CurrentPosture > MaxPosture) {
				CurrentPosture = MaxPosture;
			}
			_postureRegenerationTimer.Start(PostureRegenerationTime);
		}
	}
	
	public void TakePostureDamage(int damage) {
		CurrentPosture -= damage;
		_postureRegenerationCooldownTimer.Start(PostureRegenerationCooldownTime);
		_postureRegenerationTimer.Stop();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if(!IsOnFloor()) {
			Vector2 velocity = Velocity;
			velocity.Y += GRAVITY * (float) delta * 60f;

			Velocity = velocity;
		}
	}
	
	public virtual void OnHit(Player player, int damage, int direction, int postureDamage) {
		GD.Print("OUCH!");
		_health.TakeDamage(damage);
		TakePostureDamage(postureDamage);
	}
	
	protected virtual void OnDeath() {
		this.QueueFree();
	}
}
