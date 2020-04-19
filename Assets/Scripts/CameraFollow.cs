using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float lerpingSpeed;

    // Update is called once per frame
    private void Update()
    {
        if (target != null)
        {
            float xPos = Mathf.Lerp(transform.position.x, target.position.x, Time.deltaTime * lerpingSpeed);
            transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
        }
    }
}
