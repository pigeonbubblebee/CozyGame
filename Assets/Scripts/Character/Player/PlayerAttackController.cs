using Godot;
using System;
using System.Threading.Tasks;

public partial class PlayerAttackController : Node2D // TODO: Attack Buffer
{
	[Export] public float SlashTime;
	[Export] public int SlashDamage;
	[Export] public int SlashRange;
	[Export] public float SlashCooldown;
	[Export] public NodePath SlashBufferPath;
	private Timer _slashBuffer;

	public bool DesiredAttack => _CheckDesiredAttack() && CanSlash;
	private IInputManager _inputManager;

	public event Action<int, float, float> SlashEvent; // Damage, Speed, Range
	public event Action<IHittable, int, int> HitEvent; // Hittable, Damage, Direction
	public event Action FinishSlashEvent;

	[Export] public NodePath AttackAreaPath;
	private Area2D _attackArea;
	[Export] public NodePath RightAttackAreaColliderPath;
	private CollisionShape2D _rightAttackAreaCollider;
	[Export] public NodePath LeftAttackAreaColliderPath;
	private CollisionShape2D _leftAttackAreaCollider;

	[Export] public NodePath RightAttackSpritePath;
	private Sprite2D _rightAttackSprite;
	[Export] public NodePath LeftAttackSpritePath;
	private Sprite2D _leftAttackSprite;

	private CollisionShape2D _currentAttackAreaCollider;
	private Sprite2D _currentAttackSprite;
	public bool CanSwitchAttackDirection = true;

	private bool _canHit = false;
	public bool CanSlash { get; private set; }

	private Player _player;

	private PlayerMovementController _movementController;

	public override void _Ready()
	{
		CanSlash = true;

		_inputManager = GetNode<IInputManager>("/root/InputManager");
		_attackArea = GetNode<Area2D>(AttackAreaPath);
		_rightAttackAreaCollider = GetNode<CollisionShape2D>(RightAttackAreaColliderPath);
		_leftAttackAreaCollider = GetNode<CollisionShape2D>(LeftAttackAreaColliderPath);

		_rightAttackSprite = GetNode<Sprite2D>(RightAttackSpritePath);
		_leftAttackSprite = GetNode<Sprite2D>(LeftAttackSpritePath);

		_slashBuffer = GetNode<Timer>(SlashBufferPath);

		_attackArea.AreaEntered += _OnSlashHit;
		_attackArea.CollisionMask = (uint) PhysicsLayers.HittableLayer;
		_attackArea.CollisionLayer = (uint) PhysicsLayers.UntouchableLayer;

		HitEvent += _OnHit;
		SlashEvent += _EnableSlashSprite;
		FinishSlashEvent += _DisableSlashSprite;
	}

	public void Initialize(Player player) {
		_player = player;
		_movementController = _player.MovementController;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_UpdateAttackCollider();

		if(_CheckDesiredAttack() && !CanSlash) {
			StartSlashBuffer();
		}
	}

	private bool _CheckDesiredAttack() {
		return _inputManager.GetAttackActuation();
	}

	

	public void Slash() {
		CanSwitchAttackDirection = false;
		SlashEvent?.Invoke(SlashDamage, SlashTime, SlashRange);

		_canHit = true;
		_currentAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
		
		GetTree().CreateTimer(SlashTime).Timeout += _FinishSlash;
		// await ToSignal(GetTree().CreateTimer(SlashTime), SceneTreeTimer.SignalName.Timeout);
	}

	private void _FinishSlash() {
		_canHit = false;
		_currentAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

		FinishSlashEvent?.Invoke();
		CanSwitchAttackDirection = true;
	}

	public void StartSlashCooldown() {
		CanSlash = false;
		GetTree().CreateTimer(SlashCooldown).Timeout += _FinishSlashCooldown;
	}

	private void _FinishSlashCooldown() {
		CanSlash = true; 
	}

	private void _OnSlashHit(Node2D hit) {
		if(!_canHit || !(hit is IHittable)) {
			return;
		}

		HitEvent?.Invoke((IHittable) hit, SlashDamage, this.GlobalPosition.X > hit.GlobalPosition.X ? 1 : -1);
	}

	private void _OnHit(IHittable hittable, int damage, int direction) {
		hittable.OnHit(_player, damage, direction); // Maybe flip direction, not sure yet
		// TODO: Slash Particle, Screen Shake
	}

	private void _UpdateAttackCollider() {
		if(!CanSwitchAttackDirection) {
			return;
		}

		_currentAttackSprite = _movementController.Direction == 1 ? _rightAttackSprite : _leftAttackSprite;

		_currentAttackAreaCollider = _movementController.Direction == 1 ? _rightAttackAreaCollider : _leftAttackAreaCollider;
	}

	public void StartSlashBuffer() {
		_slashBuffer.Start();
	}

	public bool GetSlashBufferStop() {
		return _slashBuffer.IsStopped();
	}

	private void _EnableSlashSprite(int damage, float speed, float range) {
		_currentAttackSprite.Visible = true;
	}

	private void _DisableSlashSprite() {
		_currentAttackSprite.Visible = false;
	}
}
