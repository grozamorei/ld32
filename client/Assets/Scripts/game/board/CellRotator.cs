using UnityEngine;
using System.Collections;

public class CellRotator : MonoBehaviour {

    public Animator nanoMachine;
    public SkinnedMeshRenderer mesh;
    
    Color startTint = Color.white;
    Color tartetTint = Color.red;
    Color currentTint = Color.black;
    
    float switchTime = float.MaxValue;
    float nextTime = float.MaxValue;
    bool animChanged = false;
    float deathTime = float.MaxValue;
    
    void Start()
    {
        nextTime = Time.timeSinceLevelLoad + Random.Range(1f, 20f);
    }
    
    public void on()
    {
        if (nanoMachine.gameObject.activeSelf)
            nanoMachine.SetBool("death", false);
        nanoMachine.gameObject.SetActive(true);
    }
    
    public void off()
    {
//        if (nanoMachine.gameObject.activeSelf)
//            nanoMachine.SetBool("death", true);
//        deathTime = Time.timeSinceLevelLoad;
        instantOff();
    }
    
    public void instantOff()
    {
        nanoMachine.gameObject.SetActive(false);
    }
    
    public void setColor(Color tint)
    {
        mesh.material.SetColor("_TintColor", new Color(tint.r, tint.g, tint.b, 0.5f));
    }
    
    public void runRotate(Color tint)
    {
        mesh.material.SetColor("_TintColor", new Color(tint.r, tint.g, tint.b, 0.5f));
    }
    void Update () {

        if (nanoMachine.gameObject.activeSelf)
        {
            if (nanoMachine.GetBool("death"))
            {
                if (Time.timeSinceLevelLoad - deathTime > 0.8f)
                {
                    instantOff();
                    return;
                }
            }
            if (Time.timeSinceLevelLoad > nextTime)
            {
                var tt = Random.Range(0, 4);
//                Debug.Log("new tt! " + tt);
                nanoMachine.SetInteger("IdleIndex", tt);
                nextTime = Time.timeSinceLevelLoad + Random.Range(4f, 20f);
                switchTime = Time.timeSinceLevelLoad;
                animChanged = true;
            }
            else
            {
                if (animChanged && Time.timeSinceLevelLoad - switchTime > 0.5f)
                {
                    nanoMachine.SetInteger("IdleIndex", -1);
                    animChanged = false;
                }
            }
        }
	}
}
