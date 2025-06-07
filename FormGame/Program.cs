using Microsoft.Xna.Framework;
using System.Drawing.Text;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using SharpDX.Direct3D9;
using System.Linq;
using Microsoft.Xna.Framework.Audio;

public abstract class Object
{
    abstract public VertexPositionTexture[] GetMeshAsTriangles(Vector3 transform);
}
internal class Mesh : Object
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
internal class Triangle3D : Object
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
    static float tcf = 6f; // the number of texture squares on tile map

    static Mesh grassCube = CreateCube([0, 0, 0, 0, 1, 2]);
    static Mesh dirtCube = CreateCube([1, 1, 1, 1, 1, 1]);
    static Mesh stoneCube = CreateCube([3, 3, 3, 3, 3, 3]);
    static Mesh woodCube = CreateCube([4, 4, 4, 4, 4, 4]);
    static Mesh bedrock = CreateCube([5, 5, 5, 5, 5, 5]);
    static int chunkWidth = 16, chunkHeight = 128, chunkDepth = 16;
    /// <summary>
    /// Returns a new mesh with cube faces defined by you
    /// </summary>
    /// <param name="textures">A 6 item long array, in the order front back left right bottom top</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    static private Mesh CreateCube(int[] t)
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
    static private Mesh CreateBlockArray(int width, int height, int depth)
    {
        List<Mesh> chunk = new List<Mesh>();
        List<Vector3> transforms = new List<Vector3>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                Random rand = new Random();
                int bHeight = rand.Next(1, 5);
                int worldHeight = rand.Next(58, 63);
                for (int k = 0; k < height; k++)
                {
                    if (k < bHeight)
                    {
                        chunk.Add(bedrock);
                        transforms.Add(new Vector3(i * 10, k * 10, j * 10));
                    }
                    else if (k < worldHeight - 5)
                    {
                        chunk.Add(stoneCube);
                        transforms.Add(new Vector3(i * 10, k * 10, j * 10));
                    }
                    else if (k < worldHeight)
                    {
                        chunk.Add(dirtCube);
                        transforms.Add(new Vector3(i * 10, k * 10, j * 10));
                    }
                    else if (k == worldHeight)
                    {
                        chunk.Add(grassCube);
                        transforms.Add(new Vector3(i * 10, k * 10, j * 10));
                    }
                }
            }
        }
        return new Mesh(chunk.ToArray(), transforms.ToArray());
    }
    static void Main(string[] args)
    {

        // Mesh world = new Mesh([grassCube, dirtCube, woodCube, stoneCube, bedrock], [new Vector3(0, 0, 0), new Vector3(0, 0, 20), new Vector3(20, 0, 0), new Vector3(20, 0, 20), new Vector3(40, 0, 0)]);
        Mesh chunks = new Mesh([
            CreateBlockArray(chunkWidth, chunkHeight, chunkDepth),
            CreateBlockArray(chunkWidth, chunkHeight, chunkDepth),
            CreateBlockArray(chunkWidth, chunkHeight, chunkDepth),
            CreateBlockArray(chunkWidth, chunkHeight, chunkDepth), 
        ], [
            new Vector3(0, 0, 0),
            new Vector3(chunkWidth * 10 * 1, 0, 0),
            new Vector3(0, 0, chunkDepth * 10 * 1),
            new Vector3(chunkWidth * 10 * 1, 0, chunkDepth * 10 * 1),
        ]);
        var game = new FormGame.Game();

        game.triangleVertices = chunks.GetMeshAsTriangles(new Vector3(0, 0, 0));
        game.Run();
        game.Dispose();
    }
}
