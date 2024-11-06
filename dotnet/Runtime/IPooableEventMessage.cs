using System;

namespace LD.Framework
{
    internal interface IPooableEventMessage : IEventMessage
    {
        bool IsInstantiated { get; }
        void Return();
    }
}