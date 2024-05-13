using System.Collections.Generic;

namespace LD.Framework
{
    public interface IEventObserver<TListener> where TListener : IGameEventListenerMarker
    {
        IReadOnlyList<TListener> GetListeners(); 
        void RegisterListener(TListener listener);
        void UnregisterListener(TListener listener);
    }
}