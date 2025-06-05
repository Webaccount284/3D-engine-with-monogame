using Microsoft.Xna.Framework;
using System.Drawing.Text;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

public abstract class Object
{
    abstract public VertexPositionTexture[] GetMeshAsTriangles();
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
    public override VertexPositionTexture[] GetMeshAsTriangles()
    {
        VertexPositionTexture[] vertexPositionTextures = [
        new VertexPositionTexture(p1, v1),
        new VertexPositionTexture(p2, v2),
        new VertexPositionTexture(p3, v3)];
        return vertexPositionTextures;
    }
}
class Program
{
    static void Main(string[] args)
    {
        var game = new FormGame.Game();
        game.Run();
        game.Dispose();
    }
}
