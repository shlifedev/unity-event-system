using LD.Framework;
using UnityEngine;

namespace DefaultNamespace
{
    public struct CubeMergeMessage : IEventMessage
    {
        
    }
    public struct CubeCreateMessage : IEventMessage
    {
        public CubeCreateMessage(Vector3 scale, bool merged)
        {
            Scale = scale;
            Merged = merged;
        }

        public Vector3 Scale { get; }
        public bool Merged { get; }
    }
}