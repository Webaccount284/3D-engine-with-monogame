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
public class Block : ShapeObject
{
    // change to only get 3 sides of mesh face to renderer
    public int id;
    public string name;
    public Mesh mesh;
    static float tcfWidth = 16; // textures across faces width
    static float tcfHeight = 256; // textures across faces height
    public const int BLOCKSIZE = 10;
    public Mesh[] faces = new Mesh[6];
    public Block(int id, string name, Mesh[] meshFaces)
    {
        this.id = id;
        this.name = name;
        this.faces = meshFaces;
    }

    public static Mesh[] CreateCubeMeshFaces(Vector2[] t)
    {
        if (t.Length != 6)
        {
            throw new ArgumentException("Wrong length of textures[]");
        }
        Mesh[] faces = [
            // left and right sides
                new Mesh([
                    new Triangle3D(new Vector3(0, 0, 0), new Vector3(0, 10, 10), new Vector3(0, 10, 0), new Vector2(t[0].X / tcfWidth, t[0].Y / tcfHeight + 1), new Vector2((t[0].X + 1) / tcfWidth, t[0].Y / tcfHeight), new Vector2(t[0].X / tcfWidth, t[0].Y / tcfHeight)),
                    new Triangle3D(new Vector3(0, 0, 0), new Vector3(0, 0, 10), new Vector3(0, 10, 10), new Vector2(t[0].X / tcfWidth, t[0].Y / tcfHeight + 1), new Vector2((t[0].X + 1) / tcfWidth, t[0].Y / tcfHeight + 1), new Vector2((t[0].X + 1) / tcfWidth, t[0].Y / tcfHeight))
                    ]),
                new Mesh([
                    new Triangle3D(new Vector3(-10, 0, 0), new Vector3(-10, 10, 0), new Vector3(-10, 10, 10), new Vector2(t[1].X/tcfWidth, t[1].Y / tcfHeight + 1), new Vector2(t[1].X/tcfWidth, t[1].Y / tcfHeight), new Vector2((t[1].X + 1)/tcfWidth, t[1].Y / tcfHeight)),
                    new Triangle3D(new Vector3(-10, 0, 0), new Vector3(-10, 10, 10), new Vector3(-10, 0, 10), new Vector2(t[1].X/tcfWidth, t[1].Y / tcfHeight + 1), new Vector2((t[1].X + 1)/tcfWidth, t[1].Y / tcfHeight), new Vector2((t[1].X + 1)/tcfWidth, t[1].Y / tcfHeight + 1))
                    ]),
            // back front
                new Mesh([
                    new Triangle3D(new Vector3(0, 0, 0), new Vector3(0, 10, 0), new Vector3(-10, 10, 0), new Vector2(t[2].X/tcfWidth, t[2].Y / tcfHeight + 1), new Vector2(t[2].X/tcfWidth, t[2].Y / tcfHeight), new Vector2((t[2].X + 1)/tcfWidth, t[2].Y / tcfHeight)),
                    new Triangle3D(new Vector3(0, 0, 0), new Vector3(-10, 10, 0), new Vector3(-10, 0, 0), new Vector2(t[2].X/tcfWidth, t[2].Y / tcfHeight + 1), new Vector2((t[2].X + 1)/tcfWidth, t[2].Y / tcfHeight), new Vector2((t[2].X + 1)/tcfWidth, t[2].Y / tcfHeight + 1))
                    ]),
                new Mesh([
                    new Triangle3D(new Vector3(0, 0, 10), new Vector3(-10, 0, 10), new Vector3(-10, 10, 10), new Vector2(t[3].X/tcfWidth, t[3].Y / tcfHeight + 1), new Vector2((t[3].X + 1)/tcfWidth, t[3].Y / tcfHeight + 1), new Vector2((t[3].X + 1)/tcfWidth, t[3].Y / tcfHeight)),
                    new Triangle3D(new Vector3(0, 0, 10), new Vector3(-10, 10, 10), new Vector3(0, 10, 10), new Vector2(t[3].X/tcfWidth, t[3].Y / tcfHeight + 1), new Vector2((t[3].X + 1)/tcfWidth, t[3].Y / tcfHeight), new Vector2(t[3].X/tcfWidth, t[3].Y / tcfHeight))
                    ]),
            // top bottom
                new Mesh([
                    new Triangle3D(new Vector3(0, 0, 0), new Vector3(-10, 0, 10), new Vector3(0, 0, 10), new Vector2(t[4].X/tcfWidth, t[4].Y / tcfHeight), new Vector2((t[4].X + 1)/tcfWidth, t[4].Y / tcfHeight + 1), new Vector2((t[4].X + 1)/tcfWidth, t[4].Y / tcfHeight)),
                    new Triangle3D(new Vector3(0, 0, 0), new Vector3(-10, 0, 0), new Vector3(-10, 0, 10), new Vector2(t[4].X/tcfWidth, t[4].Y / tcfHeight), new Vector2(t[4].X/tcfWidth, t[4].Y / tcfHeight + 1), new Vector2((t[4].X + 1)/tcfWidth, t[4].Y / tcfHeight + 1))
                    ]),
                new Mesh([
                    new Triangle3D(new Vector3(0, 10, 0), new Vector3(0, 10, 10), new Vector3(-10, 10, 10), new Vector2(t[5].X/tcfWidth, t[5].Y / tcfHeight), new Vector2((t[5].X + 1)/tcfWidth, t[5].Y / tcfHeight),new Vector2((t[5].X + 1)/tcfWidth, t[5].Y / tcfHeight + 1)),
                    new Triangle3D(new Vector3(0, 10, 0), new Vector3(-10, 10, 10), new Vector3(-10, 10, 0), new Vector2(t[5].X/tcfWidth, t[5].Y / tcfHeight), new Vector2((t[5].X + 1)/tcfWidth, t[5].Y / tcfHeight + 1), new Vector2(t[5].X/tcfWidth, t[5].Y / tcfHeight + 1))
                    ])];
        return faces;
    }
    public override VertexPositionTexture[] GetMeshAsTriangles(Vector3 transform)
    {
        if (id < 0)
        {
            return [];
        }
        else
        {
            return new Mesh([faces[0], faces[1], faces[2], faces[3], faces[4], faces[5]], [new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)]).GetMeshAsTriangles(transform);// repeat for all 6 faces
        }
    }
}
public class Chunk : ShapeObject
{
    public Vector2 position;
    public Block[,,] data;
    public const int WIDTH = 16, DEPTH = 16, HEIGHT = 256;

