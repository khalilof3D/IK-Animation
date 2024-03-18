using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTargetMovement : MonoBehaviour
{


    public float Speed = 5.0f;
    public Transform target;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) {
            target.position += Vector3.left * Speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            target.position += Vector3.right * Speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.UpArrow)) {
            target.position += Vector3.up * Speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            target.position += Vector3.down * Speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W)) {
            target.position += Vector3.forward * Speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S)) {
            target.position += Vector3.back * Speed * Time.deltaTime;
        }
    }
}
