using System;

[Serializable]
public class PlayerState
{
    public float x, y;  // Position
    public float vx, vy; // Velocity

    // Constructor for easy initialization
    public PlayerState(float x, float y, float vx, float vy)
    {
        this.x = x;
        this.y = y;
        this.vx = vx;
        this.vy = vy;
    }
}
