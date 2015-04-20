using UnityEngine;
using System.Collections;

public class CellRotator : MonoBehaviour {

    public float rotationSpeed = 180f;
    float currentAngleAxis = 0;
    float endAngleAxis = 180;
    bool rotating = false;
    
    public Animator nanoMachine;
    public SkinnedMeshRenderer mesh;
    
    Color startTint = Color.white;
    Color tartetTint = Color.red;
    Color currentTint = Color.black;
    
    float switchTime = float.MaxValue;
    float nextTime = float.MaxValue;
    bool animChanged = false;
    
    void Start()
    {
        nextTime = Time.timeSinceLevelLoad + Random.Range(4f, 20f);
    }
    
    public void setColor(Color tint)
    {
        mesh.material.SetColor("_TintColor", new Color(tint.r, tint.g, tint.b, 0.5f));
    }
    
    public void runRotate(Color tint)
    {
        if (rotating) return;
        mesh.material.SetColor("_TintColor", new Color(tint.r, tint.g, tint.b, 0.5f));
//        startTint = GetComponent<SpriteRenderer>().color;
//        currentTint = startTint;
//        tartetTint = tint;
//        rotating = true;
//        currentAngleAxis = 0;
    }
    void Update () {

        if (Time.timeSinceLevelLoad > nextTime)
        {
            nanoMachine.SetInteger("IdleIndex", Random.Range(0, 4));
            nextTime = Time.timeSinceLevelLoad + Random.Range(4f, 20f);
            switchTime = Time.timeSinceLevelLoad;
            animChanged = true;
        }
        else
        {
            if (animChanged && Time.timeSinceLevelLoad - switchTime > 1f)
            {
                nanoMachine.SetInteger("IdleIndex", -1);
                animChanged = false;
                //Debug.Log(nanoMachine.GetCurrentAnimatorStateInfo(0).normalizedTime);
//                nanoMachine.GetCurrentAnimatorStateInfo(0).
//                if (nanoMachine.GetInteger("IdleIndex"))
//                {
//                    nanoMachine.SetInteger("IdleIndex", -1);
//                    animChanged = false;
//                }
            }
        }

        if (!rotating) return;
        
	    var moveAmount = Time.smoothDeltaTime * rotationSpeed;
        var nextAxis = currentAngleAxis + moveAmount;
        
        transform.rotation = Quaternion.AngleAxis(nextAxis, new Vector3(1f, 1f));
        currentAngleAxis = nextAxis;
        if (currentAngleAxis >= endAngleAxis/2 && currentTint == startTint)
        {
            GetComponent<SpriteRenderer>().color = tartetTint;
            currentTint = tartetTint;
        }
        if (currentAngleAxis >= endAngleAxis)
        {
            currentAngleAxis = 0;
            rotating = false;
            startTint = currentTint;
        }
	}
}
