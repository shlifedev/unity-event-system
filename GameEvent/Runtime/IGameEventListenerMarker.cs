using System;
using UnityEngine;

namespace LD.Framework
{ 
    /// <summary>
    /// 리스너 등록을 편하게 하기위한 마킹 인터페이스.
    /// </summary>
    public interface IGameEventListenerMarker
    { 
         
    }

    /// <summary>
    /// 이벤트를 수신 할 리스너
    /// 이 경우 args를 직접 convert해서 사용한다.
    /// Dispose의 구현체는 구독 해지시 필수 사용하여 호출한다.
    /// </summary>
    public interface IEventListener<TArgs> : IGameEventListenerMarker where TArgs : IEventMessage
    {
        void OnEventRaised(TArgs eventMessageArgs);
    }
}