using UnityEngine;

// Generally camera position must be focused on the "current" platform position or on ball during transition between platforms
// While ball is moving to next platform, camera is focusing him until it hits next platform, then camera locks on that platform

public class CameraController : MonoBehaviour
{
    public Transform ballTransform;

    private Vector3 ballOffset;

	private void Start()
	{
		ballOffset = transform.position - ballTransform.position;
	}

	void Update()
    {
		// Follow the ball only in one direction, not in other
		Vector3 newCameraPos = ballTransform.position + ballOffset;
		if (newCameraPos.y < transform.position.y)
		{
			transform.position = newCameraPos;
		}
	}
}
