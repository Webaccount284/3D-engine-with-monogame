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
public class Block : Object
{
    public int id;
    public string name;
    public Mesh mesh;
    static float tcf = 8;
    public const int BLOCKSIZE = 10;
    public Block(int id, string name, Mesh mesh)
    {
        this.id = id;
        this.name = name;
        this.mesh = mesh;
    }

    public static Mesh CreateCubeMesh(int[] t)
    {
        if (t.Length != 6)
        {
            throw new ArgumentException("Wrong length of textures[]");
        }
        Mesh cube = new Mesh(
            [
                new Triangle3D(new Vector3(0, 0, 0), new Vector3(0, 10, 10), new Vector3(0, 10, 0), new Vector2(t[0]/tcf, 1), new Vector2((t[0] + 1)/tcf, 0), new Vector2(t[0]/tcf, 0)),
                new Triangle3D(new Vector3(0, 0, 0), new Vector3(0, 0, 10), new Vector3(0, 10, 10), new Vector2(t[0]/tcf, 1), new Vector2((t[0] + 1)/tcf, 1), new Vector2((t[0] + 1)/tcf, 0)),
                new Triangle3D(new Vector3(0, 0, 0), new Vector3(-10, 10, 0), new Vector3(-10, 0, 0),  new Vector2(t[1]/tcf, 1),  new Vector2((t[1] + 1)/tcf, 0), new Vector2((t[1] + 1)/tcf, 1)),
                new Triangle3D(new Vector3(0, 0, 0), new Vector3(0, 10, 0), new Vector3(-10, 10, 0),  new Vector2(t[1]/tcf, 1), new Vector2(t[1]/tcf, 0), new Vector2((t[1] + 1)/tcf, 0)),

                new Triangle3D(new Vector3(-10, 0, 0), new Vector3(-10, 10, 0), new Vector3(-10, 10, 10), new Vector2(t[2]/tcf, 1), new Vector2(t[2]/tcf, 0), new Vector2((t[2] + 1)/tcf, 0)),
                new Triangle3D(new Vector3(-10, 0, 0), new Vector3(-10, 10, 10), new Vector3(-10, 0, 10), new Vector2(t[2]/tcf, 1), new Vector2((t[2] + 1)/tcf, 0), new Vector2((t[2] + 1)/tcf, 1)),
                new Triangle3D(new Vector3(0, 0, 10), new Vector3(-10, 0, 10), new Vector3(-10, 10, 10), new Vector2(t[3]/tcf, 1), new Vector2((t[3] + 1)/tcf, 1), new Vector2((t[3] + 1)/tcf, 0)),
                new Triangle3D(new Vector3(0, 0, 10), new Vector3(-10, 10, 10), new Vector3(0, 10, 10), new Vector2(t[3]/tcf, 1), new Vector2((t[3] + 1)/tcf, 0), new Vector2(t[3]/tcf, 0)),

                new Triangle3D(new Vector3(0, 0, 0), new Vector3(-10, 0, 10), new Vector3(0, 0, 10), new Vector2(t[4]/tcf, 0), new Vector2((t[4] + 1)/tcf, 1), new Vector2((t[4] + 1)/tcf, 0)),
                new Triangle3D(new Vector3(0, 0, 0), new Vector3(-10, 0, 0), new Vector3(-10, 0, 10), new Vector2(t[4]/tcf, 0), new Vector2(t[4]/tcf, 1), new Vector2((t[4] + 1)/tcf, 1)),
                new Triangle3D(new Vector3(0, 10, 0), new Vector3(0, 10, 10), new Vector3(-10, 10, 10), new Vector2(t[5]/tcf, 0), new Vector2((t[5] + 1)/tcf, 0),new Vector2((t[5] + 1)/tcf, 1)),
                new Triangle3D(new Vector3(0, 10, 0), new Vector3(-10, 10, 10), new Vector3(-10, 10, 0), new Vector2(t[5]/tcf, 0), new Vector2((t[5] + 1)/tcf, 1), new Vector2(t[5]/tcf, 1))
            ]);
        return cube;
    }
    public override VertexPositionTexture[] GetMeshAsTriangles(Vector3 transform)
    {
        if (id < 0)
        {
            return [];
        }
        else
        {
            return mesh.GetMeshAsTriangles(transform);
        }
    }
}
public class Chunk : Object
{
    public Vector2 position;
    public Block[,,] data;
    public const int WIDTH = 16, DEPTH = 16, HEIGHT = 256;

