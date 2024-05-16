using LD.Framework;
using UnityEngine;

namespace DefaultNamespace
{
    public class UI : MonoBehaviour
    {
        public void ClickMerge()
        {
            EventBus.Broadcast(new CubeMergeMessage());
        }
    }
}