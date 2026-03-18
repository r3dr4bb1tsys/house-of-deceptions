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

	// # Private Variabless # //
	private ObjectType _objectType = ObjectType.None;
	private Sprite2D _objectSrite = null;
	private AnimatedSprite2D _objectAnimatedSprite = null;
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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
