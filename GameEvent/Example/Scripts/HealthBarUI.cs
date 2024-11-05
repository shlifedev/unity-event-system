using System;
using LD.Framework;
using TMPro;
using unity_event_system.GameEvent.Example.Scripts.Messages;
using UnityEngine;
using UnityEngine.UI;

namespace unity_event_system.GameEvent.Example.Scripts
{
    public class HealthBarUI : MonoBehaviour,
        IEventListener<OnEntityDamagedMessage>,
        IEventListener<OnEntityDestroyed>

    {
        public GameEntity Target; 
        public TextMeshProUGUI Text;
        public Image FillImage;

        private void OnEnable()
        {
            // Register this class to listen to the event
            EventBus.Register(this);
            FillImage.fillAmount = (float)Target.Health / Target.MaxHealth;
            Text.text = Target.Health.ToString("0.0");
             
        }

        private void OnDestroy()
        {
            EventBus.Unregister(this);
        }

        private void OnDisable()
        {
            EventBus.Unregister(this);
        }

        private void LateUpdate()
        {
            this.transform.position = Target.transform.position;
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.1f, this.transform.position.z);
        }

        public void OnEventRaised(OnEntityDamagedMessage eventMessageArgs)
        {
            Debug.Log($"[HealthBarUI] {eventMessageArgs.Target.name} took damage => {eventMessageArgs.PreviousHealth - eventMessageArgs.CurrentHealth}");
            if (eventMessageArgs.Target == this.Target)
            { 
                FillImage.fillAmount = (float)Target.Health / Target.MaxHealth;
                Text.text = eventMessageArgs.CurrentHealth.ToString("0.0");
            }
        }


    
        public void OnEventRaised(OnEntityDestroyed eventMessageArgs)
        {
            if (eventMessageArgs.Target == this.Target)
            {
                Debug.Log($"[HealthBarUI] {eventMessageArgs.Target.name} has been destroyed");
                Destroy(this.gameObject);
            }
        }
    }
}