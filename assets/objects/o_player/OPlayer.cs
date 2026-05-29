using Godot;
public partial class OPlayer: CharacterBody2D
{

    /// <summary>
    /// Standard player actions for movement and interaction. These should be set up in the project input map for the
    /// player to work properly.
    /// </summary>
    public struct PlayerActions
    {
        public const string MoveRight = "playerMoveRight";
        public const string MoveLeft = "playerMoveLeft";
        public const string MoveUp = "playerMoveUp";
        public const string MoveDown = "playerMoveDown";
        public const string Run = "playerRun";
        public const string Interact = "playerInteract";
    }

    // # Private Variables # //
    #region Private Variables
    private Timer generalTimer = new Timer();
    private ODoor enteredDoor = null;
    private AnimatedSprite2D spriteAnimator;
    private string lastAnimation = string.Empty;
    private Label objectNameLabel = null;
    private Panel objectInfoPanel = null;
    private Panel doorInfoPanel = null;
    #endregion
    // # ----------------- # //

    //TODO debug only variables
    #region Debug Variables
    private Label debug_error_text = null;
    private Label debug_info_text = null;
    private string debug_info;
    private Area2D area2D = null;
    private Vector2 newPosition;
    private Camera2D mainCameraRef = null;
    private Control tabControl = null;
    private string objName = string.Empty;
    #endregion
    // # ----------------- # //

    // # public variables # // 
    // this variable is used to check wether the player is ready for teleportation or not, this is used in door transportation for example. 
    public bool readyToTeleport = false;
    // # ---------------- # //

    // # Exports # //
    // Speed Variable
    [Export] public int Speed = 50;
    // # ------- # //

