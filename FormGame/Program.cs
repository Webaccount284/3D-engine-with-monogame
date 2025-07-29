using Microsoft.Xna.Framework;
using System.Drawing.Text;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using System.Linq;
using Microsoft.Xna.Framework.Audio;
using System.Net.PeerToPeer;


class Program
{
    static void Main(string[] args)
    {
        // TODO
        /*
         * improve world generation
         * add collision 
         * add more blocks
        */
        var game = new FormGame.Game();
        game.Run();
        game.Dispose();
    }
}
