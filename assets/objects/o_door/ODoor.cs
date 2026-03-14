using System.ComponentModel;
using System.Runtime.Serialization.Formatters;
using Godot;
public partial class ODoor : Node2D
{

// # --------public variables-----------#
	Marker2D PlayerExitMarker = null;

// # --------private variables-----------#
	private ODoor _exit = null;
	private Vector2 _player_exit_position = Vector2.Zero;
// # ------------------------------------#

// # -----------Exports------------------#
	[Export] private ODoor Exit
	{
		get => _exit;
		set => _exit = value;
	}
// # ------------------------------------#

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		PlayerExitMarker = FindChild("player_exit_marker", true) as Marker2D;
		if (PlayerExitMarker == null)
		{
			GD.Print($"Failed to find an exit marker for the current door: [{this.Name}].");
			return;
		}

		// check for an exit, if there is none return an error message.
		if (Exit == null)
		{
			GD.PrintErr("An exit must be assigned to the current door.");
			return;
		}


		// check if the exit has been assigned to the current door, and if it has a marker set. if not return an error message. Otherwise teleport the player to the exit position defined by the marker.
		if (Exit == null)
		{
			GD.PrintErr($"An exit must be assigned to the current door: [{this.Name}].");
			return;
		}
		if (Exit.PlayerExitMarker == null)
		{
			GD.Print($"Failed to find the player exit marker inside the ODoor node: [{Exit.Name}]. Make sure to add a marker2D node as a child of the door and name it 'player_exit_marker'.");
			return;
		}
		else
		{
			_player_exit_position = Exit.PlayerExitMarker.GlobalPosition;
		}

	}

    public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed(OPlayer.PlayerActions.Interact))
		{
			// search for a player in the current scene.
			OPlayer player = GetTree().CurrentScene.FindChild("oPlayer", true) as OPlayer;
			// if no player is found return an error message. Otherwise, teleport the player to the exit position of the exit door.
			if (player == null)
			{
				GD.PrintErr($"Failed to find the player object in the current scene [{GetTree().CurrentScene.Name}]. Make sure to add one. ");
				return;
			}
			else
			{
				if (player.ReadyToTeleport == true) 
				{
					player.TeleportTo(_player_exit_position);
				}
			}
		}
	}

}
