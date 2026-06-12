using Godot;
using System.Collections.Generic;


public partial class TutorialScene: Node2D
{
    private AnimationPlayer tutorialAnimationManager = null;

    private List<int> stagesCompleted = new List<int>();


    // Called when the node enters the scene tree for the first time.
    public override void _Ready( )
    {
        base._Ready( );

        // listen for events.
        EventHandler.ListenForEvent<int>( EventHandler.EventType.TUTORIAL_PLAYER_MOVEMENT, PlayNextAnimation );
        EventHandler.ListenForEvent<int>( EventHandler.EventType.TUTORIAL_PLAYER_PICKED_FLASHLIGHT, PlayNextAnimation );

        tutorialAnimationManager = FindChild( "tutorialAnimationManager", true ) as AnimationPlayer;
        if( tutorialAnimationManager == null )
        {
            ErrorHandler.ThrowError( "[TutorialScene.cs/_Ready] - Failed to find the animation player node. Make sure to add one as the children of the tutorial scene.", ErrorHandler.ErrorType.GENERIC_ERROR );
            return;
        }

        tutorialAnimationManager.AnimationFinished += ( animationName ) =>
        {
            if (animationName == "begin_introduction" )
            {
                EventHandler.TriggerEvent( EventHandler.EventType.TUTORIAL_PLAYER_MOVEMENT, 0 );
            }
        };
    }

    // finds out how far into the tutorial the player is and plays the correct animation
    public void PlayNextAnimation( int tutorialStage )
    {
        switch( tutorialStage )
        {
            case 0:     // wasd movement
            {
                tutorialAnimationManager.Play( "stage_0" );
                stagesCompleted.Add( tutorialStage );
                break;
            }
            case 1:     // running //TODO: might change this i dont want running in my game
            {
                //TODO: stagesCompleted.Contains(0) serves to check if the previous stage has been completed before starting a new one, the value 0 should not be hard coded but im too tired right now to find a better solution. THIS MUST BE CHANGED same with the next checks.
                if( !stagesCompleted.Contains( 0 ) ) break;

                if( stagesCompleted.Contains( tutorialStage ) ) // check if the stage has already been completed
                {
                    break;
                }
                tutorialAnimationManager.Play( "stage_1" );
                stagesCompleted.Add( tutorialStage );
                break;
            }
            case 2:     // interacting
            {
                if( !stagesCompleted.Contains( 1 ) ) break;
                if( stagesCompleted.Contains( tutorialStage ) ) // check if the stage has already been completed
                {
                    break;
                }
                tutorialAnimationManager.Play( "stage_2" );
                stagesCompleted.Add( tutorialStage );
                break;
            }
            case 3:     // completed
            {
                if( !stagesCompleted.Contains( 2 ) ) break;
                if( stagesCompleted.Contains( tutorialStage ) ) // check if the stage has already been completed
                {
                    break;
                }
                //tutorialAnimationManager.Play( "stage_completed" );
                stagesCompleted.Add( tutorialStage );
                EventHandler.StopListeningToEvent<int>( EventHandler.EventType.TUTORIAL_PLAYER_MOVEMENT, PlayNextAnimation );
                EventHandler.StopListeningToEvent<int>( EventHandler.EventType.TUTORIAL_PLAYER_PICKED_FLASHLIGHT, PlayNextAnimation );

                PackedScene scene_1 = GD.Load<PackedScene>("res://assets/scenes/scene_1.tscn");
                if (scene_1 != null)
                {
                    GetTree( ).ChangeSceneToPacked( scene_1 );
                }
                break;
            }
        }
    }

}


// position (576, 0)