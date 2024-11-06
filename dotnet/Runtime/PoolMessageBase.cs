using System;

namespace LD.Framework
{
 
 

    public abstract class PoolMessageBase<T> :  IPooableEventMessage where T : PoolMessageBase<T>, new()
    { 
        public bool IsInstantiated { get; private set; }

        public static int pooledCount;
   
        /// <summary>
        /// Don't Call this method YourSelf..
        /// </summary>
        public void Return()
        { 
            MessagePool<T>.Return(this as T); 
        }
         
        internal void OutPool()
        {
            pooledCount++;
            IsInstantiated = true;
        }

        internal void InPool()
        {
            pooledCount--;
            IsInstantiated = false;
        }
    }
}