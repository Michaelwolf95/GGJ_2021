using UnityEngine;

namespace MichaelWolfGames
{
    public class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T instance { get; protected set; }
    
        protected virtual void Awake()
        {
            instance = this.GetComponent<T>();
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}