using SFML.Graphics;
using SFML.System;
using SFML.Window;

class Program
{
    public static RenderWindow window = new RenderWindow(new VideoMode(1920, 1080), "hello", Styles.Fullscreen, new ContextSettings() { AntialiasingLevel = 8 });
    //public static RenderWindow window = new RenderWindow(new VideoMode(1500, 900), "hello", Styles.Default, new ContextSettings() { AntialiasingLevel = 8 });
    public static Random rng = new Random();
    public static float DeltaTime = 1;
    static void Main()
    {
        //window.SetView(new View(new FloatRect(-1920, -1080, 1920 * 3, 1080 * 3))); // for out of bounds view
        //window.SetFramerateLimit(60);
        window.SetMouseCursorVisible(false);
        window.SetVerticalSyncEnabled(true);

        Queue<ButtonPrompt> sequence = new Queue<ButtonPrompt>();
        Score score = new Score(new Vector2f(10, 10), (int)MathF.Round(0.04f * window.Size.Y));
        Stars sky = new Stars();
        Color bgColor = new Color(0, 10, 30);
        ButtonPrompt.ControllerType input = ButtonPrompt.ControllerType.Xbox;
        ButtonPrompt.ControllerType layout = ButtonPrompt.ControllerType.PlayStation;
        Vector2f position = new Vector2f(window.Size.X / 2, window.Size.Y * 0.80f);

        int tick = 0;

        for (int i = 0; i < 100; i++)
        {
            int buttonType = rng.Next(3);
            switch (buttonType)
            {
                case 0:
                    sequence.Enqueue(new ButtonPrompt(rng.Next(4), rng.Next(100 - i / 2, 200 - i / 2) / 100f * 60, position, input, layout));
                    break;
                case 1:
                    int count = rng.Next(3, 6);
                    int[] buttons = new int[count];
                    for (int o = 0; o < count; o++)
                        buttons[o] = rng.Next(4);
                    sequence.Enqueue(new ButtonPrompt(buttons, rng.Next(300 - i / 2, 400 - i / 2) / 100f * 60, position, input, layout));
                    break;
                case 2:
                    sequence.Enqueue(new ButtonPrompt(rng.Next(4), rng.Next(200 - i / 2, 300 - i / 2) / 100f * 60, rng.Next(3, 7), position, input, layout));
                    break;
            }
        }

        Clock deltaClock = new Clock();
        Clock clock = new Clock();
        while (window.IsOpen)
        {
            float LastDelta = DeltaTime;
            DeltaTime = 60 * deltaClock.Restart().AsSeconds();
            //if (DeltaTime - LastDelta > 1) // If there are some hiccups in framerate, try to combat them
            //    DeltaTime = LastDelta;
            Joystick.Update();

            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape) || Joystick.IsButtonPressed(0, (uint)ButtonPrompt.PlayStationButtons.Start))
                Environment.Exit(0);

            

            if (Keyboard.IsKeyPressed(Keyboard.Key.Num3))
                window.SetFramerateLimit(10);

            if (Keyboard.IsKeyPressed(Keyboard.Key.Num4))
                window.SetFramerateLimit(60);

            if (Keyboard.IsKeyPressed(Keyboard.Key.Num5))
                window.SetFramerateLimit(144);

            if (Keyboard.IsKeyPressed(Keyboard.Key.Num1))
                foreach (var s in sequence)
                    for (int i = 0; i < s.Buttons.Length; i++)
                        s.Sprites[i].Texture = Res.PS_buttons[s.Buttons[i]];

            if (Keyboard.IsKeyPressed(Keyboard.Key.Num2))
                foreach (var s in sequence)
                    for (int i = 0; i < s.Buttons.Length; i++)
                        s.Sprites[i].Texture = Res.XBox_buttons[s.Buttons[i]];

            if (clock.ElapsedTime.AsSeconds() > 3)
                if (sequence.Count > 0)
                {
                    sequence.Peek().Update(score);
                    if (sequence.Peek().Done && rng.Next((int)MathF.Round(30 / DeltaTime)) == 0)
                        sequence.Dequeue();
                }

            sky.Update();
            tick++;
            /* =============== DRAW =============== */

            window.Clear(bgColor);

            //foreach (var s in sky.stars)
            //    window.Draw(s.sprite);
            window.Draw(sky.sprite);
            window.Draw(score.text);
            if (clock.ElapsedTime.AsSeconds() > 3)
                if (sequence.Count > 0)
                {
                    foreach (var SequenceSprites in sequence.Peek().Sprites)
                        window.Draw(SequenceSprites);
                    foreach (var timer in sequence.Peek().TimerShape)
                        window.Draw(timer);
                    window.Draw(sequence.Peek().SplashSprite);
                }

            window.Display();
            window.DispatchEvents();
        }
    }
}