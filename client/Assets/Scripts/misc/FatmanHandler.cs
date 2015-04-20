

using UnityEngine;

public class FatmanHandler : MonoBehaviour
{

    public GameObject explosion;
    
    private bool d = false;
    private Vector3 launchPt;
    
    public void init(Vector3 launchPt)
    {
        this.launchPt = launchPt;
    }
    
    void OnCollisionEnter(Collision c)
    {
        this.launchPt.z = 0;
        Instantiate(explosion, this.launchPt, Quaternion.Euler(0f, 0f, 0f));
        gameObject.SetActive(false);
        d = true;
    }
    
    void Update()
    {
        if (d == true)
        {
            DestroyImmediate(this);
        }
    }
}
