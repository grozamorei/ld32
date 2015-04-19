using UnityEngine;
using System.Collections;

public class CellRotator : MonoBehaviour {

    public float rotationSpeed = 180f;
    float currentAngleAxis = 0;
    float endAngleAxis = 180;
    bool rotating = false;
    
    Color startTint = Color.white;
    Color tartetTint = Color.red;
    Color currentTint = Color.black;
    
    public void setColor(Color tint)
    {
        GetComponent<SpriteRenderer>().color = tint;
    }
    
    public void runRotate(Color tint)
    {
        if (rotating) return;
        startTint = GetComponent<SpriteRenderer>().color;
        currentTint = startTint;
        tartetTint = tint;
        rotating = true;
        currentAngleAxis = 0;
    }
    void Update () {
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
