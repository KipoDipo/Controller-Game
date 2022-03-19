using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    private static Texture AddTexture(string path)
    {
        return new Texture(@"res\" + path + ".png") { Smooth = true };
    }
}