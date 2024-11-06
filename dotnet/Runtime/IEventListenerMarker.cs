using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LD.Framework
{ 
    /// <summary>
    /// Marking interface to facilitate listener registration.
    /// </summary>
    public interface IEventListenerMarker
    { 
         
    }

    /// <summary>
    /// A listener to listen for events
    /// In this case, convert the args directly and use them.
    /// The implementation of Dispose is mandatory to call when unsubscribing.
    /// </summary>
    public interface IEventListener<TArgs> : IEventListenerMarker where TArgs : IEventMessage
    {
        UniTask OnEvent(TArgs args);
    } 
    
    
}