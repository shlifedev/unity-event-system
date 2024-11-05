using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection; 
using UnityEngine;

namespace LD.Framework
{
    public static class EventBus
    {
        private static Dictionary<System.Type, Type[]> _interFacesMap = new Dictionary<Type, Type[]>();
        private static Dictionary<System.Type, List<Action<IGameEventListenerMarker>>> _registerMethodMap = new();
        private static Dictionary<System.Type, List<Action<IGameEventListenerMarker>>> _unregisterMethodMap = new();
        private static HashSet<System.Type> _ignoredTypes = new();
        static Type[] GetInterfaces(object target)
        {
            if(!_interFacesMap.ContainsKey(target.GetType()))
                _interFacesMap.Add(target.GetType(), target.GetType().GetInterfaces());
            return _interFacesMap[target.GetType()];
        }
        static bool IsInitialized(object target)
        {
            if (_registerMethodMap.ContainsKey(target.GetType()) &&
                _unregisterMethodMap.ContainsKey(target.GetType()))
                return true;
            else return false;
        }

        static void Initialize(object target)
        { 
            if(!_interFacesMap.ContainsKey(target.GetType()))
                _interFacesMap.Add(target.GetType(), target.GetType().GetInterfaces());
            var interfaces = GetInterfaces(target);
            for (var index = 0; index < interfaces.Length; index++)
            {
                var listenerType = interfaces[index];
                if (listenerType.IsSubclassOfRawGeneric(typeof(IEventListener<>)))
                {
                    var genericArgs = listenerType.GenericTypeArguments;
                    if (genericArgs.Length != 0)
                    {
                        for (int i = 0; i < genericArgs.Length; i++)
                        {
                            var genericArg = genericArgs[i];
                            var baseType = genericArg;
                            var eventBusType = typeof(EventBusGeneric<>);
                            var genericEventBusType = eventBusType.MakeGenericType(baseType);

                            if (genericEventBusType == null)
                                throw new Exception($"{target.GetType().Name} {genericArg.Name} EventBus를 찾을 수 없습니다.");
 
                            var registerMethodInfo = genericEventBusType.GetMethod("Register",
                                BindingFlags.Static | BindingFlags.Public);
                            var unRegisterMethodInfo = genericEventBusType.GetMethod("Unregister",
                                BindingFlags.Static | BindingFlags.Public);

                            if (registerMethodInfo != null)
                            {
                                var myDelegate = (Action<IGameEventListenerMarker>)Delegate.CreateDelegate(typeof(Action<IGameEventListenerMarker>), registerMethodInfo);



                                if (!_registerMethodMap.ContainsKey(target.GetType()))
                                    _registerMethodMap.Add(target.GetType(), new());
                                _registerMethodMap[target.GetType()].Add(myDelegate);
                            }
                            else
                            {
                                throw new Exception(
                                    $"Register 메소드를 찾을 수 없습니다.");
                            }

                            if (unRegisterMethodInfo != null)
                            {
                                var myDelegate = (Action<IGameEventListenerMarker>)Delegate.CreateDelegate(typeof(Action<IGameEventListenerMarker>), unRegisterMethodInfo);



                                if (!_unregisterMethodMap.ContainsKey(target.GetType()))
                                    _unregisterMethodMap.Add(target.GetType(), new());
                                _unregisterMethodMap[target.GetType()].Add(myDelegate);
                            }
                            else
                            {
                                throw new Exception(
                                    $"Unregister 메소드를 찾을 수 없습니다.");
                            }
                        }
                    }
                }
            }
        }
        static void RawCall(bool register, object target)
        {
            if (!_ignoredTypes.Contains(target.GetType()))
            {
                if (!IsInitialized(target))
                {
                    Initialize(target);
                    // 초기화 이후에도 초기화가 되지않은 객체인경우 (non event listener marker)
                    if (!IsInitialized(target) && !_ignoredTypes.Contains(target.GetType())) 
                       _ignoredTypes.Add(target.GetType());
                }
            }

            // 무시할 객체 타입
            if (_ignoredTypes.Contains(target.GetType())) return;
            if (register)
                {
                    for (var index = 0; index < _registerMethodMap[target.GetType()].Count; index++)
                    {
                        var item = _registerMethodMap[target.GetType()][index];
                        item?.Invoke(target as IGameEventListenerMarker);
                    }
                }
                else
                {
                    for (var index = 0; index < _unregisterMethodMap[target.GetType()].Count; index++)
                    {
                        var item = _unregisterMethodMap[target.GetType()][index];
                        item?.Invoke(target as IGameEventListenerMarker);
                    }
                } 
        }
        
        /// <summary>
        /// 호출시 자동으로 생명주기로 관리할 인터페이스 메세지를 등록합니다.
        /// </summary> 
        public static void Register(IGameEventListenerMarker target)
        { 
            RawCall(true, target);
        }
        
        /// <summary>
        /// 호출시 자동으로 생명주기로 관리할 인터페이스 메세지를 등록합니다.
        /// </summary> 
        public static void Broadcast<T>(T message) where T : IEventMessage
        { 
            EventBusGeneric<T>.Broadcast(message); 
        }
        /// <summary>
        /// 호출시 자동으로 인터페이스 메세지를 찾아 이벤트버스에서 더이상 관리하지 않게됩니다.
        /// </summary> 
        public static void Unregister(IGameEventListenerMarker target)
        {
            RawCall(false, target);
        }

 
        
        /// <summary>
        /// 호출시 자동으로 생명주기로 관리할 인터페이스 메세지를 등록합니다.
        /// </summary> 
        public static void Register(object target)
        { 
            RawCall(true, target);
        }
        /// <summary>
        /// 호출시 자동으로 인터페이스 메세지를 찾아 이벤트버스에서 더이상 관리하지 않게됩니다.
        /// </summary> 
        public static void Unregister(object target)
        {
            RawCall(false, target);
        }
    }
}
