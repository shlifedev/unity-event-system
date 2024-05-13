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
    /// 게임 이벤트를 전송, 등록하는 객체 
    /// </summary>
    public class EventPipeline<TMessage> : IEventPipeline<IGameEventListenerMarker, TMessage> 
        where TMessage : IEventMessage
    {
        #region Fields  
        /// <summary>
        /// 리스너 객체
        /// </summary>
        protected virtual List<IGameEventListenerMarker> Listeners { get; } = new List<IGameEventListenerMarker>();


        /// <summary>
        /// 리스너 객체와 함께 사용되는 해시맵 
        /// </summary>
        protected virtual HashSet<IGameEventListenerMarker> RegisteredHashMap { get; } = new HashSet<IGameEventListenerMarker>();
        #endregion
        #region Functions

        public IReadOnlyList<IGameEventListenerMarker> GetListeners()
        {
            return Listeners;
        }

        /// <summary>
        /// 리스너를 등록한다. 
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
        /// 이미 등록 된 객체인가?
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
        /// 리스너를 해제한다.
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
        /// 리스너객체에 이벤트 전송
        /// </summary> 
        public virtual UniTask BroadcastAll<TEventArgs>(TEventArgs args) where TEventArgs : struct, TMessage
        { 
            // 루핑 도중 요소가 삭제되는 경우가 존재
            for (int i=Listeners.Count-1; i>=0; --i)
            { 
                var listener = Listeners[i]; 
                var convert = listener as IEventListener<TEventArgs>;
                if (convert == null)
                {
                    // LBDebug.LogError($"{nameof(IGameEventListenerMarker)} 는 반드시 명시적으로 제네릭 인자와 함께 구현해야 합니다.");
                }
                else 
                { 
//                    LBDebug.Log("raised from");
                    convert?.OnEventRaised(args);
                }
            } 

            return UniTask.CompletedTask;
        } 
        #endregion
    }
}