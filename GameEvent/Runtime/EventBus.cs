using System;
using System.Linq;
using System.Reflection; 
using UnityEngine;

namespace LD.Framework
{
    public static class EventBus
    { 
        static void RawCall(string methodName, object target)
        { 
            var interfaces = target.GetType().GetInterfaces();
            foreach(var listenerType in interfaces)
            { 
                if (listenerType.IsSubclassOfRawGeneric(typeof(IEventListener<>)))
                {
                    var genericArgs = listenerType.GenericTypeArguments; 
                    if (genericArgs.Length != 0)
                    {
                        foreach (var genericArg in genericArgs)
                        { 
                            var baseType = genericArg.DeclaringType;
                            baseType = null;
                            if (baseType == null)
                                baseType = genericArg; 
                            
                            var eventBusType = typeof(EventBusGeneric<>);
                            var genericEventBusType = eventBusType.MakeGenericType(baseType);

                            if (genericEventBusType == null)
                                throw new Exception($"{target.GetType().Name} EventBus를 찾을 수 없습니다.");
                           
                            var registerMethodInfo = genericEventBusType.GetMethod(methodName,
                                BindingFlags.Static | BindingFlags.NonPublic);
                            if (registerMethodInfo != null)
                                registerMethodInfo.Invoke(null, new[] { target });
                            else
                            {
                                throw new Exception(
                                    $"{methodName} 메소드를 찾을 수 없습니다.");
                            }
                            
                        }
                    }
                } 
            } 
        }
        
        /// <summary>
        /// 호출시 자동으로 생명주기로 관리할 인터페이스 메세지를 등록합니다.
        /// </summary> 
        public static void Register(IGameEventListenerMarker target)
        { 
            RawCall(nameof(Register), target);
        }
        
        /// <summary>
        /// 호출시 자동으로 생명주기로 관리할 인터페이스 메세지를 등록합니다.
        /// </summary> 
        public static void Broadcast<T>(T message) where T : struct, IEventMessage
        { 
            EventBusGeneric<T>.Broadcast(message); 
        }
        /// <summary>
        /// 호출시 자동으로 인터페이스 메세지를 찾아 이벤트버스에서 더이상 관리하지 않게됩니다.
        /// </summary> 
        public static void Unregister(IGameEventListenerMarker target)
        {
            RawCall(nameof(Unregister), target);
        }

 
        
        /// <summary>
        /// 호출시 자동으로 생명주기로 관리할 인터페이스 메세지를 등록합니다.
        /// </summary> 
        public static void Register(object target)
        { 
            RawCall(nameof(Register), target);
        }
        /// <summary>
        /// 호출시 자동으로 인터페이스 메세지를 찾아 이벤트버스에서 더이상 관리하지 않게됩니다.
        /// </summary> 
        public static void Unregister(object target)
        {
            RawCall(nameof(Unregister), target);
        }
    }
}