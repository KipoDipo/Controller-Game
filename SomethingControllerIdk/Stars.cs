using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

class Stars : Program
{
    public List<Star> stars = new List<Star>();

    List<int> toRemove = new List<int>();

    public Stars()
    {
        for (int i = 0; i < 256; i++)
            AddRandomStar();
    }

    int tick;
    public void Update()
    {
        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].sprite.Position -= stars[i].normalizedDirection * 3 * ((stars[i].aliveTicks * DeltaTime) / 100f) * DeltaTime;
            
            stars[i].aliveTicks++;

            if (stars[i].sprite.Color.A < 255 - 5)
                stars[i].sprite.Color = new Color(255, 255, 255, (byte)(stars[i].sprite.Color.A + 5));

            stars[i].sprite.Scale += new Vector2f(0.01f, 0.01f) * DeltaTime;

            if (stars[i].sprite.Position.X > window.Size.X ||
                stars[i].sprite.Position.X < -stars[i].sprite.Scale.X * stars[i].sprite.Texture.Size.X ||
                stars[i].sprite.Position.Y > window.Size.Y ||
                stars[i].sprite.Position.Y < -stars[i].sprite.Scale.Y * stars[i].sprite.Texture.Size.Y)
            {
                toRemove.Add(i);
            }
        }
        foreach (var remove in toRemove)
            stars[remove].aliveTicks = -1;

        List<Star> newStarsList = new List<Star>();
        for (int i = 0; i < stars.Count; i++)
            if (stars[i].aliveTicks != -1) //aka not out of bounds
                newStarsList.Add(stars[i]);

        stars = newStarsList;
        toRemove.Clear();
        if (tick % MathF.Round(1f / DeltaTime) == 0)
            for (int i = 0; i < 10; i++)
                AddRandomStar();
        tick++;
    }

    void AddRandomStar()
    {
        stars.Add(new Star());
        stars[^1].sprite = new Sprite(Res.stars[rng.Next(Res.stars.Length)])
        {
            Position = new Vector2f(rng.Next(1920), rng.Next(1080)),
            Color = new Color(255, 255, 255, 0),
            Scale = new Vector2f(0.1f, 0.1f)
        };
        stars[^1].startingPosition = stars[^1].sprite.Position;
        stars[^1].aliveTicks = 0;

        Vector2f dir = (Vector2f)window.Size / 2f - stars[^1].startingPosition;
        Vector2f normalizedDirection = dir / MathF.Sqrt(dir.X * dir.X + dir.Y * dir.Y);
        stars[^1].normalizedDirection = normalizedDirection;
    }
}