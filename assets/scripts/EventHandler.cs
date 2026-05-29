using System;
using System.Collections.Generic;
using Godot;

public partial class EventHandler: Node
{
    public enum EventType
    {
        GENERIC_EVENT = 0,
        ERROR_OCCURRED = 1,
        PLAYER_INFO_UPDATED = 2,
        AREA_ENTERED = 3,
        TRIGGER_AREA_ENTERED = 4,
        MOUSE_ENTERED = 9,
        MOUSE_EXITED = 10,

        TUTORIAL_PLAYER_MOVEMENT = 5,
        TUTORIAL_PLAYER_RUNNING = 6,
        TUTORIAL_PLAYER_INTERACT = 7,
        TUTORIAL_COMPLETED = 8,
    }

    public static Dictionary<EventType, Delegate> events = new();
    /// <summary>
    /// Listen for an event with data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="callback"></param>
    public static void ListenForEvent<T>( EventType eventType, Action<T> callback )
    {
        events[eventType] = ( Action<T> )events.GetValueOrDefault( eventType ) + callback;
    }


    public static void ListenForEvent( EventType eventType, Action callback )
    {
        events[eventType] = ( Action )events.GetValueOrDefault( eventType ) + callback;
    }


    public static void StopListeningToEvent( EventType eventType, Action callback )
    {
        if( events.ContainsKey( eventType ) )
        {
            events[eventType] = ( Action )events[eventType] - callback;
        }
    }

    public static void StopListeningToEvent<T>( EventType eventType, Action<T> callback )
    {
        if( events.ContainsKey( eventType ) )
        {
            events[eventType] = ( Action<T> )events[eventType] - callback;
        }
    }


    /// <summary>
    /// Trigger an event.
    /// </summary>
    /// <param name="eventType"></param>
    public static void TriggerEvent( EventType eventType )
    {
        if( events.TryGetValue( eventType, out var del ) )
        {
            ( del as Action )?.Invoke( );
        }
    }
    /// <summary>
    /// Trigger an event with data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="data"></param>
    public static void TriggerEvent<T>( EventType eventType, T data )
    {
        if( events.TryGetValue( eventType, out var del ) )
        {
            ( ( Action<T> )del )?.Invoke( data );
        }
    }
}