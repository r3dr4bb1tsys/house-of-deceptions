// TODO: This is a generic todo, so it applied to all the .cs files in the project, please refactor the code so it's more clean and readable, add more comments where needed and remove any redundant code. 

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
			GD.PrintErr($"[OPlayer.cs/_Ready] - Failed to find the interaction area for the player [{this.Name}]. Make sure to add one as a child of the player.");
			return;
		}
		playerInteactionArea.Connect("area_entered", new Callable(this, nameof(InteractionAreaEntered)));
		playerInteactionArea.Connect("area_exited", new Callable(this, nameof(InteractionAreaExited)));

		// Check if the playerInteractionShape is inside the player, if not return an error message.
		CollisionShape2D playerInteractionShape = FindChild("playerInteractionShape", true) as CollisionShape2D;
		if (playerInteractionShape == null)
		{
			GD.PrintErr($"Failed to find the interaction shape for the player [{this.Name}]. Make sure to add one as a child of the player.");
			return;
		}
    }


	// standard physics process that runs every frame.
	public override void _PhysicsProcess(double delta)
	{
		// collect the direction of the player based on the action pressed.
		var direction = Input.GetVector(PlayerActions.MoveLeft, PlayerActions.MoveRight, PlayerActions.MoveUp, PlayerActions.MoveDown);
		
		// normalize the player velocity so that diagonal movements isnt sqrt(2) faster. 
		Velocity = direction.Normalized() * Speed;
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
    {
		// Handle the door's interaction input. 
		if (@event.IsActionPressed(PlayerActions.Interact))
		{
			// check the door is reade to teleport the player, if not, return an error message. This is crucial as we dont want the player to be teleported even if they're not colliding with any door.
			if (this._readyToTeleport == false)
			{
				GD.PrintErr($"[OPlayer.cs/Interaction Input] - The player is not ready for teleportation as it's not colliding with a valid door. this._readyToTeleport: {this._readyToTeleport}\t returning...");
				return;
			}

			// teleport the player to the new position.
			Vector2 newPosition = _enteredDoor.LinkedDoor._localExitMarker.GlobalPosition;
			GD.Print($"[OPlayer.cs/_Input] - Teleporting player to the new door's position: [{newPosition}]");
			this.TeleportTo(newPosition);
		}
    }
	

	// # Signals # //
	private void InteractionAreaEntered(Area2D area)
	{
		// get the door from wich is parent of the area that entered the player interaction area.
		_enteredDoor = area.GetParent() as ODoor;

		GD.Print(new string('-', 100));
		GD.Print($"[OPlayer.cs/Area entered signal] - The area [{area.Name}] has entered [{this.Name}]'s interaction area.");
		GD.Print($"[OPlayer.cs/Area entered signal] - [{_enteredDoor.Name}]'s groups: {string.Join(",", _enteredDoor.GetGroups())}");
		if (_enteredDoor.IsInGroup("Door"))
		{
			this._readyToTeleport = true;
			GD.Print($"[OPlayer.cs/Area entered signal] - The player is colliding with door: [{_enteredDoor.Name}].");
		}
		GD.Print($"[OPlayer.cs/Area entered signal] - this._readyToTeleport has been set to: {this._readyToTeleport}\n" + new string('-', 100) + "\n");
	}
	private void InteractionAreaExited(Area2D area)
	{
		this._readyToTeleport = false;
		GD.Print(new string('-', 100));
		GD.Print($"[OPlayer.cs/Area exited signal] - The player is no longer colliding with a door.");
		GD.Print($"[OPlayer.cs/Area exited signal] - this._readyToTeleport has been set to: {this._readyToTeleport}\n" + new string('-', 100) + "\n");	
	}
	// # -------- # //

	/// <summary>
	/// Standard fucntion for player teleportation. 
	/// </summary>
	/// <param name="newPosition"></param>
	public void TeleportTo(Vector2 newPosition)
	{
		Position = newPosition;
	}
}
