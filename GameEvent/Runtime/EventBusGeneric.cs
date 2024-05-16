using System.Collections.Generic;
using System.Diagnostics;
using System.Linq; 
using UnityEngine;
using Component = System.ComponentModel.Component;

namespace LD.Framework
{
    /// <summary>
    /// Classes to manage events for each message 
    /// </summary> 
    public static class EventBusGeneric<TMessage> where TMessage : IEventMessage
    {
        private static EventPipeline<TMessage> Pipeline = new EventPipeline<TMessage>();  
        public static void Broadcast(TMessage message) => Pipeline.BroadcastAll(message); 
        static void Register(IGameEventListenerMarker listener) => Pipeline.RegisterListener(listener); 
        static void Unregister(IGameEventListenerMarker listener) => Pipeline.UnregisterListener(listener); 
        public static void Clear() => Pipeline.ClearListener();
        
    }
}
