using Godot;
using System.ComponentModel.DataAnnotations;
public partial class GameActionManager : Node2D
{
	public enum GameAction
	{
		quee_free = 0,
		open_dialog = 1,
		trigger_event = 2
	}


	private GameAction _action;
	private EventHandler.EventType _event_to_trigger = EventHandler.EventType.GENERIC_EVENT;
	private bool _with_data = false;
	private int _data;

	[Export]
	public GameAction Action
	{
		get => _action;
		set => _action = value;
	}

	[ExportSubgroup("Trigger Event")]
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

	
	public override void _Ready()
	{

	}

	public void TriggerAction()
	{
		switch (_action)
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
				if (_with_data != false)
				{
					EventHandler.TriggerEvent( _event_to_trigger, _data );
				}
				else EventHandler.TriggerEvent( _event_to_trigger);
				break;
			}
		}
	}
}
