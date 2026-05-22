using Godot;

public partial class GameMenu : Control
{
	private Button resume_button = null;
	private Button settings_button = null;
	private Button save_and_exit_button = null;
	private Button quit_button = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		resume_button = FindChild("resume_button") as Button;
		settings_button = FindChild("settings_button") as Button;
		save_and_exit_button = FindChild("save_and_exit_button") as Button;
		quit_button = FindChild("quit_button") as Button;
	
	
		// Check if the buttons exist, and print an error message if they dont. 
		if (resume_button == null)
		{
			GD.PrintErr($"[GameMenu] Failed to find the resume button. Make sure to add one as a child of the GameMenu node.");
			return;
		}
		if (settings_button == null)
		{
			GD.PrintErr($"[GameMenu] Failed to find the settings button. Make sure to add one as a child of the GameMenu node.");
			return;
		}
		if (save_and_exit_button == null)
		{
			GD.PrintErr($"[GameMenu] Failed to find the save and exit button. Make sure to add one as a child of the GameMenu node.");
			return;
		}
		if (quit_button == null)
		{
			GD.PrintErr($"[GameMenu] Failed to find the quit button. Make sure to add one as a child of the GameMenu node.");
			return;
		}

		resume_button.Connect("pressed", new Callable(this, nameof(OnResumeButtonPressed)));
		settings_button.Connect("pressed", new Callable(this, nameof(OnSettingsButtonPressed)));
		save_and_exit_button.Connect("pressed", new Callable(this, nameof(OnSaveAndExitButtonPressed)));
		quit_button.Connect("pressed", new Callable(this, nameof(OnQuitButtonPressed)));
	}

    public override void _Input(InputEvent @event)
    {
		// Toggle the game pause menu visibility.
		if (@event.IsActionPressed("ui_cancel"))
		{
			// toggle pause menu
			TogglePause();
		}
    }

	private void TogglePause()
	{
		SharedVariables.isGamePaused = !SharedVariables.isGamePaused;
		this.Visible = SharedVariables.isGamePaused;
		SharedVariables.canPlayerMove = !SharedVariables.isGamePaused;
	}

    private void OnQuitButtonPressed()
	{
		GD.Print("Quit button pressed. Quitting the game...");
		GetParent().GetTree().Quit();
	}

    private void OnSaveAndExitButtonPressed()
	{
		GD.Print("Save and Exit button pressed. Saving the game and exiting...");
		
		// save logic here

		GetParent().GetTree().Quit();
	}

    private void OnSettingsButtonPressed()
	{
		GD.Print("Settings button pressed. Opening the settings menu...");
	}

    private void OnResumeButtonPressed()
	{
		GD.Print("Resume button pressed. Resuming the game...");
		// resume
		TogglePause();
	}

}
