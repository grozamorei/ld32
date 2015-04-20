using UnityEngine;

namespace mics
{
    public class MusicStarter : MonoBehaviour
    {
        private float targetTime = float.MaxValue;
        void Start()
        {
            targetTime = Time.timeSinceLevelLoad + 1.8f;
        }
        
        void Update()
        {
            if (Time.timeSinceLevelLoad > targetTime)
            {
                var clips = GetComponents<AudioSource>();
                foreach (var a in clips)
                {
                    //Debug.Log(a.name + ":" + a.clip.name);
                    if (a.clip.name == "music")
                    {
                        a.loop = true;
                        a.Play();
                    }
                }
                targetTime = float.MaxValue;
            }
        }
    }
}

