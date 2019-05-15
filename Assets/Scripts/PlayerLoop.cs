using UnityEngine;
using System;
using System.Collections;

namespace Cytus2
{
    public class PlayerLoop : MonoBehaviour
    {
        public static PlayerLoop instance { get; private set; }

        public static PlayerLoop NewInstance()
        {
            if (instance != null)
            {
                GameObject.Destroy(instance);
            }
            instance = new GameObject("PlayerLoop").AddComponent<PlayerLoop>();
            return instance;
        }

        public Action onFixedUpdate = delegate { };

        private void FixedUpdate()
        {
            onFixedUpdate();
        }
    }
}