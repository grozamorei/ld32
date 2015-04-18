using UnityEngine;
using System.Collections;

public class CellRotator : MonoBehaviour {

    public float rotationSpeed = 180f;
    float currentAngleAxis = 0;
    float endAngleAxis = 180;
    bool rotating = false;
    
    Color currentTint = Color.white;
    Color startTint = Color.white;
    
    Color defaultTint = Color.white;
    Color coloredTint = Color.red;
    
    public void runRotate()
    {
        if (rotating) return;
        GetComponent<SpriteRenderer>().color = startTint;
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
            if (startTint == defaultTint)
            {
                currentTint = coloredTint;
            }
            else
            {
                currentTint = defaultTint;
            }
            GetComponent<SpriteRenderer>().color = currentTint;
        }
        if (currentAngleAxis >= endAngleAxis)
        {
            currentAngleAxis = 0;
            rotating = false;
            startTint = currentTint;
        }
	}
}
