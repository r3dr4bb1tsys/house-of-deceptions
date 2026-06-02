using Godot;
using System;

public partial class SharedVariables: Node
{
    public static bool isGamePaused = false;
    public static bool canPlayerMove = true;
    public static bool IS_DEBUG_MODE = false;
    public static bool playerJustTeleported = false;
}
