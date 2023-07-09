using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingItem : MonoBehaviour {
    float start_y = 0.0f;
    void Awake() {
        start_y = transform.position.y;
    }
    void Update() {
        float x = transform.position.x;
        float y = start_y + Mathf.Sin( Time.time );
        transform.position = new Vector2( x, y );
    }
}