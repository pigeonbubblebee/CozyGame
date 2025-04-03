using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public NodePath HitboxPath { get; private set; }
	private EnemyHitbox _hitbox;
	[Export] public NodePath HealthSystemPath { get; private set; }
	private HealthSystem _health;

	[Export] public int _currency_drop_amount = 5;
	[Export] private Item coin_resource;
	
	[Export] private NodePath _healthBarPath;
	protected TextureProgressBar _healthBar;
	
	[Export] public int MaxHealth { get; protected set; }
	public event Action<Enemy> DeathEvent;
	
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
	[Export] private NodePath _deathblowMarkPath;
	private TextureRect _deathblowMark;
	
	[Export] private NodePath _attacksParentPath;
	protected Node2D _attacksParent;
	public EnemyAttack[] Attacks { get; private set; }
	
	public EnemyAttack CurrentAttack { get; private set; }
	
	public event Action FinishAttackEvent;
	
	[Export] private NodePath _spritePath;
	public AnimatedSprite2D Sprite { get; private set; }
	
	public readonly float GRAVITY = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	[Export] public NodePath StateMachinePath { get; private set; }
	protected EnemyStateMachine StateMachine;
	
	[Export] private NodePath _postureBarPath;
	protected TextureProgressBar _postureBar;
	
	public event Action<Player, int, int, int> TakeDamageEvent;
	
	public bool Staggered;
	public event Action PostureBreakEvent;
	public event Action StaggerRecoveryEvent;
	[Export] public float StaggerRecoveryTime;
	
	[Export] public int DeathBlowDamage;
	
	// --------- Knockback Variables ---------
	[Export] private NodePath _knockbackTimePath;
	private Timer _knockbackTimer;
	
	private float _knockbackSpeed;
	private float _knockbackAcceleration;
	private int _knockbackDirection;
	
	public bool KnockbackEnabled = true;
	
	[Export] public float IFrameTime = 0.3f;
	private bool _iFrameOn = false;
	public bool Invincible => _iFrameOn;
	
	[Export] public bool HasBossBar { get; private set; } = false;
	[Export] public string EnemyNameLocalizationKey;
	
	public override void _EnterTree() {
		StateMachine = GetNode<EnemyStateMachine>(StateMachinePath);
		StateMachine.Initialize(this);
		
		Sprite = GetNode<AnimatedSprite2D>(_spritePath);
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_hitbox = GetNode<EnemyHitbox>(HitboxPath);
		_hitbox.OnHitEvent += OnHit;
		_hitbox.Initialize(this);
		
		_health = GetNode<HealthSystem>(HealthSystemPath);
		_health.DeathEvent += OnDeath;
		_health.DeathEvent += DisableBossBar;
		// _health.DamageEvent += TakeDamageEvent;
		CurrentPosture = MaxPosture;
		
		_health.MaxHealthPoints = MaxHealth;
		_health.ResetHealth();
		_health.Invincible = false;
		
		_postureRegenerationCooldownTimer = GetNode<Timer>(_postureRegenerationCooldownTimerPath);
		_postureRegenerationTimer = GetNode<Timer>(_postureRegenerationTimerPath);
		
		_postureBar = GetNode<TextureProgressBar>(_postureBarPath);
		_healthBar = GetNode<TextureProgressBar>(_healthBarPath);
		_deathblowMark = GetNode<TextureRect>(_deathblowMarkPath);
		
		Sprite = GetNode<AnimatedSprite2D>(_spritePath);

		_attacksParent = GetNode<Node2D>(_attacksParentPath);
		Attacks = new EnemyAttack[_attacksParent.GetChildren().Count];
		int i = 0;
		
		foreach(Node n in _attacksParent.GetChildren()) {
			Attacks[i] = (EnemyAttack) n;
			Attacks[i].Initialize(this);
			// if(Attacks[i].NotChainAttack) {
				Attacks[i].GetNextChainAttack().FinishAttackEvent += FinishAttackEvent;
			// }
			i ++;
		}
		
		FinishAttackEvent += _FinishAttack;
		
		_knockbackTimer = GetNode<Timer>(_knockbackTimePath);
		_knockbackTimer.Timeout += _StopKnockback;
	}
	
	public void ExecuteAttack(EnemyAttack e, Player p) {
		e.Execute(p, this);
		CurrentAttack = e;
	}
	
	public EnemyAttack GetCurrentAttack(Player p) {
		foreach(EnemyAttack e in Attacks) {
			if(e.GetCondition(p, this)) {
				return e;
			}
		}
		
		return null;
	}
	
	private void _FinishAttack() {
		CurrentAttack = null;
	}
	
	public void RegeneratePosture() {
		//GD.Print(_postureRegenerationCooldownTimer.TimeLeft + "Time");
		// GD.Print(_postureRegenerationCooldownTimer.TimeLeft);
		if(!_postureRegenerationCooldownTimer.IsStopped()) {
			return;
		}
		
		if(_postureRegenerationTimer.IsStopped()) {
			float healthRatio = ((float)_health.CurrentHealthPoints / (float)_health.MaxHealthPoints);
			if(healthRatio <= 0.75f) // If not at full, posture regenerates slower
				healthRatio *= 0.8f;
			if(healthRatio <= 0.5f) // If under half, posture regenerates even slower (at around 60%)
				healthRatio *= 0.8f;
			// GD.Print(PostureRegenerationRate * (_health.CurrentHealthPoints / _health.MaxHealthPoints));
			CurrentPosture += (int)(PostureRegenerationRate * healthRatio);
			if(CurrentPosture > MaxPosture) {
				CurrentPosture = MaxPosture;
			}
			_postureRegenerationTimer.Start(PostureRegenerationTime);
		}
	}
	
	public void TakePostureDamage(int damage) {
		if(CurrentPosture <= 0) {
			return;
		}
		
		CurrentPosture -= damage;
		if(CurrentPosture <= 0) {
			PostureBreakEvent?.Invoke();
			GetTree().CreateTimer(StaggerRecoveryTime).Timeout += _RecoverStagger;
			Staggered = true;
			CurrentPosture = 0;
			_deathblowMark.Visible = true;
			
			_postureRegenerationCooldownTimer.Start(StaggerRecoveryTime + PostureRegenerationCooldownTime);
		} else {
			_postureRegenerationCooldownTimer.Start(PostureRegenerationCooldownTime);
			//GD.Print(_postureRegenerationCooldownTimer.TimeLeft + "Time");
		}
		_postureRegenerationTimer.Stop();
	}
	
	private void _RecoverStagger() {
		
		// _postureRegenerationTimer.Stop();
		Staggered = false;
		_deathblowMark.Visible = false;
		StaggerRecoveryEvent?.Invoke();
		CurrentPosture = MaxPosture;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// GD.Print(Name + " " + CurrentPosture);
		
		float postureRatio = ((float)CurrentPosture) / ((float)MaxPosture);
		// GD.Print(Ratio);
		postureRatio = Mathf.Max(postureRatio, 0);
		postureRatio = 1 - postureRatio;
		
		// _postureBar.Value = postureRatio * 100;
		
		float Ratio = postureRatio * 100;
		
		if(Ratio != _postureBar.Value) {
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(_postureBar, "value", Ratio, 0.075f);
			// tweeningValuePosture = Ratio;
		}
		
		float healthRatio = ((float)_health.CurrentHealthPoints) / ((float)_health.MaxHealthPoints);
		// GD.Print(Ratio);
		healthRatio = Mathf.Max(healthRatio, 0);

		// _healthBar.Value = healthRatio * 100;
		
		Ratio = healthRatio * 100;
		
		if(Ratio != _healthBar.Value) {
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(_healthBar, "value", Ratio, 0.075f);
			// tweeningValuePosture = Ratio;
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if(!_knockbackTimer.IsStopped() && KnockbackEnabled) {
			// GD.Print("Recoil!");
			KnockbackRecoil(new Vector2(_knockbackDirection, 0f), _knockbackSpeed, _knockbackAcceleration, delta);
		} 
		
		if(!IsOnFloor()) {
			Vector2 velocity = Velocity;
			velocity.Y += GRAVITY * (float) delta * 60f;

			Velocity = velocity;
		}
	}
	
	public virtual void OnHit(Player player, int damage, int direction, int postureDamage) {
		if(_iFrameOn)
			return;
		GD.Print("OUCH!");
		TakeDamageEvent?.Invoke(player, damage, direction, postureDamage);
		_health.TakeDamage(damage);
		TakePostureDamage(postureDamage);
		_iFrameOn = true;
		GetTree().CreateTimer(IFrameTime).Timeout += _FinishIFrame;
	}
	
	public int GetCurrentHealth() {
		return _health.CurrentHealthPoints;
	}
	
	private void _FinishIFrame() {
		_iFrameOn = false;
	}
	
	protected virtual void OnDeath() {
		GD.Print("Death");
		DeathEvent?.Invoke(this);
		GetNode<Player>("/root/Player").InventoryManager.AddItemToInventory(coin_resource, _currency_drop_amount);
		this.QueueFree();
		GD.Print(IsInstanceValid(this));
	}
	
	public void ApplyKnockback(int direction, float speed, float acceleration, float time) {
		_knockbackSpeed = speed;
		_knockbackAcceleration = acceleration;
		_knockbackDirection = direction;
		_knockbackTimer.Start(time);
	}
	
	private void _StopKnockback() {
		// GD.Print("Stop Recoil!");
		Vector2 velocity = new Vector2(0, Velocity.Y);
		this.Velocity = velocity;
	}
	
	public void KnockbackRecoil(Vector2 direction, float speed, float acceleration, double delta) {
		Vector2 velocity = this.Velocity.MoveToward(speed * direction, acceleration * (float)delta * 60f);
		velocity.Y = Velocity.Y;
		this.Velocity = velocity;
	}
	
	public void DisableBossBar() {
		if(HasBossBar) {
			GetNode<UIManager>("/root/UIManager").DisableBossBar(this);
		}
	}
}
