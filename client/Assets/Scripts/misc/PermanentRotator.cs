

using UnityEngine;

namespace misc
{
    public class PermanentRotator : MonoBehaviour
    {
        public float speed = 30;
        public float direction = 1;
        private Transform t;
        
        
        void Update()
        {
            if (t == null)
                t = transform;
               
            //t.transform.rotation = Quaternion.FromToRotation(t.transform.rotation)
            transform.Rotate(new Vector3(0f, 0f, Time.deltaTime * speed * direction));
        }
    }
}
