using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

class Star
{
    public Sprite sprite;
    public Vector2f startingPosition;
    public Vector2f normalizedDirection;
    public int aliveTicks;
}