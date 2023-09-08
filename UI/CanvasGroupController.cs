using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupController : MonoBehaviour
    {
        private List<string> _openers = new List<string>();
        public bool Visible => CanvasGroup.alpha > 0f;
        
        public CanvasGroup CanvasGroup {
            get
            {
                if (_canvasGroup == null)
                {
                    _canvasGroup = GetComponent<CanvasGroup>();
                }
                return _canvasGroup;
            }
        }
    
        private CanvasGroup _canvasGroup;

        public virtual void Show()
        {
            OnShow();
        }

        public virtual void Hide()
        {
            OnHide();
        }

        public void SetVisible(bool visible)
        {
            if(visible)
            {
                OnShow();
            }
            else
            {
                OnHide();
            }
        }

        private void OnShow()
        {
            CanvasGroup.alpha = 1f;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
        }

        private void OnHide()
        {
            CanvasGroup.alpha = 0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }
        
    }
}