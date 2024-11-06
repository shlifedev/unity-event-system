
using System;
using LD.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace unity_event_system.GameEvent.Example.Scripts.Messages
{
    public class GameEntity : MonoBehaviour
    {
        public float Health = 100;
        public float MaxHealth = 100;
        void Damage()
        {
            int Damage = Random.Range(5, 20); 
            float previousHealth =Health;
            Health -= Damage; 
            var damagedMessage =  new OnEntityDamagedMessage()
            {
                CurrentHealth = Health,
                PreviousHealth = previousHealth,
                Target = this
            }; 
            
            EventFlow.Broadcast(damagedMessage);

            RandomKnockBack(); 
            if (Health <= 0)
            {
                Die();
            } 
        }


        void Die()
        { 
                var destroyedMessage = new OnEntityDestroyed()
                {
                    Target = this
                };
                EventFlow.Broadcast(destroyedMessage);
                GameObject.DestroyImmediate(this.gameObject); 
        }
        void RandomKnockBack()
        {
            GetComponent<Rigidbody2D>()
                .AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(1f,2f)), ForceMode2D.Impulse);

        }
        private void OnMouseUp()
        { 
            Damage();
        }
    }
}