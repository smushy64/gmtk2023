using UnityEngine;

namespace Player {
    public class CameraController: MonoBehaviour {
        [SerializeField]
        Transform trackingTransform;
        [SerializeField]
        float minX, maxX;
        [SerializeField]
        float minY, maxY;

        float aspect;
        float half_size;

        void Awake() {
            Camera camera = GetComponent<Camera>();
            half_size = camera.orthographicSize;
            aspect = camera.aspect;
        }

        private void Update() {
            float x = trackingTransform.position.x;
            float y = trackingTransform.position.y;

            float x_size = half_size * aspect;
            if( x - x_size < minX ) {
                x = minX + x_size;
            } else if( x + x_size > maxX ) {
                x = maxX - x_size;
            }

            if( y - half_size < minY ) {
                y = minY + half_size;
            } else if( y + half_size > maxY ) {
                y = maxY - half_size;
            }

            transform.position = new Vector3( x, y, -10.0f );
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(
                (Vector3.right * minX) +
                (Vector3.down * 100.0f),
                (Vector3.right * minX) +
                (Vector3.up * 100.0f)
            );
            Gizmos.DrawLine(
                (Vector3.right * maxX) +
                (Vector3.down * 100.0f),
                (Vector3.right * maxX) +
                (Vector3.up * 100.0f)
            );
            Gizmos.DrawLine(
                (Vector3.up * minY) +
                (Vector3.left * 100f),
                (Vector3.up * minY) + 
                (Vector3.right * 100f)
            );
            Gizmos.DrawLine(
                (Vector3.up * maxY) +
                (Vector3.left * 100f),
                (Vector3.up * maxY) + 
                (Vector3.right * 100f)
            );
        }
    }
}