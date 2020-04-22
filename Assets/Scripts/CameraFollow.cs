using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float lerpingSpeed;
    private float originalX;

    public delegate void ParallaxCameraDelegate( float deltaMovement );
    public ParallaxCameraDelegate onCameraTranslate;

    public void Start()
    {
        originalX = transform.position.x;
    }
    // Update is called once per frame
    private void Update()
    {
        if (target != null)
        {
            float xPos = Mathf.Lerp(transform.position.x, target.position.x, Time.deltaTime * lerpingSpeed);
            transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
            float delta = originalX - xPos;
            onCameraTranslate?.Invoke(delta);
            originalX = xPos;
        }
    }
}
