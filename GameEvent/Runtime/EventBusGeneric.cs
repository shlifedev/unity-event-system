using System.Collections.Generic;
using System.Diagnostics;
using System.Linq; 
using UnityEngine;
using Component = System.ComponentModel.Component;

namespace LD.Framework
{
    /// <summary>
    /// 각 메세지 별로 이벤트를 관리하기 위한 클래스
    /// </summary> 
    public static class EventBusGeneric<TMessage> where TMessage : struct, IEventMessage
    {
        private static EventPipeline<TMessage> Pipeline = new EventPipeline<TMessage>(); 
        /// <summary>
        /// EventBusUtil에 있는 Broadcast를 사용해주세요.
        /// </summary>
        /// <param name="message"></param>
        public static void Broadcast(TMessage message) => Pipeline.BroadcastAll(message); 
        static void Register(IGameEventListenerMarker listener) => Pipeline.RegisterListener(listener); 
        static void Unregister(IGameEventListenerMarker listener) => Pipeline.UnregisterListener(listener); 
        public static void Clear() => Pipeline.ClearListener();
        
    }
}
