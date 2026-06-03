using Godot;
using Godot.Collections;

// TODO ABSOULTELY ADD A FEATURE TO USE MULTIPLE GAME ACTION MANAGERS THIS COULD BE SO COOL!!

[Tool]
public partial class GameActionManager: Node2D
{
    public enum GameAction
    {
        quee_free = 0,
        open_dialog = 1,
        trigger_event = 2,
        switch_animated_sprite_frame = 3
    }


    private GameAction _action;
    private EventHandler.EventType _event_to_trigger = EventHandler.EventType.GENERIC_EVENT;
    private bool _with_data = false;
    private int _data;

    private AnimatedSprite2D _animated_sprite = null;
    private string _selected_frame_animation_1 = string.Empty;
    private string _selected_frame_animation_2 = string.Empty;
    private bool _toggle_between_frames = false;


    [Export]
    public GameAction Action
    {
        get => _action;
        set => _action = value;
    }

    [ExportSubgroup( "Trigger Event" )]
    [Export]
    EventHandler.EventType EventToTrigger
    {
        get => _event_to_trigger;
        set => _event_to_trigger = value;
    }
    [Export]
    private bool WithData
    {
        get => _with_data;
        set => _with_data = value;
    }
    [Export]
    public int data
    {
        get => _data;
        set => _data = value;
    }

    [ExportSubgroup( "Switch AnimatedSprite frame" )]
    [Export]
    public AnimatedSprite2D AnimatedSprite
    {
        get => _animated_sprite;
        set
        {
            _animated_sprite = value;
            NotifyPropertyListChanged( );
        }
    }
    [Export]
    public bool ToggleBetweenFrames
    {
        get => _toggle_between_frames;
        set => _toggle_between_frames = value;

    }

    public string FrameAnimation_1
    {
        get => _selected_frame_animation_1;
        set => _selected_frame_animation_1 = value;
    }
    public string FrameAnimation_2
    {
        get => _selected_frame_animation_2;
        set => _selected_frame_animation_2 = value;
    }

    public override void _Ready( ) { }

    // Override to define dynamic properties
    public override Array<Dictionary> _GetPropertyList( )
    {
        Array<Dictionary> properties = new();

        // Add standard properties if needed, or base behavior
        // properties.AddRange(base._GetPropertyList());

        if( _animated_sprite != null && _animated_sprite.SpriteFrames != null )
        {
            string[] frames = _animated_sprite.SpriteFrames.GetAnimationNames();
            if( frames.Length > 0 )
            {
                string hint_string = string.Join(",", frames);
                properties.Add( new Dictionary
                {
                    { "name", nameof(FrameAnimation_1) },
                    { "type", (int)Variant.Type.String },
                    { "hint", (int)PropertyHint.Enum },
                    { "hint_string", hint_string },
                    { "usage", (int)PropertyUsageFlags.Default }
                } );

                properties.Add( new Dictionary
                {
                    { "name", nameof(FrameAnimation_2) },
                    { "type", (int)Variant.Type.String },
                    { "hint", (int)PropertyHint.Enum },
                    { "hint_string", hint_string },
                    { "usage", (int)PropertyUsageFlags.Default }
                } );
            }
        }

        return properties;
    }


    public void TriggerAction( )
    {
        switch( _action )
        {
            case GameAction.quee_free:
            {
                break;
            }
            case GameAction.open_dialog:
            {
                break;
            }
            case GameAction.trigger_event:
            {
                if( _with_data != false )
                {
                    EventHandler.TriggerEvent( _event_to_trigger, _data );
                }
                else EventHandler.TriggerEvent( _event_to_trigger );
                break;
            }
            case GameAction.switch_animated_sprite_frame:
            {
                if( _animated_sprite != null )
                {
                    if( _toggle_between_frames == true ) _animated_sprite.Play( ( _animated_sprite.Animation == FrameAnimation_1 ? FrameAnimation_2 : FrameAnimation_1 ) );
                    else _animated_sprite.Play( FrameAnimation_1 );
                }
                break;
            }

        }
    }
}
