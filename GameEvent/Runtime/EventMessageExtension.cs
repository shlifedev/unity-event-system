using System;
using System.Collections.Generic;
using UnityEngine;

namespace LD.Framework
{
    [Obsolete]
    public static class EventMessageExtension
    {
        [Obsolete]
        private static readonly Dictionary<Type, bool> TypeCheckMap = new Dictionary<Type, bool>(); 
        [Obsolete]
        public static T To<T>(this IEventMessage message) where T : IEventMessage
        {
            return (T)message;
        }
        [Obsolete]
        public static bool Is<T>(this IEventMessage message) where T : IEventMessage
        { 
            
            if (TypeCheckMap.ContainsKey(typeof(T)) == false)
                TypeCheckMap.Add(typeof(T), message is T);
            if (TypeCheckMap.TryGetValue(typeof(T), out var valid))
            {
                return valid;
            }
            else
            {
                return false;
            }
        }
        
        [Obsolete]
        public static bool TryGet<T>(this IEventMessage message, out T converted) where T : IEventMessage
        {
            var equal = message.Is<T>();
            converted = (equal) ? message.To<T>() : default;
            return equal;
        }
    }
}