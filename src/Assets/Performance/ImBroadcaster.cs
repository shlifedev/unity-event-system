using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LD.Framework; 
using UnityEditor;
using UnityEngine;

namespace Performance
{ 
    public class ImMessage : PoolMessageBase<ImMessage>
    {   
    }
    
    [Serializable]
    public class ImListener : IEventListener<ImMessage>
    {
        public UniTask OnEvent(ImMessage args)
        {   
            return UniTask.CompletedTask;
        }
    }
     
    public class ImBroadcaster : MonoBehaviour
    {
        public List<ImListener> Listeners = new();

        private void Awake()
        {
            for (int i = 0; i < 100; i++)
            {
                var listener = new ImListener();
                Listeners.Add(listener);
                EventFlow.Register(listener);
            }
        }

        private int iq = 0;

        private void OnGUI()
        {
            GUILayout.Label($"{PoolMessageBase<ImMessage>.pooledCount}"); 
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Alpha0))
            { 
                UnityEngine.Profiling.Profiler.BeginSample("먀옹");
                for (int i = 0; i < 10; i++)
                {
                    this.iq++;
                    var message = MessagePool<ImMessage>.GetWithParam(iq); 
                    EventFlow.Broadcast(message);
                }
                
                // var message2 = MessagePool<ImMessage>.Get();
                // for (int i = 0; i < 10; i++)
                // { 
                //     message2.a++;
                //     EventFlow.Broadcast(message2);
                // }

                UnityEngine.Profiling.Profiler.EndSample(); 
                EditorApplication.isPaused = true;
            }
        }
    }
}