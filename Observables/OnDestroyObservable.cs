using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Observables
{
    /// <summary>
    /// This class is used to observe the OnDestroy event of a MonoBehaviour without having to
    /// do the annoying thing for having an event and then usubscribe at destroy time.
    /// </summary>
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