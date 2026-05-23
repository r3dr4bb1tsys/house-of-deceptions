// TODO: This is a generic todo, so it applies to all the .cs files in the project, please refactor the code so it's more clean and readable, add more comments where needed and remove any redundant code. 

using Godot;
public partial class OPlayer : CharacterBody2D
{

	/// <summary>
	/// Standard player actions for movement and interaction. These should be set up in the project input map for the player to work properly.
	/// </summary>
	public struct PlayerActions
	{
		public const string MoveRight = "playerMoveRight";
		public const string MoveLeft = "playerMoveLeft";
		public const string MoveUp = "playerMoveUp";
		public const string MoveDown = "playerMoveDown";
		public const string Interact = "playerInteract";
	}

	// # Private Variables # //
	private ODoor _enteredDoor;
	private AnimatedSprite2D spriteAnimator;
	private string lastAnimation = string.Empty;

//TODO: debug only variables
	private Label debug_error_text = null;
	private Label debug_info_text = null;
	private string debug_info;
	private Area2D area2D = null;
	private Vector2 newPosition;
	private Camera2D mainCameraRef = null;
	// # ----------------- # //

	// # public variables # // 
	// this variable is used to check wether the player is ready for teleportation or not, this is used in door transportation for example. 
	public bool _readyToTeleport = false;
	// # ---------------- # //

	// # Exports # //
	// Speed Variable
	[Export] public int Speed = 50;
	// # ------- # //

	public override void _Ready()
	{
		// check if the playerInteactionArea is inside the player, if not return an error message.
		Area2D playerInteactionArea = FindChild("playerInteactionArea", true) as Area2D;
		if (playerInteactionArea == null)
		{
			ErrorHandler.ThrowError($"[OPlayer.cs/_Ready] - Failed to find the interaction area for the player [{this.Name}]. Make sure to add one as a child of the player.", ErrorHandler.ErrorType.GENERIC_ERROR);
			return;
		}
		playerInteactionArea.Connect("area_entered", new Callable(this, nameof(InteractionAreaEntered)));
		playerInteactionArea.Connect("area_exited", new Callable(this, nameof(InteractionAreaExited)));

		// Check if the playerInteractionShape is inside the player, if not return an error message.
		CollisionShape2D playerInteractionShape = FindChild("playerInteractionShape", true) as CollisionShape2D;
		if (playerInteractionShape == null)
		{
			ErrorHandler.ThrowError($"Failed to find the interaction shape for the player [{this.Name}]. Make sure to add one as a child of the player.", ErrorHandler.ErrorType.GENERIC_ERROR);
			return;
		}

		// check if the spriteAnimator is inside the player, if not return an error message. 
		spriteAnimator = FindChild("spriteAnimator", true) as AnimatedSprite2D;
		if (spriteAnimator == null)
		{
			ErrorHandler.ThrowError($"[OPlayer.cs/_Ready] - Failed to find the sprite animator for the player. Make sure to add one as a child of the player.", ErrorHandler.ErrorType.GENERIC_ERROR);
			return;
		}
		
		mainCameraRef = GetParent().FindChild("mainSceneCamera", true) as Camera2D;
		if (mainCameraRef == null)
		{
			ErrorHandler.ThrowError("[OPlayer.cs/_Ready] - Failed to retrieve the reference to the main camera. Make sure to add one as a child of the main scene.", ErrorHandler.ErrorType.GENERIC_ERROR);
			return;
		}




		//TODO: debug only, remove or add a check for debug and release builds.
		// check if the debug_error_text is inside the player, if not return an error message.
		debug_error_text = FindChild("debug_error_text", true) as Label;
		if (debug_error_text == null)
		{
			ErrorHandler.ThrowError($"[OPlayer.cs/_Ready] - Failed to find the debug_error_text label. Make sure to add one as a child of the player.", ErrorHandler.ErrorType.GENERIC_ERROR);
			return;
		}
		
		// check if the debug_info_text is inside the player, if not return an error message.
		debug_info_text = FindChild("debug_info_text", true) as Label;
		if (debug_info_text == null)
		{
			ErrorHandler.ThrowError($"[OPlayer.cs/_Ready] - Failed to find the debug_info_text label. Make sure to add one as a child of the player.", ErrorHandler.ErrorType.GENERIC_ERROR);
			return;
		}

		EventHandler.ListenForEvent(EventHandler.EventType.ERROR_OCCURRED, UpdateDebugErrorText);
		EventHandler.ListenForEvent(EventHandler.EventType.PLAYER_INFO_UPDATED, UpdateDebugInfoText);
		EventHandler.TriggerEvent(EventHandler.EventType.PLAYER_INFO_UPDATED); // just to trigger an initial update of the debug_info_text.
	}




