using UnityEngine;

namespace Cos.Common
{
    public class CachedMono: MonoBehaviour
    {
        private GameObject cachedGameObject = null;

        private Transform cachedTransform = null;

        public GameObject CachedGameObject
        {
            get
            {
                if(cachedGameObject == null)
                    cachedGameObject = gameObject;
                return cachedGameObject;
            }
        }

        public Transform CachedTransform
        {
            get
            {
                if(cachedTransform == null)
                    cachedTransform = transform;
                return cachedTransform;
            }
        }

        protected virtual void OnDestroy()
        {
            cachedGameObject = null;
            cachedTransform = null;
        }
    }
}
