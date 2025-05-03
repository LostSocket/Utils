using System;
using UnityEngine;

namespace Assets.Scripts.Utils.Interaction
{
    public abstract class InteractiveObject : MonoBehaviour
    {
        public string Name = "Interactive Object";
        public string InteractionDescription = "Pickup";

        public event Action<InteractiveObject> OnDestroyed;
        
        public abstract void OnInteract();
        public abstract void OnFocused();
        public abstract void OnUnfocused();

        public void SafeDestroy()
        {
            OnDestroyed?.Invoke(this);
            Destroy(gameObject);
        }
    }
    
}