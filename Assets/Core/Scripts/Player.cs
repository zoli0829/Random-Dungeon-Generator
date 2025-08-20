using UnityEngine;

public class Player : MonoBehaviour
{
    Transform GFX;
    float flipX;
    
    void Start()
    {
        GFX = GetComponentInChildren<SpriteRenderer>().transform;
        flipX = GFX.LocalScale.x;
    }
    
    void Update()
    {
        float horz = System.Math.Sign(Input.GetAxisRaw("Horizontal"));
        if (Mathf.Abs(horz) > 0) 
        {
            GFX.LocalScale = new Vector2(flipX * horz, GFX.LocalScale.y);
        }
    }
}
