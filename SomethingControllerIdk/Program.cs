using SFML.Graphics;
using SFML.System;
using SFML.Window;

class Program
{
    public static RenderWindow window = new RenderWindow(new VideoMode(1920, 1080), "hello", Styles.Fullscreen, new ContextSettings() { AntialiasingLevel = 8 });
    public static Random rng = new Random();
    public static float DeltaTime;
    static void Main()
    {
        //window.SetFramerateLimit(144);
        window.SetVerticalSyncEnabled(true);

        Queue<ButtonPrompt> sequence = new Queue<ButtonPrompt>();
        //Score score = new Score((Vector2f)window.Size * 0.66f, (int)MathF.Round(0.05f * window.Size.Y));
        Score score = new Score(new Vector2f(10,10), (int)MathF.Round(0.04f * window.Size.Y));
        Color bgColor = new Color(30, 30, 50);

        ButtonPrompt.ControllerType input = ButtonPrompt.ControllerType.Xbox;
        ButtonPrompt.ControllerType layout = ButtonPrompt.ControllerType.PlayStation;
        Vector2f position = (Vector2f)window.Size / 2 + new Vector2f(0, 300);

        for (int i = 0; i < 100; i++)
        {
            int buttonType = rng.Next(3);
            switch (buttonType)
            {
                case 0:
                    sequence.Enqueue(new ButtonPrompt(rng.Next(4), rng.Next(100 - i/2, 200 - i/2) / 100f * 60, position, input, layout));
                    break;
                case 1:
                    int count = rng.Next(3, 6);
                    int[] buttons = new int[count];
                    for (int o = 0; o < count; o++)
                        buttons[o] = rng.Next(4);
                    sequence.Enqueue(new ButtonPrompt(buttons, rng.Next(400 - i / 2, 500 - i / 2) / 100f * 60, position, input, layout));
                    break;
                case 2:
                    sequence.Enqueue(new ButtonPrompt(rng.Next(4), rng.Next(200 - i / 2, 300 - i / 2) / 100f * 60, rng.Next(3, 7), position, input, layout));
                    break;
            }
        }

        //CircleShape ass = new CircleShape()
        //{
        //    FillColor = Color.Cyan,
        //    Radius = 50
        //};

        //ass.Position = new Vector2f(200, 200);
        Time Time;
        Clock deltaClock = new Clock();
        while (window.IsOpen)
        {
            Time = deltaClock.Restart();
            DeltaTime = 60f / (1f / Time.AsSeconds());
            Joystick.Update();

            //ass.Position += new Vector2f(0.1f, 0) * Joystick.GetAxisPosition(0, Joystick.Axis.X);
            //ass.Position += new Vector2f(0, 0.1f) * Joystick.GetAxisPosition(0, Joystick.Axis.Y);

            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape) || Joystick.IsButtonPressed(0, (uint)ButtonPrompt.PlayStationButtons.Start))
                Environment.Exit(0);

            if (Keyboard.IsKeyPressed(Keyboard.Key.Num1))
                foreach (var s in sequence)
                    for (int i = 0; i < s.Buttons.Length; i++)
                        s.Sprites[i].Texture = Res.PS_buttons[s.Buttons[i]];

            if (Keyboard.IsKeyPressed(Keyboard.Key.Num2))
                foreach (var s in sequence)
                    for (int i = 0; i < s.Buttons.Length; i++)
                        s.Sprites[i].Texture = Res.XBox_buttons[s.Buttons[i]];

            if (sequence.Count > 0)
            {
                sequence.Peek().Update(score);
                if (sequence.Peek().Done && rng.Next((int)MathF.Round(30 / DeltaTime)) == 0)
                    sequence.Dequeue();
            }

            window.Clear(bgColor);

            window.Draw(score.text);
            if (sequence.Count > 0)
            {
                foreach (var SequenceSprites in sequence.Peek().Sprites)
                    window.Draw(SequenceSprites);
                foreach (var timer in sequence.Peek().TimerShape)
                    window.Draw(timer);
                window.Draw(sequence.Peek().SplashSprite);
            }
            //window.Draw(ass);

            window.Display();
            window.DispatchEvents();
        }
    }
}