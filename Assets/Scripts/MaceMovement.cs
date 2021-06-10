using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceMovement : MonoBehaviour{

    private Vector3 startPos;
    void Start(){
        startPos = transform.position;
    }


    void Update(){
        transform.position = startPos + new Vector3(0f, Mathf.Sin(Time.time), 0f);
    }
}