    static Block air = new Block(-1, "Air Block", Block.CreateCubeMeshFaces([new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0)]));
    static Block grassBlock = new Block(0, "Grass Block", Block.CreateCubeMeshFaces([new Vector2(3, 0), new Vector2(3, 0), new Vector2(3, 0), new Vector2(3, 0), new Vector2(2, 0), new Vector2(0, 0)]));
    static Block dirtBlock = new Block(1, "Dirt Block", Block.CreateCubeMeshFaces([new Vector2(2, 0), new Vector2(2, 0), new Vector2(2, 0), new Vector2(2, 0), new Vector2(2, 0), new Vector2(2, 0)]));
    static Block stoneBlock = new Block(2, "Stone Block", Block.CreateCubeMeshFaces([new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0)]));
    static Block woodPlank = new Block(3, "Wood Planks", Block.CreateCubeMeshFaces([new Vector2(4, 0), new Vector2(4, 0), new Vector2(4, 0), new Vector2(4, 0), new Vector2(4, 0), new Vector2(4, 0)]));
    static Block diamondOre = new Block(4, "Diamond Ore", Block.CreateCubeMeshFaces([new Vector2(2, 3), new Vector2(2, 3), new Vector2(2, 3), new Vector2(2, 3), new Vector2(2, 3), new Vector2(2, 3)]));
    static Block bedrock = new Block(5, "Bedrock", Block.CreateCubeMeshFaces([new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1)]));
    static Block leaves = new Block(6, "Leaves", Block.CreateCubeMeshFaces([new Vector2(5, 3), new Vector2(5, 3), new Vector2(5, 3), new Vector2(5, 3), new Vector2(5, 3), new Vector2(5, 3)]));
    public override VertexPositionTexture[] GetMeshAsTriangles(Vector3 transform)
    {
        List<VertexPositionTexture> mesh = new List<VertexPositionTexture>();
        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 0; j < DEPTH; j++)
            {
                for (int k = 0; k < HEIGHT; k++)
                {
                    if (data[i, j, k].id != -1)
                    {
                        if (k < HEIGHT - 1)
                        {
                            if (data[i, j, k + 1].id == -1)
                            {
                                mesh.AddRange(data[i, j, k].faces[5].GetMeshAsTriangles(transform + new Vector3(Block.BLOCKSIZE * i, Block.BLOCKSIZE * k, Block.BLOCKSIZE * j)));
                            }
                        }
                        else
                        {
                            mesh.AddRange(data[i, j, k].faces[5].GetMeshAsTriangles(transform + new Vector3(Block.BLOCKSIZE * i, Block.BLOCKSIZE * k, Block.BLOCKSIZE * j)));
                        }
                        if (k > 0)
                        {
                            if (data[i, j, k - 1].id == -1)
                            {
                                mesh.AddRange(data[i, j, k].faces[4].GetMeshAsTriangles(transform + new Vector3(Block.BLOCKSIZE * i, Block.BLOCKSIZE * k, Block.BLOCKSIZE * j)));
                            }
                        }
                        else
                        {
                            mesh.AddRange(data[i, j, k].faces[4].GetMeshAsTriangles(transform + new Vector3(Block.BLOCKSIZE * i, Block.BLOCKSIZE * k, Block.BLOCKSIZE * j)));
                        }
                        if (i < WIDTH - 1)
                        {
                            if (data[i + 1, j, k].id == -1)
                            {
                                mesh.AddRange(data[i, j, k].faces[0].GetMeshAsTriangles(transform + new Vector3(Block.BLOCKSIZE * i, Block.BLOCKSIZE * k, Block.BLOCKSIZE * j)));
                            }
                        }
                        else
                        {
                            mesh.AddRange(data[i, j, k].faces[0].GetMeshAsTriangles(transform + new Vector3(Block.BLOCKSIZE * i, Block.BLOCKSIZE * k, Block.BLOCKSIZE * j)));
                        }
                        if (i > 0)
                        {
                            if (data[i - 1, j, k].id == -1)
                            {
                                mesh.AddRange(data[i, j, k].faces[1].GetMeshAsTriangles(transform + new Vector3(Block.BLOCKSIZE * i, Block.BLOCKSIZE * k, Block.BLOCKSIZE * j)));
                            }
                        }
                        else
                        {
                            mesh.AddRange(data[i, j, k].faces[1].GetMeshAsTriangles(transform + new Vector3(Block.BLOCKSIZE * i, Block.BLOCKSIZE * k, Block.BLOCKSIZE * j)));
                        }
                        if (j < DEPTH - 1)
                        {
                            if (data[i, j + 1, k].id == -1)
                            {
                                mesh.AddRange(data[i, j, k].faces[3].GetMeshAsTriangles(transform + new Vector3(Block.BLOCKSIZE * i, Block.BLOCKSIZE * k, Block.BLOCKSIZE * j)));
                            }
                        }
                        else
                        {
                            mesh.AddRange(data[i, j, k].faces[3].GetMeshAsTriangles(transform + new Vector3(Block.BLOCKSIZE * i, Block.BLOCKSIZE * k, Block.BLOCKSIZE * j)));
                        }
                        if (j > 0)
                        {
                            if (data[i, j - 1, k].id == -1)
                            {
                                mesh.AddRange(data[i, j, k].faces[2].GetMeshAsTriangles(transform + new Vector3(Block.BLOCKSIZE * i, Block.BLOCKSIZE * k, Block.BLOCKSIZE * j)));
                            }
                        }
                        else
                        {
                            mesh.AddRange(data[i, j, k].faces[2].GetMeshAsTriangles(transform + new Vector3(Block.BLOCKSIZE * i, Block.BLOCKSIZE * k, Block.BLOCKSIZE * j)));
                        }
                    }
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
public class World : ShapeObject
{
    public Chunk[,] chunks;
    public Vector2 size = new Vector2(0, 0);
    int[,] worldHeight;
    public World(Vector2 size)
    {
        int sizeX = (int)((size.X + 2) * Block.BLOCKSIZE * Chunk.WIDTH);
        int sizeY = (int)((size.X + 2) * Block.BLOCKSIZE * Chunk.DEPTH);
        int[,] worldHeight = new int[sizeX, sizeY];
        int[,] worldHeight2 = new int[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Random random = new Random();
                worldHeight[x, y] = random.Next(0, 200);
            }
        }
        for (int i = 0; i < 40; i++)
        {
            for (int x = 1; x < sizeX - 1; x++)
            {
                for (int y = 1; y < sizeY - 1; y++)
                {
                    worldHeight2[x, y] = (worldHeight[x - 1, y] + worldHeight[x + 1, y] + worldHeight[x, y - 1] + worldHeight[x, y + 1] + worldHeight[x, y]) / 5;
                }
            }
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    worldHeight[x, y] = worldHeight2[x, y];
                }
            }
        }
        this.worldHeight = worldHeight;
        this.size = size;
        chunks = new Chunk[(int)size.X, (int)size.Y];
        /*for (int i = 0; i < size.X; i++)
        {
            for (int j = 0; j < size.Y; j++)
            {
                chunks[i, j] = new Chunk();
                chunks[i, j].CreateBlockArray(worldHeight, new Vector2(i + 1, j + 1));
            }
        }*/
    }
    public VertexPositionTexture[] GetChunkMesh(Vector2 chunk, Vector3 transform)
    {
        if (chunk.X < 0 || chunk.X > size.X)
        {
            throw new Exception("World size too small");
        }
        if (chunk.Y < 0 || chunk.Y > size.Y)
        {
            throw new Exception("World size too small");
        }
        if (chunks[(int) chunk.X, (int) chunk.Y] == null)
        {
            chunks[(int)chunk.X, (int)chunk.Y] = new Chunk();
            chunks[(int)chunk.X, (int)chunk.Y].CreateBlockArray(worldHeight, chunk + new Vector2(2, 2));
        }
        return chunks[(int)chunk.X, (int)chunk.Y].GetMeshAsTriangles(new Vector3(transform.X + chunk.X * Chunk.WIDTH * Block.BLOCKSIZE, transform.Y, transform.Z + chunk.Y * Chunk.DEPTH * Block.BLOCKSIZE));
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
public abstract class ShapeObject
{
    abstract public VertexPositionTexture[] GetMeshAsTriangles(Vector3 transform);
}
public class Mesh : ShapeObject
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
            for (int j = 0; j < meshes[i].triangles.Count(); j++)
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
        for (int i = 0; i < triangles.Length; i++)
        {
            VertexPositionTexture[] textureArray = triangles[i].GetMeshAsTriangles(transform);
            textures.Add(textureArray[0]);
            textures.Add(textureArray[1]);
            textures.Add(textureArray[2]);
        }
        return textures.ToArray();
    }
}
public class Triangle3D : ShapeObject
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
