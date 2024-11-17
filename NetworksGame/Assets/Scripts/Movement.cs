using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public Vector2 speed = new Vector2(100.1f, 0.1f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        Debug.Log("Hit " + inputX);

        transform.Translate(speed.x * inputX * Time.deltaTime, speed.y * inputY * Time.deltaTime, 0.0f);
    }
}
