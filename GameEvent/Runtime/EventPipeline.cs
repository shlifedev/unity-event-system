using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using LD.Framework;
using UnityEngine;
using UnityEngine.Profiling;

namespace LD.Framework
{
    /// <summary>
    /// Objects that send and register game events 
    /// </summary>
    public class EventPipeline<TMessage> : IEventPipeline<IGameEventListenerMarker, TMessage> 
        where TMessage : IEventMessage
    {
        #region Fields  
        /// <summary>
        /// Listener Objs
        /// </summary>
        protected virtual List<IGameEventListenerMarker> Listeners { get; } = new List<IGameEventListenerMarker>();


        /// <summary>
        /// Listener Hashs
        /// </summary>
        protected virtual HashSet<IGameEventListenerMarker> RegisteredHashMap { get; } = new HashSet<IGameEventListenerMarker>();
        #endregion
        #region Functions

        public IReadOnlyList<IGameEventListenerMarker> GetListeners()
        {
            return Listeners;
        }

        /// <summary>
        /// Regist Listener
        /// </summary>
        /// <param name="listener"></param>
        public void RegisterListener(IGameEventListenerMarker listener)
        {
            
           
            if (RegisteredHashMap.Contains(listener) == false)
            {
                Listeners.Add(listener);
                RegisteredHashMap.Add(listener);
            } 
        }

        
        /// <summary>
        /// Is Already Registred?
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool IsRegistered(IGameEventListenerMarker listener)
        {
            return RegisteredHashMap.Contains(listener);
        }

        public void ClearListener()
        {
            Listeners.Clear();
            RegisteredHashMap.Clear();
        }
        
        /// <summary>
        /// Unregister
        /// </summary>
        /// <param name="listener"></param>
        public void UnregisterListener(IGameEventListenerMarker listener)
        { 
            if (!RegisteredHashMap.Contains(listener)) return;
            // Listeners.Add(listener);
            RegisteredHashMap.Remove(listener);
            Listeners.Remove(listener); 
        } 
        
        /// <summary>
        /// Broadcast to all listeners
        /// </summary> 
        public virtual UniTask BroadcastAll<TEventArgs>(TEventArgs args) where TEventArgs :  TMessage
        {  
            for (int i=Listeners.Count-1; i>=0; --i)
            { 
                var listener = Listeners[i]; 
                var convert = listener as IEventListener<TEventArgs>;
                if (convert == null)
                {
                     Debug.LogError($"{nameof(IGameEventListenerMarker)}  must be explicitly implemented with a generic argument.");
                }
                else 
                {  
                    convert?.OnEventRaised(args);
                }
            } 

            return UniTask.CompletedTask;
        } 
        #endregion
    }
}