    // standard physics process that runs every frame.
    public override void _PhysicsProcess(double delta)
	{
		//* IMPORTANT NOTE: This is a basic check for player movement. This cheks if the player is allowed to move or not. 
		if (SharedVariables.canPlayerMove == false) return;

		// collect the direction of the player based on the action pressed.
		var direction = Input.GetVector(PlayerActions.MoveLeft, PlayerActions.MoveRight, PlayerActions.MoveUp, PlayerActions.MoveDown);

		// normalize the player velocity so that diagonal movements isnt sqrt(2) faster. 
		Velocity = direction.Normalized() * Speed;

		// apply the correct animation based on the player movement direction.

		if (Velocity == Vector2.Zero) // velocity is zero meaning the player is not moving, therefore the player is idle, so we play the idle animation. 
		{
			if (lastAnimation.ToLower().Contains("side"))   // the last animation was walkSide;
			{
				spriteAnimator.Play("sideIdle");
			}
			else    // the last animation was either walkdown or walkup. 
			{
				if (lastAnimation.ToLower().Contains("up"))
				{
					spriteAnimator.Play("backIdle");
				}
				else if (lastAnimation.ToLower().Contains("down"))
				{
					spriteAnimator.Play("frontIdle");
				}
			}
		}
		if (direction.X != Vector2.Zero.X)  // x direction is not zero meaning the player is moving either left or right
		{
			spriteAnimator.Play("walkSide");
			if (direction.X < 0) // player is moving left so we flip the sprite horizontaly to the left.
			{
				spriteAnimator.FlipH = true;
			}
			else // player is moving right so we flip the sprite horizontaly to the right.
			{
				spriteAnimator.FlipH = false;
			}
		}
		if (direction.Y != Vector2.Zero.Y) // y direction is not zero meaning the player is moving either up or down.
		{
			if (direction.Y < 0) // player is moving up so we play the walkUp animation.
			{
				spriteAnimator.Play("walkUp");
			}
			else // player is moving down so we play the walkDown animation.
			{
				spriteAnimator.Play("walkDown");
			}
		}
		lastAnimation = spriteAnimator.Animation;
		MoveAndSlide();

		//TODO: debug only, remove or add a check for debug and release builds.
		EventHandler.TriggerEvent(EventHandler.EventType.PLAYER_INFO_UPDATED); // trigger the debug_info_update for every frame
	}

