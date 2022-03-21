using SFML.Graphics;
using SFML.System;
using SFML.Window;

class ButtonPrompt : Program
{
    public List<Sprite> Sprites = new List<Sprite>();
    public Sprite SplashSprite = new Sprite();
    public List<Sprite> TimerShape = new List<Sprite>();
    public List<bool> IsPressed = new List<bool>();

    int timerShpCount = 33;

    public uint[] Buttons;
    public float LifeSpan;
    public bool Done = false;

    Vector2f failDir;

    Vector2f Position;
    PromptType PromType;
    ControllerType Input; // TODO: Support for *both* PS4 and Xbox controllers
    ControllerType Layout;

    int tick;

    public ButtonPrompt(int button, float lifeSpan, Vector2f position, ControllerType input, ControllerType layout)
    {
        Buttons = new uint[1] { (uint)button };
        LifeSpan = lifeSpan;
        Input = input;
        Layout = layout;
        Position = position;
        PromType = PromptType.Single;

        CreateSingle();
    }
    public ButtonPrompt(int[] buttons, float lifeSpan, Vector2f position, ControllerType input, ControllerType layout)
    {
        Buttons = new uint[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
            Buttons[i] = (uint)buttons[i];
        Input = input;
        Layout = layout;
        LifeSpan = lifeSpan;
        Position = position;
        PromType = PromptType.Chained;

        CreateChained();
    }
    public ButtonPrompt(int button, float lifeSpan, int mashCount, Vector2f position, ControllerType input, ControllerType layout)
    {
        Input = input;
        Layout = layout;
        Buttons = new uint[1] { (uint)button };
        LifeSpan = lifeSpan;
        Position = position;
        PromType = PromptType.Mash;

        CreateMash(mashCount);
    }

    float objectsToDelete;
    bool success = false;
    bool fail = false;

    public void Update(Score score)
    {
        switch (PromType)
        {
            case PromptType.Single:
                UpdateSingle(score);
                break;
            case PromptType.Chained:
                UpdateChained(score);
                break;
            case PromptType.Mash:
                UpdateMash(score);
                break;
            default:
                break;
        }
        tick++;
    }

    bool[] isHoldingButton = new bool[4];
    void SplashSpriteUpdate()
    {
        SplashSprite.Scale += new Vector2f(1, 1) * 0.5f * DeltaTime;
        if (SplashSprite.Color.A - 5 * DeltaTime >= 0)
            SplashSprite.Color = new Color(255, 255, 255, (byte)(SplashSprite.Color.A - 5 * DeltaTime));
    }

    void CreateSingle()
    {
        Texture[] buttonsGroup = Layout == ControllerType.PlayStation ? Res.PS_buttons : Res.XBox_buttons;
        //Texture[] buttonsGroup = Res.PS_buttons;

        Sprites.Add(new Sprite(buttonsGroup[Buttons[0]]));
        SplashSprite = new Sprite(Res.button_splash);
        Sprites[0].Origin = SplashSprite.Origin = (Vector2f)Sprites[0].Texture.Size / 2f;
        Sprites[0].Position = SplashSprite.Position = Position;
        Sprites[0].Scale = SplashSprite.Scale = new Vector2f(0.5f, 0.5f);
        failDir = new Vector2f(rng.Next(-100, 100) / 100f, rng.Next(50, 100) / 100f);

        float timerShpInterval = 2 * MathF.PI / timerShpCount;
        for (int i = 0; i < timerShpCount; i++)
        {
            TimerShape.Add(new Sprite(Res.timer)
            {
                Position = Sprites[0].Position + new Vector2f(MathF.Cos(3 * MathF.PI / 2 - timerShpInterval * i), MathF.Sin(3 * MathF.PI / 2 + timerShpInterval * i)) * 200,
                Origin = new Vector2f(50, 50) / 2,
            });
        }
        IsPressed.Add(new bool());
    }
    void UpdateSingle(Score score)
    {
        SplashSpriteUpdate();

        if (IsPressed[0])
        {
            Done = true;
            return;
        }

        if (success)
        {
            SuccessSequence();
            return;
        }
        if (fail)
        {
            FailSequence();
            return;
        }

        TimerUpdate(score);

        CheckIfWrongButtonIsPressed(Buttons[0], score);

        if (Joystick.IsButtonPressed(0, Buttons[0]))
        {
            score.Add(1);
            success = true;
        }
    }

    void CreateChained()
    {
        Texture[] buttonsGroup = Layout == ControllerType.PlayStation ? Res.PS_buttons : Res.XBox_buttons;
        //Texture[] buttonsGroup = Res.PS_buttons;
        for (int i = 0; i < Buttons.Length; i++)
        {
            Sprites.Add(new Sprite(buttonsGroup[Buttons[i]]));
            IsPressed.Add(new bool());
            SplashSprite = new Sprite(Res.button_splash);

            Sprites[i].Origin = (Vector2f)Sprites[i].Texture.Size / 2f;
            // That shit down there is ugly as hell but it works
            Sprites[i].Position = new Vector2f(Position.X - (Buttons.Length - 1) * Sprites[i].Texture.Size.X / 2 + ((Buttons.Length - 1) * Sprites[i].Texture.Size.X / 2) / 2, 0) + new Vector2f(i * Sprites[i].Texture.Size.X / 2, Position.Y);
            Sprites[i].Scale = new Vector2f(0.5f, 0.5f);
            failDir = new Vector2f(rng.Next(-100, 100) / 100f, rng.Next(50, 100) / 100f);
        }
        SplashSprite = new Sprite(Res.button_splash)
        {
            Origin = Sprites[0].Origin,
            Position = Position,
            Scale = new Vector2f(MathF.Ceiling((Buttons.Length + 1) / 2f), 0.2f)
        };

        float timerShpInterval = 1f/timerShpCount * window.Size.X/2f;
        for (int i = 0; i < timerShpCount; i++)
        {
            TimerShape.Add(new Sprite(Res.timer)
            {
                Position = new Vector2f(
                   Position.X - ((timerShpCount - 1) * timerShpInterval) / 2 + i * timerShpInterval,
                   Position.Y + 150
                ),
                Origin = (Vector2f)Res.timer.Size / 2,
            });
        }
    }
    void UpdateChained(Score score)
    {
        SplashSpriteUpdate();

        if (fail)
        {
            if (Sprites[^1].Color.A == 0)
                Done = true;
            for (int i = 0; i < Buttons.Length; i++)
                FailSequence(i);
            return;
        }

        int PressedButtonsCount = 0;
        for (int i = 0; i < Buttons.Length; i++)
            if (IsPressed[i])
            {
                SuccessSequence(i);
                PressedButtonsCount++;
            }

        int CurrentButton = Buttons.Length - Sprites.Count + PressedButtonsCount;

        if (PressedButtonsCount < Buttons.Length)
        {
            if (!isHoldingButton[Buttons[CurrentButton]] &&
                !IsPressed[CurrentButton] &&
                Joystick.IsButtonPressed(0, Buttons[CurrentButton]))
            {
                score.Add(1);
                IsPressed[CurrentButton] = true;
            }
            CheckIfWrongButtonIsPressed(Buttons[CurrentButton], score);
        }
        else
            success = true;

        if (Sprites[^1].Color.A == 0)
            Done = true;

        if (!success)
            TimerUpdate(score, true);

        CheckControllerButtonsState();
    }

    void CreateMash(int mashCount)
    {
        Texture[] buttonsGroup = Layout == ControllerType.PlayStation ? Res.PS_buttons : Res.XBox_buttons;
        //Texture[] buttonsGroup = Res.PS_buttons;

        Sprites.Add(new Sprite(buttonsGroup[Buttons[0]]));
        SplashSprite = new Sprite(Res.button_splash);
        Sprites[0].Origin = SplashSprite.Origin = (Vector2f)Sprites[0].Texture.Size / 2f;
        Sprites[0].Position = SplashSprite.Position = Position;
        Sprites[0].Scale = SplashSprite.Scale = new Vector2f(0.5f, 0.5f);
        failDir = new Vector2f(rng.Next(-100, 100) / 100f, rng.Next(50, 100) / 100f);

        float timerShpInterval = 2 * MathF.PI / timerShpCount;
        for (int i = 0; i < timerShpCount; i++)
        {
            TimerShape.Add(new Sprite(Res.timer)
            {
                Position = Sprites[0].Position + new Vector2f(MathF.Cos(3 * MathF.PI / 2 - timerShpInterval * i), MathF.Sin(3 * MathF.PI / 2 + timerShpInterval * i)) * 200,
                Origin = new Vector2f(50, 50) / 2,
            });
        }
        for (int i = 0; i < mashCount; i++)
            IsPressed.Add(new bool());
    }
    void UpdateMash(Score score)
    {
        if (Sprites[0].Color.A == 0)
        {
            Done = true;
            return;
        }
        SplashSpriteUpdate();
        if (fail)
        {
            FailSequence();
            return;
        }

        int PressedButtonsCount = 0;
        for (int i = 0; i < IsPressed.Count; i++)
            if (IsPressed[i])
                PressedButtonsCount++;

        if (PressedButtonsCount == IsPressed.Count - 1)
        {
            if (!success)
            {
                Sprites[0].Scale = new Vector2f(0.5f, 0.5f);
                score.Add(1);
            }
            success = true;
            SuccessSequence();
            return;
        }

        if (SplashSprite.Color.A < 150)
            SplashSprite = new Sprite(SplashSprite.Texture)
            {
                Position = Sprites[0].Position,
                Origin = Sprites[0].Origin,
                Scale = Sprites[0].Scale
            };
        if (tick % MathF.Round(5f / DeltaTime) == 0)
            Sprites[0].Scale = Sprites[0].Scale.X == 0.4f ? new Vector2f(0.5f, 0.5f) : new Vector2f(0.4f, 0.4f);
        Console.WriteLine(MathF.Round(5f / DeltaTime));
        if (!isHoldingButton[Buttons[0]] && Joystick.IsButtonPressed(0, Buttons[0]))
            IsPressed[PressedButtonsCount] = true;

        TimerUpdate(score);

        CheckControllerButtonsState();
    }

    void CheckControllerButtonsState()
    {
        for (uint i = 0; i < 4; i++)
        {
            if (Joystick.IsButtonPressed(0, i))
                isHoldingButton[i] = true;
            if (!Joystick.IsButtonPressed(0, i))
                isHoldingButton[i] = false;
        }
    }
    void CheckIfWrongButtonIsPressed(uint button, Score score)
    {
        for (int i = 0; i < 4; i++)
            if (!isHoldingButton[i] && Joystick.IsButtonPressed(0, (uint)i) && button != i)
            {
                score.Reset();
                fail = true;
            }
    }
    void TimerUpdate(Score score, bool symmetrical = false)
    {
        objectsToDelete += timerShpCount / (LifeSpan * (symmetrical ? 2 : 1)) * DeltaTime;
        if (objectsToDelete >= 1 && TimerShape.Count > 0)
        {
            for (int i = 0; i < MathF.Floor(objectsToDelete); i++)
            {
                if (TimerShape.Count <= 0)
                    break;
                TimerShape.Remove(TimerShape[0]);
                if (symmetrical)
                    if (TimerShape.Count > 0)
                        TimerShape.Remove(TimerShape[^1]);
            }
            objectsToDelete -= MathF.Floor(objectsToDelete);
        }

        if (TimerShape.Count <= 0)
        {
            score.Reset();
            fail = true;
        }
    }

    void SuccessSequence(int indx = 0)
    {
        if (Sprites[indx].Color.A - 10 * DeltaTime < 0)
        {
            Sprites[indx].Color = new Color(255, 255, 255, 0);
            if (success)
                foreach (var t in TimerShape)
                    t.Color = Sprites[indx].Color;
            //Sprites.Remove(Sprites[indx]);
            IsPressed[indx] = true;
            return;
        }

        Sprites[indx].Scale += new Vector2f(1, 1) * 0.01f * DeltaTime;
        Sprites[indx].Color = new Color(255, 255, 255, (byte)(Sprites[indx].Color.A - 10f * DeltaTime));
        if (fail || success)
            foreach (var t in TimerShape)
                t.Color = Sprites[indx].Color;

    }
    void FailSequence(int indx = 0)
    {
        if (Sprites[indx].Color.A - 5 * DeltaTime < 0)
        {
            Sprites[indx].Color = new Color(255, 0, 0, 0);
            foreach (var t in TimerShape)
                t.Color = Sprites[indx].Color;

            IsPressed[indx] = true;
            return;
        }
        Sprites[indx].Position += failDir * DeltaTime;
        Sprites[indx].Color = new Color(255, 0, 0, (byte)(Sprites[indx].Color.A - 5 * DeltaTime));

        foreach (var t in TimerShape)
            t.Color = Sprites[indx].Color;
    }

    public enum PromptType
    {
        Single,
        Chained,
        Mash
    }
    public enum PlayStationButtons
    {
        Square, Cross, Circle, Triangle,
        L1, R1,
        Select, Start
    }
    public enum XBoxButtons
    {
        Cross, Circle, Square, Triangle,
        L1, R1,
        Select, Start
    }
    public enum ControllerType
    {
        PlayStation,
        Xbox
    }
}