using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

class Score
{
    public Text text;
    int score;
    int highScore;
    Font font = new Font(@"res\ARLRDBD.TTF");
    public Score(Vector2f position, int textSize)
    {
        text = new Text("", font, (uint)textSize)
        {
            Position = position,
            FillColor = Color.White
        };
        Update();
    }

    public void Add(int score)
    {
        this.score += score;
        Update();
    }
    public void Reset()
    {
        if (score > highScore)
            highScore = score;
        score = 0;
        Update();
    }
    public void Update()
    {
        text.DisplayedString =
            "Score: " + score + "\n" +
            "High Score: " + highScore + "\n";
    }
}
