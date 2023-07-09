using UnityEngine;

public class FloatingItem : MonoBehaviour {
    float start_y = 0.0f;
    void Awake() {
        start_y = transform.position.y;
    }
    void Update() {
        float x = transform.position.x;
        float y = start_y + (Mathf.Sin( Time.time * 1.25f ) * 0.4f);
        transform.position = new Vector2( x, y );
    }
}