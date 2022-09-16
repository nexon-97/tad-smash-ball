using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 1.0f;
    public float acceleration = 5.0f;
    public float bounceSpeed = 0.25f;
    
    private float velocity = 0.0f;

    void Update()
    {
        if (GameSession.Get().GameState == GameState.InProgress)
		{
            // Move the ball
            Vector3 ballPos = transform.position;
            velocity = Mathf.Clamp(velocity + acceleration * Time.deltaTime, -maxSpeed, bounceSpeed);
            ballPos.y += velocity;
            transform.position = ballPos;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Finish")
		{
            // If we have hit finish trigger - notify game is finished
            GameSession.Get().OnFinish();
            return;
		}

        // Skip touches of already passed platform
        if (velocity > 0.001f)
		{
            return;
		}

        // Check if we hit the hole
        PlatformMesh platformMesh = other.gameObject.GetComponent<PlatformMesh>();
        
        PlatformMesh.HitResult hit = platformMesh.CheckHit();
        if (hit.death)
		{
            // Notify defeat
		}
        else if (hit.collisionHit)
		{
            // Do bounce
            velocity = bounceSpeed;

            // Play bounce sound
            GetComponent<AudioSource>().Play();
        }
        else
		{
            GameSession.Get().AddScore(10);

            // Play platform pass sound
            other.gameObject.GetComponent<AudioSource>().Play();

            // Destroy object in 1 second
            GameObject.Destroy(other.gameObject, 0.3f);
        }
    }
}
