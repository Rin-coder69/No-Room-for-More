using UnityEngine;

namespace CGL.DesignPatterns
{
    // Singleton - Design pattern that ensures only one instance of a class exists at a time
    // accessible from anywhere in code without needing a direct reference to the object.
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        // instance of the class, static = one per class and can use without instance of class
        private static T instance;

        // static property, can be called without instance: GameManager.Instance.<Method>();
        public static T Instance
        {
            get
            {
                // check if instance has been set already
                if (instance == null)
                {
                    // if no instance, find instance in scene
                    instance = GameObject.FindFirstObjectByType<T>();
                    if (instance == null)
                    {
                        // if no instance in the scene, create game object and attach singleton
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        public virtual void Awake()
        {
            // if singleton exists in scene, set instance to this
            if (instance == null)
            {
                instance = this as T;
                // singleton exists across loads, don't destroy singleton on load
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                // singleton already exists, destroy this one (there can only be one)
                Destroy(gameObject);
            }
        }
    }
}