using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceMovement : MonoBehaviour {
    [SerializeField] private float amplitude = 1;
    private Vector3 startPos;
    void Start() {
        startPos = transform.position;
    }


    void Update(){
        transform.position = startPos + amplitude * new Vector3(0f, Mathf.Sin(Time.time), 0f);
    }
}
