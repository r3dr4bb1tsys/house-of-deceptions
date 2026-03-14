using Godot;
using System;

public partial class OPlayer : CharacterBody2D
{

	// # --------public variables-----------#
	public bool ReadyToTeleport = false;
	// # ------------------------------------#

// # -----------Exports------------------#
	// Speed Variable
	[Export] public int Speed = 50;
    // # --------------------------------#

    public override void _Ready()
    {
		Area2D playerInteractionArea = FindChild("playerInteactableArea", true) as Area2D;
		playerInteractionArea.Connect("body_entered", new Callable(this, nameof(OnPlayerInteractionAreaBodyEntered)));
		playerInteractionArea.Connect("body_exited", new Callable(this, nameof(OnPlayerInteractionAreaBodyExited)));
    }


	public override void _PhysicsProcess(double delta)
	{

		var direction = Input.GetVector(PlayerActions.MoveLeft, PlayerActions.MoveRight, PlayerActions.MoveUp, PlayerActions.MoveDown);
		
		Velocity = direction.Normalized() * Speed;
		MoveAndSlide();
	}

	public void TeleportTo(Vector2 newPosition)
	{
		Position = newPosition;
	}

	public struct PlayerActions
	{
		public const string MoveRight = "playerMoveRight";
		public const string MoveLeft = "playerMoveLeft";
		public const string MoveUp = "playerMoveUp";
		public const string MoveDown = "playerMoveDown";
		public const string Interact = "playerInteract";
	}


	// # -----------Signals------------------#
	private void OnPlayerInteractionAreaBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Door")) ReadyToTeleport = true;	
	}

	private void OnPlayerInteractionAreaBodyExited(Node2D body)
	{
		ReadyToTeleport = false;
	}
	// # ------------------------------------#
}
