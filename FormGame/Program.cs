using Microsoft.Xna.Framework;
using System.Drawing.Text;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using SharpDX.Direct3D9;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using System.Net.PeerToPeer;
using SharpDX.Direct3D11;
using System.Windows.Forms;

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
