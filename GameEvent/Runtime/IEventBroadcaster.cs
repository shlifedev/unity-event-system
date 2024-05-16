using Cysharp.Threading.Tasks;

namespace LD.Framework
{
    public interface IEventBroadcaster<TMessage> where TMessage : IEventMessage
    {
        /// <summary>
        /// 리스너에 이벤트를 전송합니다.
        /// </summary> 
        UniTask BroadcastAll<TEventArgs>(TEventArgs args) where TEventArgs : TMessage;
    }
}