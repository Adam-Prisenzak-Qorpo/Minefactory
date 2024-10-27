using UnityEngine;

namespace Minefactory.Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [Range(0, 1)]
        public float smoothTime = 0.1f;
        public Transform target;

        public void FixedUpdate()
        {
            Vector3 position = GetComponent<Transform>().position;

            position.x = Mathf.Lerp(position.x, target.position.x, smoothTime);
            position.y = Mathf.Lerp(position.y, target.position.y, smoothTime);

            GetComponent<Transform>().position = position;
        }
    }
}
