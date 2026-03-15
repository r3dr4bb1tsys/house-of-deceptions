using Godot;
using System;

public partial class ODoor : Node2D
{
	// # Private Variables # //
	private Area2D _doorInteractionArea;
	private bool _readyToTeleport;
	private ODoor _linkedDoor;
	// # ----------------- # //


	// # Public Variables # //
	public Marker2D _localExitMarker;
	public Marker2D _linkedExitMarker;
	// # ---------------- # // 

	// # Export Varianles # //
	[Export] ODoor LinkedDoor
	{
		get => _linkedDoor;
		set
		{
			_linkedDoor = value;
		}
	}
	// # ---------------- # //

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// check for the door's interaction area existence, if none is found return an error message, otherwise, connect the body_entered and body_exited signals of the area.
		_doorInteractionArea = FindChild("doorInteractionArea") as Area2D;
		if (_doorInteractionArea == null)
		{
			GD.PrintErr($"[Ready] - Failed to find the interaction area for the door: [{this.Name}]. Make sure to add one. ");
			return;
		}
		_doorInteractionArea.Connect("area_entered", new Callable(this, nameof(InteractionAreaEntered)));
		_doorInteractionArea.Connect("area_exited", new Callable(this, nameof(InteractionAreaExited)));

		// check for the existence of a linked door, if none is found return an error message, otherwise, use the linked door's marker position as the exit position for the player teleportation.
		if (_linkedDoor == null)
		{
			GD.PrintErr($"[Ready] - Failed to find a linked door for [{this.Name}]. Make sure to assign one through the inspector.");
			return;
		}

		// check for the existence of a local exit marker, if none is found return an error message.
		_localExitMarker = FindChild("localExitPositionMarker", true) as Marker2D;
		if (_localExitMarker == null)
		{
			GD.PrintErr($"[Ready] - Failed to find a local exit marker for door: [{this.Name}]. Make sure to add one as a child of the door.");
			return;
		}
	}

    // # Input # //
    public override void _Input(InputEvent @event)
    {
		// Handle the door's interaction input. 
		if (@event.IsActionPressed(OPlayer.PlayerActions.Interact))
		{
			// check the door is reade to teleport the player, if not, return an error message. This is crucial as we dont want the player to be teleported even if they're not colliding with any door.
			if (_readyToTeleport == false) //! // FIXME: This check returns false even if the player is colliding with the door and the _readyToTeleport variable is set to true, i think this is because it checks for all the door's _readyToTeleport variables and not just the one that the player is colliding with, need to find a way to check for the specific door that the player is colliding with instead of all the doors ins the scene. 
			{
				GD.PrintErr($"[Interaction Input] - The door is not ready to teleport the player as it's not colliding with it. returning...");
				return;
			}

			// Search for a player inside the current scene root. This is crucial for the door teleportation to work, as we need to access the player's teleportation function and position. If no player is found, return an error message.
			OPlayer player = GetTree().Root.FindChild("oPlayer", true) as OPlayer;
			if (player == null)
			{
				GD.PrintErr($"[Interaction Input] - Failed to find the player node inside the scene: [{GetTree().CurrentScene.Name}]. Make sure the player is instanced in the current scene.");
				return;
			}

			// teleport the player to the new position.
			Vector2 newPosition = _linkedDoor._localExitMarker.Position;
			GD.Print($"[Interaction Input] - Teleporting player to the new door's position: [{newPosition}]");
			player.TeleportTo(newPosition);
		}
    }


	// # Signals # //
	private void InteractionAreaEntered(Area2D area)
	{
		GD.Print(new string('-', 100));
		GD.Print($"[Area entered signal] - The area [{area.Name}] has entered [{this.Name}]'s interaction area.");
		GD.Print($"[{area.Name}]'s groups: {string.Join(",", area.GetGroups())}");
		if (area.IsInGroup("PlayerInteactionArea"))
		{
			_readyToTeleport = true;
			GD.Print($"[Area entered signal] - The player is colliding with door: [{this.Name}].");
		}
		GD.Print($"[Area entered signal] -  _readyToTeleport has been set to: {_readyToTeleport}\n" + new string('-', 100) + "\n");
	}
	private void InteractionAreaExited(Area2D area)
	{
		_readyToTeleport = false;
		GD.Print(new string('-', 100));
		GD.Print($"[Area exited signal] - The player is no longer colliding with door: [{this.Name}].");
		GD.Print($"[Area exited signal] - _readyToTeleport has been set to: {_readyToTeleport}\n" + new string('-', 100) + "\n");	
	}
	// # -------- # //
}
