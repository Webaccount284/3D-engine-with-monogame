using Microsoft.Xna.Framework;
using System.Drawing.Text;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using SharpDX.Direct3D9;
using System.Linq;

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
    static void Main(string[] args)
    {
        Mesh cube = new Mesh(
            [
                new Triangle3D(new Vector3(0, 0, 0), new Vector3(0, 10, 10), new Vector3(0, 10, 0), new Vector2(0, 1), new Vector2(1/4f, 0), new Vector2(0, 0)),
                new Triangle3D(new Vector3(0, 0, 0), new Vector3(0, 0, 10), new Vector3(0, 10, 10), new Vector2(0, 1), new Vector2(1/4f, 1), new Vector2(1/4f, 0)),
                new Triangle3D(new Vector3(0, 0, 0), new Vector3(-10, 10, 0), new Vector3(-10, 0, 0),  new Vector2(0, 1),  new Vector2(1/4f, 0), new Vector2(1/4f, 1)),
                new Triangle3D(new Vector3(0, 0, 0), new Vector3(0, 10, 0), new Vector3(-10, 10, 0),  new Vector2(0, 1), new Vector2(0, 0), new Vector2(1/4f, 0)),

                new Triangle3D(new Vector3(-10, 0, 0), new Vector3(-10, 10, 0), new Vector3(-10, 10, 10), new Vector2(0, 1), new Vector2(0, 0), new Vector2(1/4f, 0)),
                new Triangle3D(new Vector3(-10, 0, 0), new Vector3(-10, 10, 10), new Vector3(-10, 0, 10), new Vector2(0, 1), new Vector2(1/4f, 0), new Vector2(1/4f, 1)),
                new Triangle3D(new Vector3(0, 0, 10), new Vector3(-10, 0, 10), new Vector3(-10, 10, 10), new Vector2(0, 1), new Vector2(1/4f, 1), new Vector2(1/4f, 0)),
                new Triangle3D(new Vector3(0, 0, 10), new Vector3(-10, 10, 10), new Vector3(0, 10, 10), new Vector2(0, 1), new Vector2(1/4f, 0), new Vector2(0, 0)),

                new Triangle3D(new Vector3(0, 0, 0), new Vector3(-10, 0, 10), new Vector3(0, 0, 10), new Vector2(1/4f, 0), new Vector2(2/4f, 1), new Vector2(2/4f, 0)),
                new Triangle3D(new Vector3(0, 0, 0), new Vector3(-10, 0, 0), new Vector3(-10, 0, 10), new Vector2(1/4f, 0), new Vector2(1/4f, 1), new Vector2(2/4f, 1)),
                new Triangle3D(new Vector3(0, 10, 0), new Vector3(0, 10, 10), new Vector3(-10, 10, 10), new Vector2(2/4f, 0), new Vector2(3/4f, 0),new Vector2(3/4f, 1)),
                new Triangle3D(new Vector3(0, 10, 0), new Vector3(-10, 10, 10), new Vector3(-10, 10, 0), new Vector2(2/4f, 0), new Vector2(3/4f, 1), new Vector2(2/4f, 1))
            ]);
        Mesh cubes = new Mesh([cube, cube], [new Vector3(0, 0, 0), new Vector3(20, 0, 20)]);
        var game = new FormGame.Game();
        game.triangleVertices = cubes.GetMeshAsTriangles(new Vector3(0, 0, 0));
        game.Run();
        game.Dispose();
    }
}
