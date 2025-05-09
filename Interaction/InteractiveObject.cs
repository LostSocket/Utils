using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils.Interaction
{
    public abstract class InteractiveObject : MonoBehaviour
    {
        public string Name = "Interactive Object";
        public string InteractionDescription = "Pickup";
        
        private Dictionary<MeshRenderer, Material[]> m_UnfocusedMaterials;
        private Dictionary<MeshRenderer, Material[]> m_FocusedMaterials;

        private MeshRenderer[] m_MeshRenderers;
        private bool m_IsInitialized;
        
        public event Action<InteractiveObject> OnDestroyed;

        public void Interact()
        {
            OnInteract();
        }

        public void Focused(Material _focusedMaterial)
        {
            if (!m_IsInitialized)
            {
                InitializeMaterials(_focusedMaterial);
                m_IsInitialized = true;
            }

            foreach (var r in m_MeshRenderers)
            {
                r.materials = m_FocusedMaterials[r];
            }
            OnFocused();
        }

        public void UnFocused()
        {
            OnUnfocused();
            if (m_MeshRenderers == null) return;

            foreach (var r in m_MeshRenderers)
            {
                r.materials = m_UnfocusedMaterials[r];
            }
        }
        
        private void InitializeMaterials(Material _focusedMaterial)
        {
            m_MeshRenderers = GetComponentsInChildren<MeshRenderer>(true);
            m_UnfocusedMaterials = new Dictionary<MeshRenderer, Material[]>();
            m_FocusedMaterials = new Dictionary<MeshRenderer, Material[]>();

            foreach (var r in m_MeshRenderers)
            {
                var unfocused_materials = r.materials;
                m_UnfocusedMaterials.Add(r, unfocused_materials);

                var focused_materials = new Material[unfocused_materials.Length + 1];
                Array.Copy(unfocused_materials, focused_materials, unfocused_materials.Length);

                var focused_material_instance = Instantiate(_focusedMaterial);
                focused_materials[unfocused_materials.Length] = focused_material_instance;

                m_FocusedMaterials.Add(r, focused_materials);
            }
        }

        protected abstract void OnInteract();
        protected abstract void OnFocused();
        protected abstract void OnUnfocused();

        protected void SafeDestroy()
        {
            OnDestroyed?.Invoke(this);
            Destroy(gameObject);
        }
    }
}