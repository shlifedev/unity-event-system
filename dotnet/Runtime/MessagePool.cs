using System.Collections.Generic;
using UnityEngine;

namespace LD.Framework
{
    public static class MessagePool<T> where T : PoolMessageBase<T>, new()
    {
        private static List<T> _pooled = new();
        private static List<T> _using = new();
 
        public static T Get()
        {
            if (_pooled.Count > 0)
            {
                var message = _pooled[0];
                _pooled.RemoveAt(0);
                _using.Add(message); 
                message.OutPool();
                return message;
            }
            else
            {
                var message = new T();
                _using.Add(message); 
                message.OutPool();
                return message;
            }
        }
        
        public static void Return(T messageBase)
        {  
            _using.Remove(messageBase);
            _pooled.Add(messageBase);  
            messageBase.InPool();
        }
    }
}