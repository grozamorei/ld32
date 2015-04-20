using UnityEngine;
using System.Collections;

public class CellRotator : MonoBehaviour {

    public float rotationSpeed = 180f;
    float currentAngleAxis = 0;
    float endAngleAxis = 180;
    bool rotating = false;
    
    public GameObject nanoMachine;
    public SkinnedMeshRenderer mesh;
    
    Color startTint = Color.white;
    Color tartetTint = Color.red;
    Color currentTint = Color.black;
    
    public void setColor(Color tint)
    {
        mesh.material.SetColor("_TintColor", new Color(tint.r, tint.g, tint.b, 0.5f));
//        GetComponent<SpriteRenderer>().color = tint;
//        Renderer rend = GetComponent<Renderer>();
//        rend.material.shader = Shader.Find("Specular");
//        rend.material.SetColor("_SpecColor", Color.red);
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
