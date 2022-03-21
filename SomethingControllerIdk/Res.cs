using SFML.Graphics;

interface Res
{
    static Texture[] PS_buttons = new Texture[]
    {
        AddTexture("cross"),
        AddTexture("circle"),
        AddTexture("square"),
        AddTexture("triangle")
    };
    static Texture[] XBox_buttons = new Texture[]
    {
        AddTexture("A"),
        AddTexture("B"),
        AddTexture("X"),
        AddTexture("Y")
    };

    static Texture button_splash = AddTexture("splash");
    static Texture timer = AddTexture("timer");

    static Texture[] stars = AddTextures("star");

    private static Texture AddTexture(string path)
    {
        try
        {
            return new Texture(@"res\" + path + ".png") { Smooth = true };
        }
        catch (Exception)
        {
            RenderTexture renderTexture = new RenderTexture(50, 50);
            renderTexture.Clear(new Color(255, 0, 255));
            renderTexture.Display();
            return renderTexture.Texture;
        }
    }
    private static Texture[] AddTextures(string path)
    {
        int indx = 0;
        List<Texture> textures = new List<Texture>();
        while (File.Exists(@"res\" + path + indx + ".png"))
        {
            textures.Add(new Texture(@"res\" + path + indx + ".png") { Smooth = true });
            indx++;
        }
        return textures.ToArray();
    }
}