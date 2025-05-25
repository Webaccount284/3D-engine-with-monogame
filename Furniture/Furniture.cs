using Util;

namespace FurnitureLibrary
{
    public abstract class Furniture
    {
        public abstract Triangle[] mesh { get; set; }
        public abstract Position position { get; set; }
    }
    public class FurnitureObject : Furniture
    {
        public override Triangle[] mesh { get; set; }
        public override Position position { get; set; }
        public FurnitureObject()
        {
            mesh = new Triangle[0];
            position = new Position();
        }
        public FurnitureObject(Triangle[] mesh, Position position)
        {
            this.mesh = mesh;
            this.position = position;
        }
    }

}
