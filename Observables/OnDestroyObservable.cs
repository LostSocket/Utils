using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class OnDestroyObservable : MonoBehaviour
    {
        
        private readonly List<Action> _onDestroyActions = new();
        
        public void AddOnDestroyAction(Action action)
        {
            _onDestroyActions.Add(action);
        }
        
        public void RemoveOnDestroyAction(Action action)
        {
            _onDestroyActions.Remove(action);
        }
        
        private void OnDestroy()
        {
            foreach (var action in _onDestroyActions)
            {
                action.Invoke();
            }
        }
    }
}