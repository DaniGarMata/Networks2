using UnityEngine;

public class Movement : MonoBehaviour
{
    public Vector2 speed = new Vector2(100.1f, 0.1f);
    private PlayerState playerState;

    void Start()
    {
        // Initialize PlayerState with the current position and velocity
        playerState = new PlayerState(transform.position.x, transform.position.y, 0f, 0f);
    }

    void Update()
    {
        // Update the player's state based on input
        playerState.x += Input.GetAxis("Horizontal") * speed.x * Time.deltaTime;
        playerState.y += Input.GetAxis("Vertical") * speed.y * Time.deltaTime;

        // Update the position in the scene
        transform.position = new Vector2(playerState.x, playerState.y);
    }

    public void SetPlayerState(PlayerState state)
    {
        // This method will be used to update the player position from server-side state
        playerState = state;
        transform.position = new Vector2(playerState.x, playerState.y);
    }
}
