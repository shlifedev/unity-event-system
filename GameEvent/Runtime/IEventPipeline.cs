namespace LD.Framework
{
    public interface IEventPipeline<TListener, TMessage> : IEventObserver<TListener>, IEventBroadcaster<TMessage>
        where TListener : IGameEventListenerMarker
        where TMessage : IEventMessage
    {
        
    }
}