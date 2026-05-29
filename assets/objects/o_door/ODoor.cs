using Godot;

[Tool]
public partial class ODoor: Node2D
{
    // # Private Variables # //
    private Area2D _doorInteractionArea = null;
    private ODoor _linkedDoor = null;
    private Vector2 _localExitMarkerPosition = Vector2.Zero;
    private bool _is_locked = false;
    // # ----------------- # //


    // # Public Variables # //
    public Marker2D LocalExitMarker = null;
    // # ---------------- # // 

    // # Export Varianles # //
    [Export]
    public ODoor LinkedDoor
    {
        get => _linkedDoor;
        set
        {
            _linkedDoor = value;
        }
    }
    [Export]
    public bool IsLocked
    {
        get => _is_locked;
        set => _is_locked = value;
    }
    [Export]
    public Vector2 LocalExitMarkerPosition
    {
        get => _localExitMarkerPosition;
        set => _localExitMarkerPosition = value;
    }
    // # ---------------- # //

    // Called when the node enters the scene tree for the first time.
    public override void _Ready( )
    {

        // check for the door's interaction area existence, if none is found return an error message.
        _doorInteractionArea = FindChild( "doorInteractionArea" ) as Area2D;
        if( _doorInteractionArea == null )
        {
            ErrorHandler.ThrowError( $"[ODoor.cs/Ready] - Failed to find the interaction area for the door: [{this.Name}]. Make sure to add one. ", ErrorHandler.ErrorType.GENERIC_ERROR );
        }

        // check for the existence of a linked door, if none is found return an error message.
        if( _linkedDoor == null )
        {
            ErrorHandler.ThrowError( $"[ODoor.cs/Ready] - Failed to find a linked door for [{this.Name}]. Make sure to assign one through the inspector.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }

        // check for the existence of a local exit marker, if none is found return an error message.
        LocalExitMarker = FindChild( "localExitPositionMarker", true ) as Marker2D;
        if( LocalExitMarker == null )
        {
            ErrorHandler.ThrowError( $"[ODoor.cs/Ready] - Failed to find a local exit marker for door: [{this.Name}]. Make sure to add one as a child of the door.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }
        else LocalExitMarker.Position = _localExitMarkerPosition;
    }
}