    public override void _Ready( )
    {
        base._Ready( );

        // check if the playerInteactionArea is inside the player, if not return an error message.
        Area2D playerInteactionArea = FindChild("playerInteactionArea", true) as Area2D;
        if( playerInteactionArea == null )
        {
            ErrorHandler.ThrowError( $"[OPlayer.cs/_Ready] - Failed to find the interaction area for the player [{this.Name}]. Make sure to add one as a child of the player.", ErrorHandler.ErrorType.GENERIC_ERROR );
            return;
        }
        else
        {
            playerInteactionArea.Connect( "area_entered", new Callable( this, nameof( InteractionAreaEntered ) ) );
            playerInteactionArea.Connect( "area_exited", new Callable( this, nameof( InteractionAreaExited ) ) );
        }

        // Check if the playerInteractionShape is inside the player, if not return an error message.
        CollisionShape2D playerInteractionShape = FindChild("playerInteractionShape", true) as CollisionShape2D;
        if( playerInteractionShape == null )
        {
            ErrorHandler.ThrowError( $"Failed to find the interaction shape for the player [{this.Name}]. Make sure to add one as a child of the player.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }

        // check if the spriteAnimator is inside the player, if not return an error message. 
        spriteAnimator = FindChild( "spriteAnimator", true ) as AnimatedSprite2D;
        if( spriteAnimator == null )
        {
            ErrorHandler.ThrowError( $"[OPlayer.cs/_Ready] - Failed to find the sprite animator for the player. Make sure to add one as a child of the player.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }

        // search for the main camera inside the main scene, if fails to find one return an error message.
        mainCameraRef = GetParent( ).FindChild( "mainSceneCamera", true ) as Camera2D;
        if( mainCameraRef == null )
        {
            ErrorHandler.ThrowError( "[OPlayer.cs/_Ready] - Failed to retrieve the reference to the main camera. Make sure to add one as a child of the main scene.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }

        // check if the object name label is inside the player, if not return an error message.
        objectNameLabel = FindChild( "objectNameLabel", true ) as Label;
        if( objectNameLabel == null )
        {
            ErrorHandler.ThrowError( "[OPlayer.cs/_Ready] - Failed to find the object name label. Make sure to add one as a child of the player.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }

        // check if the object info panel is inside the player, if not return an error message.
        objectInfoPanel = FindChild( "objectInfoPanel", true ) as Panel;
        if( objectInfoPanel == null )
        {
            ErrorHandler.ThrowError( "[OPlayer.cs/_Ready] - Failed to find the object info panel. Make sure to add one as a child of the player.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }

        // check if the door info panel is inside the player, if not return an error message.
        doorInfoPanel = FindChild( "doorInfoPanel", true ) as Panel;
        if( doorInfoPanel == null )
        {
            ErrorHandler.ThrowError( "[OPlaye.cs/_Ready] - Failed to find the door info panel. Make sure to add one as a child of the player.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }
        else doorInfoPanel.Visible = false;

        // * mouse entering object event listeners
        EventHandler.ListenForEvent<OGeneric>( EventHandler.EventType.MOUSE_ENTERED, UpdateObjectNameLabel );
        EventHandler.ListenForEvent( EventHandler.EventType.MOUSE_EXITED, ResetObjectNameLabel );


        // TODO checks if debug mode is on.
        if( SharedVariables.IS_DEBUG_MODE == false ) return;

        //TODO debug only, remove or add a check for debug and release builds.
        // check if the debug_error_text is inside the player, if not return an error message.
        debug_error_text = FindChild( "debug_error_text", true ) as Label;
        if( debug_error_text == null )
        {
            ErrorHandler.ThrowError( $"[OPlayer.cs/_Ready] - Failed to find the debug_error_text label. Make sure to add one as a child of the player.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }

        // check if the debug_info_text is inside the player, if not return an error message.
        debug_info_text = FindChild( "debug_info_text", true ) as Label;
        if( debug_info_text == null )
        {
            ErrorHandler.ThrowError( $"[OPlayer.cs/_Ready] - Failed to find the debug_info_text label. Make sure to add one as a child of the player.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }

        // check if the tabControl is inside the player, if not return an error message.
        tabControl = FindChild( "tabControl", true ) as Control;
        if( tabControl == null )
        {
            ErrorHandler.ThrowError( $"[OPlayer.cs/_Ready] - Failed to find the tabControl. Make sure to add one as a child of the player.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }
        else tabControl.Visible = true;

        // TODO // * generic event listeners for debug
        EventHandler.ListenForEvent( EventHandler.EventType.ERROR_OCCURRED, UpdateDebugErrorText );
        EventHandler.ListenForEvent( EventHandler.EventType.PLAYER_INFO_UPDATED, UpdateDebugInfoText );

        // * intial trigger for some events
        CallDeferred( nameof( TriggerInitialEvents ) );
    }

    // just to trigger an initial update of the debug_info_text.
    private void TriggerInitialEvents( )
    {
        // TODO debug event trigger
        EventHandler.TriggerEvent( EventHandler.EventType.PLAYER_INFO_UPDATED );
    }


    // standard physics process that runs every frame.
    public override void _PhysicsProcess( double delta )
    {
        //* IMPORTANT NOTE: This is a basic check for player movement. This cheks if the player is allowed to move or not. 
        if( SharedVariables.canPlayerMove == false ) return;

        // collect the direction of the player based on the action pressed.
        var direction = Input.GetVector(PlayerActions.MoveLeft, PlayerActions.MoveRight, PlayerActions.MoveUp, PlayerActions.MoveDown);

        // normalize the player velocity so that diagonal movements isnt sqrt(2) faster. 
        Velocity = direction.Normalized( ) * Speed;

        // apply the correct animation based on the player movement direction.

        if( Velocity == Vector2.Zero ) // velocity is zero meaning the player is not moving, therefore the player is idle, so we play the idle animation. 
        {
            if( lastAnimation.ToLower( ).Contains( "side" ) )   // the last animation was walkSide;
            {
                spriteAnimator.Play( "sideIdle" );
            }
            else    // the last animation was either walkdown or walkup. 
            {
                if( lastAnimation.ToLower( ).Contains( "up" ) )
                {
                    spriteAnimator.Play( "backIdle" );
                }
                else if( lastAnimation.ToLower( ).Contains( "down" ) )
                {
                    spriteAnimator.Play( "frontIdle" );
                }
            }
        }
        if( direction.X != Vector2.Zero.X )  // x direction is not zero meaning the player is moving either left or right
        {
            spriteAnimator.Play( "walkSide" );
            if( direction.X < 0 ) // player is moving left so we flip the sprite horizontaly to the left.
            {
                spriteAnimator.FlipH = true;
            }
            else // player is moving right so we flip the sprite horizontaly to the right.
            {
                spriteAnimator.FlipH = false;
            }
        }
        if( direction.Y != Vector2.Zero.Y ) // y direction is not zero meaning the player is moving either up or down.
        {
            if( direction.Y < 0 ) // player is moving up so we play the walkUp animation.
            {
                spriteAnimator.Play( "walkUp" );
            }
            else // player is moving down so we play the walkDown animation.
            {
                spriteAnimator.Play( "walkDown" );
            }
        }
        lastAnimation = spriteAnimator.Animation;
        MoveAndSlide( );

        //TODO debug only, remove or add a check for debug and release builds.
        EventHandler.TriggerEvent( EventHandler.EventType.PLAYER_INFO_UPDATED ); // trigger the debug_info_update for every frame
    }

    public override void _Input( InputEvent @event )
    {
        //* Interaction
        if( @event.IsActionPressed( PlayerActions.Interact ) )
        {
            TryInteractWithDoor( );
            EventHandler.TriggerEvent( EventHandler.EventType.TUTORIAL_PLAYER_INTERACT, 3 );
        }

        // trigger movement related events
        if(
            @event.IsActionPressed( PlayerActions.MoveDown ) ||
            @event.IsActionPressed( PlayerActions.MoveUp ) ||
            @event.IsActionPressed( PlayerActions.MoveLeft ) ||
            @event.IsActionPressed( PlayerActions.MoveRight )
            )
        {
            EventHandler.TriggerEvent( EventHandler.EventType.TUTORIAL_PLAYER_MOVEMENT, 1 );
        }

        if( @event.IsActionPressed( PlayerActions.Run ) )
        {
            EventHandler.TriggerEvent( EventHandler.EventType.TUTORIAL_PLAYER_RUNNING, 2 );
        }


        if( SharedVariables.IS_DEBUG_MODE == false ) return;
        //TODO debug only, remove or add a check for debug and release builds.
        if( @event.IsActionPressed( "debug_clear_error" ) )
        {
            debug_error_text.Text = string.Empty;
            ErrorHandler.GetErrorHistory( ).Clear( );
        }

        //TODO debug only, remove or add a check for debug and release builds.
        if( @event.IsActionPressed( "toggle_debug_window" ) )
        {
            tabControl.Visible = !tabControl.Visible;
        }
    }


    // # Interaction Logic # //
    private void TryInteractWithDoor( )
    {
        //* DOOR LOGIC
        // check the door is reade to teleport the player, if not, return an error message. This is crucial as we dont want the player to be teleported even if they're not colliding with any door.


        if( enteredDoor.IsLocked == true )
        {
            if( doorInfoPanel == null ) return;
            doorInfoPanel.Visible = true;
            generalTimer.WaitTime = .8f;
            generalTimer.OneShot = true;
            AddChild( generalTimer );
            generalTimer.Timeout += ( ) =>
            {
                doorInfoPanel.Visible = false;
                RemoveChild( generalTimer );
            };
            generalTimer.Start( );
            return;
        }

        if( this.readyToTeleport == false )
        {
            ErrorHandler.ThrowError( $"[OPlayer.cs/Interaction Input] - The player is not ready for teleportation as it's not colliding with a valid door. this.readyToTeleport: {this.readyToTeleport}", ErrorHandler.ErrorType.GENERIC_ERROR );
            return;
        }

        // teleport the player to the new position.
        newPosition = enteredDoor.LinkedDoor.LocalExitMarker.GlobalPosition;
        this.TeleportTo( newPosition );


        //* Change the camera's parent to the linked door's parent, and switch it's position to the new parent's position.
        Node2D doorParent = enteredDoor.LinkedDoor.GetParent() as Node2D; // get the door parent
        Node2D newParent = doorParent.GetParent() as Node2D;
        if( newParent == null || doorParent == null )
        {
            ErrorHandler.ThrowError( "[OPlayer.cs/_Input] - Failed to get a reference to the door's parent.", ErrorHandler.ErrorType.GENERIC_ERROR );
            return;
        }
        mainCameraRef.Reparent( newParent );
        mainCameraRef.GlobalPosition = newParent.Position; // switch the camera position.

        EventHandler.TriggerEvent( EventHandler.EventType.PLAYER_INFO_UPDATED );
        return;
    }


    // # Signals # //

    private void UpdateObjectNameLabel( OGeneric _object )
    {
        if( objectNameLabel == null || objectInfoPanel == null ) return; // do nothing if the label is not found.
        objectNameLabel.Text = _object.Name;
        objectInfoPanel.Visible = true;
    }

    private void ResetObjectNameLabel( )
    {
        if( objectNameLabel == null || objectInfoPanel == null ) return;
        objectNameLabel.Text = string.Empty;
        objectInfoPanel.Visible = false;
    }


    private void InteractionAreaEntered( Area2D area )
    {
        // get the door from wich is parent of the area that entered the player interaction area.
        area2D = area;  //TODO debug only, remove or add a check for debug and release builds.
        enteredDoor = area.GetParent( ) as ODoor;
        if( enteredDoor != null && enteredDoor.IsInGroup( "Door" ) )
        {
            newPosition = enteredDoor.LinkedDoor.LocalExitMarker.GlobalPosition;
            this.readyToTeleport = true;
        }

        //TODO debug only, remove or add a check for debug and release builds.
        EventHandler.TriggerEvent( EventHandler.EventType.PLAYER_INFO_UPDATED );
    }
    private void InteractionAreaExited( Area2D area )
    {
        this.readyToTeleport = false;

        //TODO debug only, remove or add a check for debug and release builds.
        area2D = null;
        enteredDoor = null;
        newPosition = Vector2.Zero;
        EventHandler.TriggerEvent( EventHandler.EventType.PLAYER_INFO_UPDATED );
    }
    // # -------- # //


    /// <summary>
    /// Standard fucntion for player teleportation.
    /// </summary>
    /// <param name="newPosition"></param>
    public void TeleportTo( Vector2 newPosition )
    {
        Position = newPosition.Round( );
    }

    //TODO debug only, remove or add a check for debug and release builds.
    private void UpdateDebugErrorText( )
    {
        debug_error_text.Text = string.Empty;
        foreach( string error in ErrorHandler.GetErrorHistory( ) )
        {
            debug_error_text.Text = string.Join( "\n", ErrorHandler.GetErrorHistory( ) );
        }
    }

    //TODO debug only, remove or add a check for debug and release builds.
    private void UpdateDebugInfoText( )
    {
        debug_info =
            $"""
			[PLAYER]
			[Player Position]: {this?.Position ?? Vector2.Zero}
			[Current Velocity]: {this.Velocity}
			[Interacting With]: {area2D?.Name ?? "None"}
			[this.readyToTeleport]: {this.readyToTeleport}
			[Current Animation]: {spriteAnimator.Animation}
			[Flipped]: {spriteAnimator.FlipH}

			[DOOR]
			[Door Name]: {enteredDoor?.Name ?? "None"}
			[Door Out Position]: {this?.newPosition ?? Vector2.Zero}
			[Linked Door]: {enteredDoor?.LinkedDoor?.Name ?? "None"}
			
			[CAMERA]
			[Camera Name]: {mainCameraRef?.Name ?? "None"}
			[Position]: {mainCameraRef?.GlobalPosition ?? Vector2.Zero}
			[Parent]: {mainCameraRef.GetParent( )?.Name ?? "None"}
			[Parent's type]: {mainCameraRef.GetParent( )?.GetType( ) ?? null}
			[Parent's position]: {( mainCameraRef?.GetParent( ) as Node2D )?.GlobalPosition ?? Vector2.Zero}

			[ERRORS]
			[Error Count]: {ErrorHandler.GetErrorHistory( ).Count}
			""";

        debug_info_text.Text = debug_info;
    }
}