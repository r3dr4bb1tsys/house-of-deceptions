using Godot;
using System;

public partial class PaperUi: Control
{
    private Button close_button = null;
    private RichTextLabel paper_content = null;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready( )
    {
        close_button = FindChild( "close_button", true ) as Button;
        if( close_button == null )
        {
            ErrorHandler.ThrowError( "[PaperUi.cs/_Ready] - Failed to find the close_button, make sure to add one as a child of the control.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }
        else close_button.Pressed += OnButtonPressed;

        paper_content = FindChild( "paper_content", true ) as RichTextLabel;
        if( paper_content == null )
        {
            ErrorHandler.ThrowError( "[PaperUi.cs/_Ready] - Failed to find the paper_content, make sure to add one as a child of the control.", ErrorHandler.ErrorType.GENERIC_ERROR );
        }
    }

    public void UpdateContent( string Content )
    {
        paper_content.Text = ( Content == null ? string.Empty : Content );
    }

    private void OnButtonPressed( )
    {
        this.Visible = false;
    }
}
