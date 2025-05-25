using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Runtime.InteropServices.Marshalling;

namespace Util
{
    public class Position
    {
        Point3D position;
        Rotation rotation;
        public Position()
        {
            this.position = new Point3D();
            this.rotation = new Rotation();
        }
        public Position(Point3D position, Rotation rotation) 
        {
            this.position = position;
            this.rotation = rotation;
        }
    }
    public class Rotation
    {
        Vector3 rotation;
        public Rotation()
        {
            this.rotation = new Vector3();
        }
        public Rotation(Vector3 rotation)
        {
            this.rotation = rotation;
        }
        public Rotation(double x, double y, double z)
        {
            this.rotation = new Vector3(x, y, z);
        }
    }
    public class Point3D
    {
        public Vector3 position;
        public Point3D()
        {
            this.position = new Vector3();
        }
        public Point3D(Vector3 position) 
        {
            this.position = position;
        }
        public Point3D(double x, double y, double z)
        {
            this.position = new Vector3(x, y, z);
        }
    }
    public class Vector3
    {
        public double x, y, z;
        public Vector3()
        {
            x = 0;
            y = 0;
            z = 0;
        }
        public Vector3(double xp, double yp, double zp)
        {
            x = xp;
            y = yp;
            z = zp;
        }
    }
    public class Vector4
    {
        public double x, y, z, a;
        public Vector4()
        {
            x = 0;
            y = 0;
            z = 0;
            a = 0;
        }
        public Vector4(double xp, double yp, double zp, double ap)
        {
            x = xp;
            y = yp;
            z = zp;
            a = ap;
        }
    }
    public class Texture
    {
        public int width;
        public int height;
        public Vector4[] textureData;
        public Texture()
        {
            width = 1;
            height = 1;
            textureData = [new Vector4()];
        }
        public Texture(Vector4[] texture, int width, int height) 
        {
            this.width = width;
            this.height = height;
            this.textureData = texture;
        }
    }
    public class Triangle
    {
        public Point3D v1;
        public Point3D v2;
        public Point3D v3;
        public Texture texture;
        public Triangle() 
        {
            texture = new Texture();
            v1 = new Point3D(0, 0, 0);
            v2 = new Point3D(0, 0, 0); 
            v3 = new Point3D(0, 0, 0);
        }
        public Triangle(Point3D a, Point3D b, Point3D c, Texture t)
        {
            this.v1 = a;
            this.v2 = b;
            this.v3 = c;
            this.texture = t;
        }
    }
}
