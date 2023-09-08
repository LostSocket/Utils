using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils.Unity
{
    public class LoadSceneByName : MonoBehaviour
    {
        public string sceneToLoad;
        
        public void LoadScene()
        {
            if (string.IsNullOrEmpty(sceneToLoad))
            {
                Debug.LogError("Scene to load is not set");
                return;
            }
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}