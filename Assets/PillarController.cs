using UnityEngine;

public class PillarController : MonoBehaviour
{
    public float sensivity = 0.5f;

    private float pillarRotation;

    void Start()
    {
        pillarRotation = 0.0f;
    }

    void Update()
    {
        if (GameSession.Get().GameState == GameState.InProgress)
        {
            // Rotate pillar with platforms
            float offset = Input.GetAxis("Mouse X");
            pillarRotation += (offset * sensivity);
            transform.rotation = Quaternion.Euler(new Vector3(0, pillarRotation, 0));
        }
    }
}
