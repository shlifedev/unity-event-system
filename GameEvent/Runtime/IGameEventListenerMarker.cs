using System;
using UnityEngine;

namespace LD.Framework
{ 
    /// <summary>
    /// Marking interface to facilitate listener registration.
    /// </summary>
    public interface IGameEventListenerMarker
    { 
         
    }

    /// <summary>
    /// A listener to listen for events
    /// In this case, convert the args directly and use them.
    /// The implementation of Dispose is mandatory to call when unsubscribing.
    /// </summary>
    public interface IEventListener<TArgs> : IGameEventListenerMarker where TArgs : IEventMessage
    {
        void OnEventRaised(TArgs eventMessageArgs);
    }
}