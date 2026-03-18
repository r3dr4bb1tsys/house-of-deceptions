using Godot;

[Tool]
public partial class ODoor : Node2D
{
	// # Private Variables # //
	private Area2D _doorInteractionArea;
	private ODoor _linkedDoor;
	private Vector2 _localExitMarkerPosition;
	// # ----------------- # //


	// # Public Variables # //
	public Marker2D _localExitMarker;
	public Marker2D _linkedExitMarker;
	// # ---------------- # // 

	// # Export Varianles # //
	[Export] public ODoor LinkedDoor
	{
		get => _linkedDoor;
		set
		{
			_linkedDoor = value;
		}
	}
	[Export] public Vector2 LocalExitMarkerPosition
	{
		get => _localExitMarkerPosition;
		set => _localExitMarkerPosition = value;
	}
	// # ---------------- # //

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		// check for the door's interaction area existence, if none is found return an error message.
		_doorInteractionArea = FindChild("doorInteractionArea") as Area2D;
		if (_doorInteractionArea == null)
		{
			GD.PrintErr($"[ODoor.cs/Ready] - Failed to find the interaction area for the door: [{this.Name}]. Make sure to add one. ");
			return;
		}
		
		// check for the existence of a linked door, if none is found return an error message.
		if (_linkedDoor == null)
		{
			GD.PrintErr($"[ODoor.cs/Ready] - Failed to find a linked door for [{this.Name}]. Make sure to assign one through the inspector.");
			return;
		}

		// check for the existence of a local exit marker, if none is found return an error message.
		_localExitMarker = FindChild("localExitPositionMarker", true) as Marker2D;
		if (_localExitMarker == null)
		{
			GD.PrintErr($"[ODoor.cs/Ready] - Failed to find a local exit marker for door: [{this.Name}]. Make sure to add one as a child of the door.");
			return;
		}
		_localExitMarker.Position = _localExitMarkerPosition;
	}
}