	public override void _Input(InputEvent @event)
	{
		//* Handle the door's interaction input. 
		if (@event.IsActionPressed(PlayerActions.Interact))
		{
			// check the door is reade to teleport the player, if not, return an error message. This is crucial as we dont want the player to be teleported even if they're not colliding with any door.
			if (this._readyToTeleport == false)
			{
				ErrorHandler.ThrowError($"[OPlayer.cs/Interaction Input] - The player is not ready for teleportation as it's not colliding with a valid door. this._readyToTeleport: {this._readyToTeleport}", ErrorHandler.ErrorType.GENERIC_ERROR);
				return;
			}

			// teleport the player to the new position.
			newPosition = _enteredDoor.LinkedDoor._localExitMarker.GlobalPosition;
			this.TeleportTo(newPosition);


			//* Change the camera's parent to the linked door's parent, and switch it's position to the new parent's position.
			Node2D  doorParent = _enteredDoor.LinkedDoor.GetParent() as Node2D; // get the door parent
			Node2D newParent = doorParent.GetParent() as Node2D;
			if (newParent == null || doorParent == null)
			{
				ErrorHandler.ThrowError("[OPlayer.cs/_Input] - Failed to get a reference to the door's parent.", ErrorHandler.ErrorType.GENERIC_ERROR);
				return;
			}
			mainCameraRef.Reparent(newParent);
			mainCameraRef.GlobalPosition = newParent.Position; // switch the camera position.

			EventHandler.TriggerEvent(EventHandler.EventType.PLAYER_INFO_UPDATED);
		}
		

		//TODO: debug only, remove or add a check for debug and release builds.
		if (@event.IsActionPressed("debug_clear_error"))
		{
			debug_error_text.Text = string.Empty;
			ErrorHandler.GetErrorHistory().Clear();
		}
	}


	// # Signals # //
	private void InteractionAreaEntered(Area2D area)
	{
		// get the door from wich is parent of the area that entered the player interaction area.
		area2D = area;	//TODO: debug only, remove or add a check for debug and release builds.
		_enteredDoor = area.GetParent() as ODoor;
		newPosition = _enteredDoor.LinkedDoor._localExitMarker.GlobalPosition;
		if (_enteredDoor.IsInGroup("Door"))
		{
			this._readyToTeleport = true;
		}

		//TODO: debug only, remove or add a check for debug and release builds.
		EventHandler.TriggerEvent(EventHandler.EventType.PLAYER_INFO_UPDATED);
	}
	private void InteractionAreaExited(Area2D area)
	{
		this._readyToTeleport = false;
		
		//TODO: debug only, remove or add a check for debug and release builds.
		area2D = null;
		_enteredDoor = null;
		newPosition = Vector2.Zero;
		EventHandler.TriggerEvent(EventHandler.EventType.PLAYER_INFO_UPDATED);
	}
	// # -------- # //
	

	/// <summary>
	/// Standard fucntion for player teleportation. 
	/// </summary>
	/// <param name="newPosition"></param>
	public void TeleportTo(Vector2 newPosition)
	{
		Position = newPosition.Round();
	}

//TODO: debug only, remove or add a check for debug and release builds.
	private void UpdateDebugErrorText()
	{
		debug_error_text.Text = string.Empty;
		foreach (string error in ErrorHandler.GetErrorHistory())
		{
			debug_error_text.Text = string.Join("\n", ErrorHandler.GetErrorHistory());
		}
	}

//TODO: debug only, remove or add a check for debug and release builds.
	private void UpdateDebugInfoText()
	{
		debug_info =
			$"""
			[PLAYER]
			[Player Position]: {this?.Position ?? Vector2.Zero}
			[Current Velocity]: {this.Velocity}
			[Interacting With]: {area2D?.Name ?? "None"}
			[this._readyToTeleport]: {this._readyToTeleport}
			
			--Player Animation--
			[Current Animation]: {spriteAnimator.Animation}
			[Flipped]: {spriteAnimator.FlipH}
			
			[DOOR]
			[Door Name]: {_enteredDoor?.Name ?? "None"}
			[Door Out Position]: {this?.newPosition ?? Vector2.Zero}
			[Linked Door]: {_enteredDoor?.LinkedDoor?.Name ?? "None"}

			[CAMERA]
			[Camera Name]: {mainCameraRef?.Name ?? "None"}
			[Position]: {mainCameraRef?.GlobalPosition ?? Vector2.Zero}
			[Parent]: {mainCameraRef.GetParent()?.Name ?? "None"}
			[Parent's type]: {mainCameraRef.GetParent()?.GetType() ?? null}
			[Parent's position]: {(mainCameraRef?.GetParent() as Node2D)?.GlobalPosition ?? Vector2.Zero}

			[ERROR]
			[Error Count]: {ErrorHandler.GetErrorHistory().Count}
			""";

		debug_info_text.Text = debug_info;
	}
}