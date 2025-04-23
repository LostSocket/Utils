using UnityEngine;

namespace Assets.Scripts.Utils.Interaction
{
    public abstract class InteractiveObject : MonoBehaviour
    {
        public KeyCode InteractKey = KeyCode.E;
        public string Name = "Interactive Object";
        public string Description = "Interactive Object Description";
        
        public abstract void OnInteract();
        public abstract void OnFocused();
        public abstract void OnUnfocused();
    }
    
}