using Home;
using FurnitureLibrary;
using Util;
using FormGame;
using System.Runtime.CompilerServices;

namespace 3D_Engine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to a simulation of everything");
            Game game = new Game();
            game.Run();


        }
    }
}
