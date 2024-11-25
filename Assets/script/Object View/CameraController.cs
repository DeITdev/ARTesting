using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The 3D object to focus on
    public float distance = 5.0f; // Initial distance from the object
    public float zoomSpeed = 2.0f; // Speed of zooming in/out for mouse
    public float touchZoomSpeed = 0.05f; // Reduced speed of zooming for touch
    public float rotationSpeed = 5.0f; // Speed of rotating around the object for mouse
    public float touchRotationSpeed = 0.05f; // Reduced speed of rotating for touch
    public float smoothTime = 0.2f; // Increased smooth time for smoother stop

    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private float velocityX = 0.0f; // Velocity for smooth stopping in X direction
    private float velocityY = 0.0f; // Velocity for smooth stopping in Y direction
    private float zoomVelocity = 0.0f; // Velocity for smooth stopping of zoom
    public float minYAngle = 10.0f; // Minimum vertical angle to limit the camera
    public float maxYAngle = 80.0f; // Maximum vertical angle to limit the camera

    public float minDistance = 0.5f; // Minimum distance (closer to object)
    public float maxDistance = 20.0f; // Maximum distance (further from object)

    private bool isRotating = false; // Flag to check if the camera is rotating
    private bool isZooming = false;  // Flag to check if the camera is zooming

    void Update()
    {
        // --- MOUSE INPUT ---
        if (Input.GetMouseButton(1)) // Right mouse button for rotation
        {
            isRotating = true;
            velocityX = Input.GetAxis("Mouse X") * rotationSpeed;
            velocityY = -Input.GetAxis("Mouse Y") * rotationSpeed;
        }
        else
        {
            isRotating = false;
        }

        // Zoom with mouse scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            isZooming = true;
            zoomVelocity = scroll * zoomSpeed;
        }
        else
        {
            isZooming = false;
        }

        // --- TOUCH INPUT ---
        if (Input.touchCount == 1) // One finger for rotation
        {
            isRotating = true;

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                velocityX = touch.deltaPosition.x * touchRotationSpeed; // Reduced sensitivity for touch
                velocityY = -touch.deltaPosition.y * touchRotationSpeed;
            }
        }
        else if (Input.touchCount == 2) // Two fingers for pinch-to-zoom
        {
            // Only allow zooming with two fingers; disable rotation
            isRotating = false;
            isZooming = true;

            // Get both touches
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Only allow pinch-to-zoom if both fingers are touching the screen
            if (touchZero.phase == TouchPhase.Stationary || touchZero.phase == TouchPhase.Moved ||
                touchOne.phase == TouchPhase.Stationary || touchOne.phase == TouchPhase.Moved)
            {
                // Find the position in the previous frame of each touch
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Calculate the magnitude of the vector (distance) between touches in each frame
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame
                float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

                // Adjust zoom based on the pinch gesture, fixing the direction
                // Spread fingers apart (positive delta) to zoom in, pinch together (negative delta) to zoom out
                zoomVelocity = deltaMagnitudeDiff * touchZoomSpeed;
            }
        }
        else
        {
            isRotating = false;
            isZooming = false;
        }
    }

    void LateUpdate()
    {
        // Handle camera rotation (for both mouse and touch)
        if (isRotating)
        {
            currentX += velocityX;
            currentY += velocityY;
        }
        else
        {
            // Gradually reduce the velocity for a smooth stop in rotation
            velocityX = Mathf.Lerp(velocityX, 0, smoothTime);
            velocityY = Mathf.Lerp(velocityY, 0, smoothTime);
            currentX += velocityX;
            currentY += velocityY;
        }

        // Clamp the Y rotation angle to avoid flipping the camera
        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);

        // Handle camera zooming (for both mouse and touch)
        if (isZooming)
        {
            distance -= zoomVelocity;
        }
        else
        {
            // Gradually reduce the zoom velocity for a smooth stop
            zoomVelocity = Mathf.Lerp(zoomVelocity, 0, smoothTime);
            distance -= zoomVelocity;
        }

        // Clamp the zoom distance to prevent getting too close or too far
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // Calculate the camera's position and rotation
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = new Vector3(0, 0, -distance);
        transform.position = target.position + rotation * direction;
        transform.LookAt(target); // Always look at the target
    }
}
