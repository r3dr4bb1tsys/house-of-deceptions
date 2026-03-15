using Godot;
using System;

public partial class OPlayer : CharacterBody2D
{

	// # --------public variables-----------#
	// # ------------------------------------#

// # -----------Exports------------------#
	// Speed Variable
	[Export] public int Speed = 50;
    // # --------------------------------#

    public override void _Ready()
    {
		// Check if the playerInteractionArea is inside the player, if not return an error message.
		CollisionShape2D playerInteractionShape = FindChild("playerInteractionShape", true) as CollisionShape2D;
		if (playerInteractionShape == null)
		{
			GD.PrintErr($"Failed to find the player interaction shape for the player [{this.Name}]. Make sure to add one as a child of the player.");
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

	/// <summary>
	/// Standard fucntion for player teleportation. 
	/// </summary>
	/// <param name="newPosition"></param>
	public void TeleportTo(Vector2 newPosition)
	{
		Position = newPosition;
	}

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

}
