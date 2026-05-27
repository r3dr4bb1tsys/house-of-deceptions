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
        Document = 3
    }
    // # ---------------- # // 

    // # Private Variables # //
    private ObjectType _objectType = ObjectType.None;
    private Texture2D _objTexture = null;
    private AnimatedSprite2D _objectAnimatedSprite = null;
    private bool _isMouseInside = false;
    private string _content = string.Empty;

    // # UI Variables # //
    private CanvasLayer canvas = null;
    private PaperUi paper_ui = null;

    // nodes
    private Area2D areaNode = null;
    private Sprite2D spriteNode = null;
    // # ------------------ # //


    // # Export Variables # //
    [ExportGroup( "Object Settings" )]
    [Export( PropertyHint.Enum )]
    public ObjectType objectType // type of the object, this is usefull for adding diffrent types of object inside the game scene without having to create new ones everytime.
    {
        get => _objectType;
        set => _objectType = value;
    }
    [Export]
    public Texture2D objTexture // if we want a little static sprite to display along with the object
    {
        get => _objTexture;
        set => _objTexture = value;
    }
    [Export]
    public AnimatedSprite2D objectAnimatedSprite // if we want a little animation for our sprite instead of a static picture.
    {
        get => _objectAnimatedSprite;
        set => _objectAnimatedSprite = value;
    }
    [Export]
    public string Content
    {
        get => _content;
        set => _content = value;
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


        // check if the sprite2d is inside the object, if not return an error message.
        spriteNode = FindChild( "objSprite", true ) as Sprite2D;
        if( spriteNode == null || objTexture == null )
        {
            ErrorHandler.ThrowError( $"[OGeneric.cs/_Ready] - Failed because {( spriteNode == null ? "spriteNode" : "objTexture" )} is null.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }
        else spriteNode.Texture = objTexture;

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
        else
        {
            paper_ui.UpdateContent( _content );
        }
    }


    public override void _Input( InputEvent @event )
    {
        if( Input.IsMouseButtonPressed( MouseButton.Left ) && _isMouseInside == true )   // left clicked on the object and interact with it.
        {
            switch( _objectType )
            {
                case ObjectType.Document:
                {
                    // see document content logic.
                    paper_ui.Visible = true;    // TODO: Simple show/hide for the document, implement actual logic once this is done.
                    break;
                }
            }
        }
    }

    private void OnMouseEntered( )
    {
        EventHandler.TriggerEvent( EventHandler.EventType.AREA_ENTERED, this );
        _isMouseInside = true;
    }

    private void OnMouseExited( )
    {
        EventHandler.TriggerEvent( EventHandler.EventType.AREA_ENTERED );
        _isMouseInside = false;
    }
}