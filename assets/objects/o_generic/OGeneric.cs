using System;
using System.Collections.Generic;
using Godot;


[Tool]
public partial class OGeneric : Node2D
{
	// # Public Variables # //
	public enum ObjectType
	{
		None = 0,
		Pickable = 1,
		Trigger = 2
	}
	// # ---------------- # // 

	// # Private Variables # //
	private ObjectType _objectType = ObjectType.None;
	private Sprite2D _objectSrite = null;
	private AnimatedSprite2D _objectAnimatedSprite = null;
	private Area2D objArea = null;
	Camera2D mainCamera = null;
	// # ------------------ # //


	// # Export Variables # //
	[ExportGroup("Object Settings")]
	[Export(PropertyHint.Enum)] public ObjectType objectType		// type of the object, this is usefull for adding diffrent types of object inside the game scene without having to create new ones everytime.
	{
		get => _objectType;
		set => _objectType = value;
	}
	[Export] Sprite2D objectSprite		// if we want a little static sprite to display along with the object
	{
		get => _objectSrite;
		set => _objectSrite = value;
	}
	[Export] AnimatedSprite2D objectAnimatedSprite		// if we want a little animation for our sprite instead of a static picture.
	{
		get => _objectAnimatedSprite;
		set => _objectAnimatedSprite = value;
	}
	// # ---------------- # //


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// check if the objArea is inside the object, if not return an error message.
		objArea = FindChild("objArea", true) as Area2D;
		if (objArea == null)
		{
			ErrorHandler.ThrowError($"[OGeneric.cs/_Ready] - Failed to find the Area2D. Make sure to add one as a child of the object.", ErrorHandler.ErrorType.GENERIC_ERROR);
			return;
		}
		objArea.Connect("body_entered", new Callable(this, nameof(ObjBodyEntered)));

		// check if the mainCamera is inside the scene, if not return an error message.
		mainCamera = GetTree().Root.FindChild("mainSceneCamera", true) as Camera2D;
		if (mainCamera == null)
		{
			ErrorHandler.ThrowError($"[OGeneric.cs/_Ready] - Failed to find the main camera. Make sure to add one in the main scene and name it 'mainSceneCamera'.", ErrorHandler.ErrorType.GENERIC_ERROR);
			return;
		}

	}

	private void ObjBodyEntered(Node2D body)
	{
		Dictionary<string, string> body_properties = new()
        {
            { "body_name", body.Name }
        };

		EventHandler.TriggerEvent(EventHandler.EventType.AREA_ENTERED, body_properties);
		/*
		if (_enteredPlayer == null)
		{
			ErrorHandler.ThrowError($"[OGeneric.cs/ObjBodyEntered] - Failed to cast the body that entered the object area to OPlayer.", ErrorHandler.ErrorType.GENERIC_ERROR);
			return;
		}
		if (_enteredPlayer.IsInGroup("Player"))
		{
			mainCamera.Position = (this.GetParent() as Node2D).Position;
		}
		*/
	}

}
