using System.Collections;
using System.Collections.Generic;
using LD.Framework;
using unity_event_system.GameEvent.Example.Scripts.Messages;
using UnityEngine;

public struct OnEntityDamagedMessage : IEventMessage
{
     public GameEntity Target;
     public float PreviousHealth;
     public float CurrentHealth; 
}
