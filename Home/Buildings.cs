using FurnitureLibrary;
using Util;
namespace Home
{
    public abstract class Building
    {
        Window[] windows;
        Door[] doors;
        Wall[] walls;
        Furniture[] furnitures;
        public Building() 
        {
            this.windows = new Window[0];
            this.doors = new Door[0];
            this.walls = new Wall[0];
            this.furnitures = new Furniture[0];
        }
        public Building(Window[] windows, Door[] doors, Wall[] walls, Furniture[] furnitures)
        {
            this.windows = windows;
            this.doors = doors;
            this.walls = walls;
            this.furnitures = furnitures;
        }
    }
    public class Window
    {
        int width;
        int height;
        Position position;
        Texture texture;
        public Window()
        {
            width = 0;
            height = 0;
            position = new Position();
            texture = new Texture();
        }
        public Window(int width, int height, Position position, Texture texture)
        {
            this.width = width;
            this.height = height;
            this.position = position;
            this.texture = texture;
        }
    }
    public class Door
    {
        int width;
        int height;
        Position position;
        Texture texture1;
        Texture texture2;
        bool opened = false;
        public Door()
        {
            width = 0;
            height = 0;
            position = new Position();
            this.texture1 = new Texture();
            this.texture2 = new Texture();
        }
        public Door(int width, int height, Position position, Texture texture1, Texture texture2)
        {
            this.width = width;
            this.height = height;
            this.position = position;
            this.texture1 = texture1;
            this.texture2 = texture2;
        }
        public void OpenDoor()
        {
            opened = true;
        }
        public void ShutDoor()
        {
            opened = false;
        }
    }
    public class Wall
    {
        int width;
        int height;
        Position position;
        Texture texture;
        public Wall()
        {
            width = 0;
            height = 0;
            position = new Position();
            this.texture = new Texture();
        }
        public Wall(int width, int height, Position position, Texture texture)
        {
            this.width = width;
            this.height = height;
            this.position = position;
            this.texture = texture;
        }
    }
}
