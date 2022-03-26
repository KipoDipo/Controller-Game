using SFML.Graphics;
using SFML.System;

class Star
{
    public Sprite sprite;
    public Vector2f startingPosition;
    public Vector2f normalizedDirection;
    public bool dead;
    public Clock timeAlive = new Clock();
}