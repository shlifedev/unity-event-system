using Cysharp.Threading.Tasks;

namespace LD.Framework
{
    public interface IEventBroadcaster<TMessage> where TMessage : IEventMessage
    {
        /// <summary>
        /// Send Event To Listener
        /// </summary> 
        UniTask BroadcastAll<TEventArgs>(TEventArgs args) where TEventArgs : TMessage;
    }
}