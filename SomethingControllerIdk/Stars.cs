using SFML.Graphics;
using SFML.System;

class Stars : Program
{
    List<Star> stars = new List<Star>();

    List<int> toRemove = new List<int>();
    RenderTexture texture = new RenderTexture(WIN_DIM.X, WIN_DIM.Y);
    public Sprite sprite;

    FloatRect windowsBounds = new FloatRect(-Res.stars[0].Size.X, -Res.stars[0].Size.Y, WIN_DIM.X, WIN_DIM.Y);

    public Stars()
    {
        sprite = new Sprite(texture.Texture);
        for (int i = 0; i < 256; i++)
            AddRandomStar();
    }

    Clock clock = new Clock();

    int tick;
    public void Update()
    {
        texture.Clear(Color.Transparent);
        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].sprite.Position -= stars[i].normalizedDirection * 3 * stars[i].timeAlive.ElapsedTime.AsSeconds() * DeltaTime;

            if (stars[i].sprite.Color.A < 255 - Math.Ceiling(5 * DeltaTime))
                stars[i].sprite.Color = new Color(255, 255, 255, (byte)(stars[i].sprite.Color.A + Math.Ceiling(5 * DeltaTime)));

            stars[i].sprite.Scale += new Vector2f(0.01f, 0.01f) * DeltaTime;

            if (stars[i].sprite.Position.X > windowsBounds.Width ||
                stars[i].sprite.Position.X < windowsBounds.Left * stars[i].sprite.Scale.X ||
                stars[i].sprite.Position.Y > windowsBounds.Height ||
                stars[i].sprite.Position.Y < windowsBounds.Top * stars[i].sprite.Scale.Y)
            {
                toRemove.Add(i);
                continue;
            }
            texture.Draw(stars[i].sprite);
        }
        texture.Display();
        foreach (var remove in toRemove)
            stars[remove].dead = true;

        List<Star> newStarsList = new List<Star>();
        for (int i = 0; i < stars.Count; i++)
            if (!stars[i].dead) //aka not out of bounds
                newStarsList.Add(stars[i]);

        stars = newStarsList;
        toRemove.Clear();
        if (clock.ElapsedTime.AsMilliseconds() > 15)
        {
            clock.Restart();
            for (int i = 0; i < 10; i++)
                AddRandomStar();
        }
        tick++;
    }

    void AddRandomStar()
    {
        stars.Add(new Star());
        stars[^1].sprite = new Sprite(Res.stars[rng.Next(Res.stars.Length)])
        {
            Position = new Vector2f(rng.Next(1920), rng.Next(1080)),
            //Position = new Vector2f(1920, 1080) / 2,
            Color = new Color(255, 255, 255, 0),
            Scale = new Vector2f(0.1f, 0.1f)
        };
        stars[^1].startingPosition = stars[^1].sprite.Position;
        stars[^1].dead = false;

        Vector2f dir = (Vector2f)WIN_DIM / 2f - stars[^1].startingPosition;
        Vector2f normalizedDirection = dir / MathF.Sqrt(dir.X * dir.X + dir.Y * dir.Y);
        stars[^1].normalizedDirection = normalizedDirection;
    }
}