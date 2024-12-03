using Godot;
using System;
using System.Threading.Tasks;

public partial class PlayerAttackController : Node2D // TODO: Attack Buffer
{
	[Export] private NodePath _slashBufferPath;
	private Timer _slashBuffer;

	public bool DesiredAttack => _CheckDesiredAttack() && CanSlash;
	private IInputManager _inputManager;

	public event Action<int, float, float> SlashEvent; // Damage, Speed, Range
	public event Action<IHittable, int, int, int> HitEvent; // Hittable, Damage, Direction
	public event Action FinishSlashEvent;

	[Export] private NodePath _attackAreaPath;
	private Area2D _attackArea;
	[Export] private NodePath _rightAttackAreaColliderPath;
	private CollisionShape2D _rightAttackAreaCollider;
	[Export] private NodePath _leftAttackAreaColliderPath;
	private CollisionShape2D _leftAttackAreaCollider;

	[Export] private NodePath _rightAttackSpritePath;
	private Sprite2D _rightAttackSprite;
	[Export] private NodePath _leftAttackSpritePath;
	private Sprite2D _leftAttackSprite;

	private CollisionShape2D _currentAttackAreaCollider;
	private Sprite2D _currentAttackSprite;
	public bool CanSwitchAttackDirection = true;

	private bool _canHit = false;
	public bool CanSlash { get; private set; }

	private Player _player;
	private PlayerStats _playerStats => _player.CurrentPlayerStats;

	private PlayerMovementController _movementController;

	public override void _Ready()
	{
		CanSlash = true;

		_inputManager = GetNode<IInputManager>("/root/InputManager");
		_attackArea = GetNode<Area2D>(_attackAreaPath);
		_rightAttackAreaCollider = GetNode<CollisionShape2D>(_rightAttackAreaColliderPath);
		_leftAttackAreaCollider = GetNode<CollisionShape2D>(_leftAttackAreaColliderPath);

		_rightAttackSprite = GetNode<Sprite2D>(_rightAttackSpritePath);
		_leftAttackSprite = GetNode<Sprite2D>(_leftAttackSpritePath);

		_slashBuffer = GetNode<Timer>(_slashBufferPath);
		_slashBuffer.WaitTime = _playerStats.SlashBuffer;

		_attackArea.AreaEntered += _OnSlashHit;
		_attackArea.CollisionMask = (uint) PhysicsLayers.HittableLayer;
		_attackArea.CollisionLayer = (uint) PhysicsLayers.UntouchableLayer;

		HitEvent += _OnHit;
		SlashEvent += _EnableSlashSprite;
		FinishSlashEvent += _DisableSlashSprite;
	}

	public void Initialize(Player player) {
		_player = player;
		// _playerStats = player.PlayerStatsResource;
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
		SlashEvent?.Invoke(_playerStats.SlashDamage, _playerStats.SlashTime, _playerStats.SlashRange);

		_canHit = true;
		_currentAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
		
		GetTree().CreateTimer(_playerStats.SlashTime).Timeout += _FinishSlash;
		// await ToSignal(GetTree().CreateTimer(SlashTime), SceneTreeTimer.SignalName.Timeout);
	}

	private void _FinishSlash() {
		_canHit = false;
		_rightAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		_leftAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		CanSwitchAttackDirection = true;
		FinishSlashEvent?.Invoke();
	}

	public void StartSlashCooldown() {
		CanSlash = false;
		GetTree().CreateTimer(_playerStats.SlashCooldown).Timeout += _FinishSlashCooldown;
	}

	private void _FinishSlashCooldown() {
		CanSlash = true; 
	}

	private void _OnSlashHit(Node2D hit) {
		if(!_canHit || !(hit is IHittable)) {
			return;
		}

		HitEvent?.Invoke((IHittable) hit, _playerStats.SlashDamage, this.GlobalPosition.X > hit.GlobalPosition.X ? 1 : -1, _playerStats.SlashPostureDamage);
	}

	private void _OnHit(IHittable hittable, int damage, int direction, int postureDamage) {
		hittable.OnHit(_player, damage, direction, postureDamage); // Maybe flip direction, not sure yet
		// TODO: Slash Particle, Screen Shake
	}

	private void _UpdateAttackCollider() {
		// if(!CanSwitchAttackDirection) {
		//	return;
		// }
		
		// GD.Print(_movementController.Direction);

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
		// _currentAttackSprite.Visible = false;
		
		_rightAttackSprite.Visible = false;
		_leftAttackSprite.Visible = false;
	}
}
