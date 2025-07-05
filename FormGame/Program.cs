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
         * add world class getting the mesh of one chunk
         * change instantiation of world class to not generate all the chunks, only the map for the chunks
         * improve world generation
         * 
        */
        var game = new FormGame.Game();
        game.Run();
        game.Dispose();
    }
}