    static Block air = new Block(-1, "Air Block", Block.CreateCubeMesh([0, 0, 0, 0, 0, 0]));
    static Block grassBlock = new Block(0, "Grass Block", Block.CreateCubeMesh([0, 0, 0, 0, 1, 2]));
    static Block dirtBlock = new Block(1, "Dirt Block", Block.CreateCubeMesh([1, 1, 1, 1, 1, 1]));
    static Block stoneBlock = new Block(2, "Stone Block", Block.CreateCubeMesh([3, 3, 3, 3, 3, 3]));
    static Block woodPlank = new Block(3, "Wood Planks", Block.CreateCubeMesh([4, 4, 4, 4, 4, 4]));
    static Block diamondOre = new Block(4, "Diamond Ore", Block.CreateCubeMesh([6, 6, 6, 6, 6, 6]));
    static Block bedrock = new Block(5, "Bedrock", Block.CreateCubeMesh([5, 5, 5, 5, 5, 5]));
    static Block leaves = new Block(6, "Leaves", Block.CreateCubeMesh([7, 7, 7, 7, 7, 7]));
    public override VertexPositionTexture[] GetMeshAsTriangles(Vector3 transform)
    {
        List<VertexPositionTexture> mesh = new List<VertexPositionTexture>();
        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 0; j < DEPTH; j++)
            {
                for (int k = 0; k < HEIGHT; k++)
                {
                    mesh.AddRange(data[i, j, k].GetMeshAsTriangles(transform + new Vector3(Block.BLOCKSIZE * i, Block.BLOCKSIZE * k, Block.BLOCKSIZE * j)));
                }
            }
        }
        return mesh.ToArray();
    }
    public void CreateBlockArray(int[,] heights, Vector2 vec)
    {
        Block[,,] chunk = new Block[WIDTH, DEPTH, HEIGHT];
        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 0; j < DEPTH; j++)
            {
                Random rand = new Random();
                int bHeight = rand.Next(1, 5);
                int worldHeight = heights[(int)(i + vec.X * DEPTH), (int)(j + vec.Y * DEPTH)];
                for (int k = 0; k < HEIGHT; k++)
                {
                    if (k < bHeight)
                    {
                        chunk[i, j, k] = bedrock;
                    }
                    else if (k < worldHeight - 5)
                    {
                        if (rand.Next(0, 100) == 0)
                        {
                            chunk[i, j, k] = diamondOre;
                        }
                        else
                        {
                            chunk[i, j, k] = stoneBlock;
                        }
                    }
                    else if (k < worldHeight)
                    {
                        chunk[i, j, k] = dirtBlock;
                    }
                    else if (k == worldHeight)
                    {
                        chunk[i, j, k] = grassBlock;
                    }
                    else if (k == worldHeight + 1)
                    {
                        if (rand.Next(0, 100) == 5)
                        {
                            chunk[i, j, k] = leaves;
                        }
                        else
                        {
                            chunk[i, j, k] = air;
                        }
                    }
                    else
                    {
                        chunk[i, j, k] = air;
                    }
                }
            }
        }
        data = chunk;
    }
}
public class World : Object
{
    public Chunk[,] chunks;
    public Vector2 size = new Vector2(0, 0);
    public World(Vector2 size)
    {
        // replace fixed 256 values with actual size
        int[,] worldHeight = new int[256, 256];
        int[,] worldHeight2 = new int[256, 256];
        for (int x = 0; x < 256; x++)
        {
            for (int y = 0; y < 256; y++)
            {
                Random random = new Random();
                worldHeight[x, y] = random.Next(0, 200);
            }
        }
        for (int i = 0; i < 40; i++)
        {
            for (int x = 1; x < 255; x++)
            {
                for (int y = 1; y < 255; y++)
                {
                    worldHeight2[x, y] = (worldHeight[x - 1, y] + worldHeight[x + 1, y] + worldHeight[x, y - 1] + worldHeight[x, y + 1] + worldHeight[x, y]) / 5;
                }
            }
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    worldHeight[x, y] = worldHeight2[x, y];
                }
            }

        }
        this.size = size;
        chunks = new Chunk[(int)size.X, (int)size.Y];
        for (int i = 0; i < size.X; i++)
        {
            for (int j = 0; j < size.Y; j++)
            {
                chunks[i, j] = new Chunk();
                chunks[i, j].CreateBlockArray(worldHeight, new Vector2(i + 1, j + 1));
            }
        }
    }
    public override VertexPositionTexture[] GetMeshAsTriangles(Vector3 transform)
    {
        List<VertexPositionTexture> mesh = new List<VertexPositionTexture>();
        for (int i = 0; i < size.X; i++)
        {
            for (int j = 0; j < size.Y; j++)
            {
                mesh.AddRange(chunks[i, j].GetMeshAsTriangles(transform + new Vector3(i * Block.BLOCKSIZE * Chunk.WIDTH, 0, j * Block.BLOCKSIZE * Chunk.WIDTH)));
            }
        }
        return mesh.ToArray();
    }
}
public abstract class Object
{
    abstract public VertexPositionTexture[] GetMeshAsTriangles(Vector3 transform);
}
public class Mesh : Object
{
    public Triangle3D[] triangles;
    public Mesh(Triangle3D[] triangles)
    {
        this.triangles = triangles;
    }
    public Mesh(Mesh[] meshes, Vector3[] positions)
    {
        List<Triangle3D> tris = new List<Triangle3D>();
        for (int i = 0; i < meshes.Length; i++)
        {
            Triangle3D[] newTris = new Triangle3D[meshes[i].triangles.Length];
            for (int j = 0; j < meshes[i].triangles.Count();  j++)
            {
                newTris[j] = meshes[i].triangles[j].ApplyTransform(positions[i]);
            }
            tris.AddRange(newTris);
        }
        this.triangles = tris.ToArray();
    }
    public override VertexPositionTexture[] GetMeshAsTriangles(Vector3 transform)
    {
        List<VertexPositionTexture> textures = new List<VertexPositionTexture>();
        for (int i = 0; i <  triangles.Length; i++) 
        {
            VertexPositionTexture[] textureArray = triangles[i].GetMeshAsTriangles(transform);
            textures.Add(textureArray[0]);
            textures.Add(textureArray[1]);
            textures.Add(textureArray[2]);
        }
        return textures.ToArray();
    }
}
public class Triangle3D : Object
{
    public Vector3 p1 { get; set; }
    public Vector3 p2 { get; set; }
    public Vector3 p3 { get; set; }
    public Vector2 v1 { get; set; }
    public Vector2 v2 { get; set; }
    public Vector2 v3 { get; set; }
    public Triangle3D(Vector3 p1, Vector3 p2, Vector3 p3, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
    }
    public override VertexPositionTexture[] GetMeshAsTriangles(Vector3 transform)
    {
        VertexPositionTexture[] vertexPositionTextures = [
        new VertexPositionTexture(p1 + transform, v1),
        new VertexPositionTexture(p2 + transform, v2),
        new VertexPositionTexture(p3 + transform, v3)];
        return vertexPositionTextures;
    }
    public Triangle3D ApplyTransform(Vector3 transform)
    {
        return new Triangle3D(p1 + transform, p2 + transform, p3 + transform, v1, v2, v3);
    }
}
class Program
{

    static int chunkWidth = 16, chunkHeight = 128, chunkDepth = 16;
    /// <summary>
    /// Returns a new mesh with cube faces defined by you
    /// </summary>
    /// <param name="textures">A 6 item long array, in the order front back left right bottom top</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    static void Main(string[] args)
    {
        // Mesh world = new Mesh([grassCube, dirtCube, woodCube, stoneCube, bedrock], [new Vector3(0, 0, 0), new Vector3(0, 0, 20), new Vector3(20, 0, 0), new Vector3(20, 0, 20), new Vector3(40, 0, 0)]);
        World world = new World(new Vector2(5, 5));

        var game = new FormGame.Game();
        game.triangleVertices = world.GetMeshAsTriangles(new Vector3(0, 0, 0));
        game.Run();
        game.Dispose();
    }
}
