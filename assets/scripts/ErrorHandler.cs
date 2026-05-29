using System.Collections.Generic;
using Godot;

/// <summary>
/// Simple error handler class.
/// </summary>
public partial class ErrorHandler : Node
{
	public enum ErrorType
	{
		GENERIC_ERROR = 0
	}
	/// <summary>
	/// Error list, max error count is 10. New errors will push out the oldest error.
	/// </summary>
	private static List<string> errorHistory = new List<string>();

	public static List<string> GetErrorHistory()
	{
		return errorHistory;
	}

	// Push an error into the error list. 
	public static void ThrowError(string errorMessage, ErrorType errorType = ErrorType.GENERIC_ERROR)
	{
		// TODO prevents the error logging from occuring when debug mode is off.
		if (SharedVariables.IS_DEBUG_MODE == false)
		{
			return;
		}
		/*
		if (errorHistory.Count >= 5)
		{
			// remove the oldest error.
			errorHistory.RemoveAt(0);
		}
		*/
		string formattedError = $"[{errorType}]: {errorMessage}\n";
		errorHistory.Add(formattedError);
		GD.PrintErr(formattedError);

		// trigger an event to notify listeners that a new error has occured.
		EventHandler.TriggerEvent(EventHandler.EventType.ERROR_OCCURRED);
	}
}