using Godot;


[Tool]
public partial class OGeneric: Node2D
{
    // # Public Variables # //
    public enum ObjectType
    {
        None = 0,
        Pickable = 1,
        Trigger = 2,
        Document = 3,
        Key = 4,
        Light = 5,
        TriggerGameActionManager = 6
    }
    // # ---------------- # // 

    // # Private Variables # //
    private ObjectType _objectType = ObjectType.None;
    private bool _isMouseInside = false;
    private string _content = string.Empty;
    private ODoor _door_to_unlock = null;
    private GameActionManager game_action_manger = null;
    private PointLight2D object_light = null;
    // # UI Variables # //
    private CanvasLayer canvas = null;
    private PaperUi paper_ui = null;

    // nodes
    private Area2D areaNode = null;
    // # ------------------ # //


    // # Export Variables # //
    [ExportGroup( "Object Settings" )]
    [Export( PropertyHint.Enum )]
    public ObjectType objectType // type of the object, this is usefull for adding diffrent types of object inside the game scene without having to create new ones everytime.
    {
        get => _objectType;
        set => _objectType = value;
    }

    [ExportSubgroup("Document")]
    [Export(PropertyHint.MultilineText)]
    public string Content
    {
        get => _content;
        set => _content = value;
    }

    [ExportSubgroup("Key")]
    [Export]
    public ODoor DoorToUnlock
    {
        get => _door_to_unlock;
        set => _door_to_unlock = value;
    }

    [ExportSubgroup("Light")]
    [Export]
    public PointLight2D ObjectLight
    {
        get => object_light;
        set => object_light = value;
    }

    [ExportSubgroup("Triiger Game Action Manager")]
    [Export]
    public GameActionManager GameActionManager
    {
        get => game_action_manger;
        set => game_action_manger = value;
    }
    // # ---------------- # //


    // Called when the node enters the scene tree for the first time.
    public override void _Ready( )
    {
        // check if the objArea is inside the object, if not return an error message.
        areaNode = FindChild( "objArea", true ) as Area2D;
        if( areaNode == null )
        {
            ErrorHandler.ThrowError( $"[OGeneric.cs/_Ready] - Failed to find the Area2D. Make sure to add one as a child of the object.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }
        else
        {
            areaNode.Connect( "mouse_entered", new Callable( this, nameof( OnMouseEntered ) ) );
            areaNode.Connect( "mouse_exited", new Callable( this, nameof( OnMouseExited ) ) );
        }

        // check if the canvas layer and paper_ui exists, if not return an error message.
        canvas = FindChild( "CanvasLayer", true ) as CanvasLayer;
        if( canvas == null )
        {
            ErrorHandler.ThrowError( $"[OGeneric.cs/_Ready] - Failed to find the canvas layer. Make sure to add one as a children of the object.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }
        else paper_ui = canvas.FindChild( "paper_ui", true ) as PaperUi;

        if( paper_ui == null )
        {
            ErrorHandler.ThrowError( $"[OGeneric.cs/_Ready] - Failed to find the paper_ui. Make sure to add one in a canvas layer as a children of the object.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }
        else paper_ui.UpdateContent( _content );


    }


    //- FIXME Important, apparently i've discovered a bug: When the player is moving (so if an input key is being pressed) while also this input key is being pressed. I've tried to turn on and off a lamp while moving the player, and basically if i hold the mouse left button while the player is moving, the mouse input fires everytime while the mouse left button is still being pressed, i dont know if i want to keep this feature but it can be annoying.
    public override void _Input( InputEvent @event )
    {
        if (@event is InputEventMouseButton inputEventMouseButton)
        {
            if (inputEventMouseButton.ButtonIndex == MouseButton.Left && inputEventMouseButton.Pressed && _isMouseInside == true)
            {
                HandleMouseEvent( );
            }
        }
    }


    private void HandleMouseEvent()
    {
        // TODO // * refactor this. Add the specific method for each logic.
        switch( _objectType )
        {
            case ObjectType.Document:
            {
                paper_ui.Visible = true;
                break;
            }

            case ObjectType.Key:
            {
                if( _door_to_unlock == null ) return;
                _door_to_unlock.IsLocked = false;
                EventHandler.TriggerEvent( EventHandler.EventType.MOUSE_EXITED ); // here just to hide the player object info panel. KEEP IT HERE
                this.QueueFree( );
                break;
            }

            case ObjectType.Light:
            {
                ToggleLight( );
                break;
            }
            case ObjectType.Pickable:
            {
                if( game_action_manger == null ) return;
                game_action_manger.TriggerAction( );
                break;
            }
            case ObjectType.TriggerGameActionManager:
            {
                if( game_action_manger == null ) return;
                game_action_manger.TriggerAction( );
                break;
            }
        }
    }

    private void OnMouseEntered( )
    {
        EventHandler.TriggerEvent( EventHandler.EventType.MOUSE_ENTERED, this );
        _isMouseInside = true;
    }

    private void OnMouseExited( )
    {
        EventHandler.TriggerEvent( EventHandler.EventType.MOUSE_EXITED );
        _isMouseInside = false;
    }

    private void ToggleLight()
    {
        if (ObjectLight == null) return;
        else ObjectLight.Visible = !ObjectLight.Visible;
    }
}