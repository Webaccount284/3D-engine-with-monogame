using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace FormGame
{
    internal class Block
    {
        public string name;
        public Color[] colors;
        public Block(string name, Color[] colors)
        {
            this.name = name;
            this.colors = colors;
        }
    }
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch;

        //Camera
        Microsoft.Xna.Framework.Vector2 camAngle;
        Microsoft.Xna.Framework.Vector3 camTarget;
        Microsoft.Xna.Framework.Vector3 camPosition;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;

        //BasicEffect for rendering
        BasicEffect basicEffect;

        //Geometric info
        VertexPositionColor[] triangleVertices;
        Microsoft.Xna.Framework.Graphics.VertexBuffer vertexBuffer;
        int triangleCount;

        //Map info
        Block[,,] blocks;
        int width = 32, height = 16, length = 32;
        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            //Setup Camera
            camAngle = new Microsoft.Xna.Framework.Vector2(0f, 0f); // angles of the camera
            camTarget = new Microsoft.Xna.Framework.Vector3(0f, 0f, 0f); // targeted point
            camPosition = new Microsoft.Xna.Framework.Vector3(0f, 0f, -100f);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(80f), GraphicsDevice.DisplayMode.AspectRatio, 1f, 4000f);
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, new Microsoft.Xna.Framework.Vector3(0f, 1f, 0f));// Y up
            worldMatrix = Matrix.CreateWorld(camTarget, Microsoft.Xna.Framework.Vector3.Forward, Microsoft.Xna.Framework.Vector3.Up);

            //Create blocks
            Color brown = Color.FromNonPremultiplied(97, 72, 19, 255);
            Block grass = new Block("grass", [brown, brown, brown, brown, brown, Color.Green]);
            Block stone = new Block("stone", [Color.Gray, Color.Gray, Color.Gray, Color.Gray, Color.Gray, Color.Gray]);
            blocks = new Block[width, height, length];
            blocks[0, 0, 0] = stone;
            blocks[0, 1, 0] = stone;
            blocks[0, 2, 0] = grass;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    blocks[i, 0, j] = stone;
                    blocks[i, 1, j] = stone;
                    blocks[i, 2, j] = stone;
                    Random rand = new Random();
                    int r = rand.Next(0, 3);
                    if (r == 0)
                    {
                        blocks[i, 3, j] = grass;
                    }
                    else if (r==1)
                    {
                        blocks[i, 3, j] = stone;
                        blocks[i, 4, j] = grass;
                    }
                    else if (r == 2)
                    {
                        blocks[i, 3, j] = stone;
                        blocks[i, 4, j] = stone;
                        blocks[i, 5, j] = grass;
                    }
                    else if (r == 3)
                    {
                        blocks[i, 3, j] = stone;
                        blocks[i, 4, j] = stone;
                        blocks[i, 5, j] = stone;
                        blocks[i, 6, j] = grass;
                    }
                }
            }

            //BasicEffect
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.Alpha = 1f;

            // Want to see the colors of the vertices, this needs to be on
            basicEffect.VertexColorEnabled = true;

            //Lighting requires normal information which VertexPositionColor does not have
            //If you want to use lighting and VPC you need to create a custom def
            basicEffect.LightingEnabled = false;

            //Geometry
            triangleVertices = LoadBlocks();
            triangleCount = triangleVertices.Length / 3;
            vertexBuffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), triangleCount * 3, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(triangleVertices);
        }
        private VertexPositionColor[] LoadBlocks()
        {
            List<VertexPositionColor> u = new List<VertexPositionColor>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < length; k++)
                    {
                        if (blocks[i, j, k] != null)
                        {
                            VertexPositionColor[] v = CreateBlock(new Vector3(i * 16, j * 16, k * 16), 16, blocks[i, j, k].colors);
                            for (int l = 0; l < v.Length; l++)
                            {
                                u.Add(v[l]);
                            }
                        }
                    }
                }
            }
            return u.ToArray();
        }
        private VertexPositionColor[] CreateBlock(Vector3 position, float size, Color[] colors)
        {
            float x = position.X;
            float y = position.Y;
            float z = position.Z;

            VertexPositionColor[] u = new VertexPositionColor[36];

            // front face
            u[0] = new VertexPositionColor(new Vector3(x, y, z), colors[0]); 
            u[1] = new VertexPositionColor(new Vector3(x + size, y, z), colors[0]);
            u[2] = new VertexPositionColor(new Vector3(x + size, y + size, z), colors[0]);
            u[3] = new VertexPositionColor(new Vector3(x, y, z), colors[0]);
            u[4] = new VertexPositionColor(new Vector3(x, y + size, z), colors[0]);
            u[5] = new VertexPositionColor(new Vector3(x + size, y + size, z), colors[0]);
            // back face
            u[6] = new VertexPositionColor(new Vector3(x, y, z + size), colors[1]);
            u[7] = new VertexPositionColor(new Vector3(x + size, y, z + size), colors[1]);
            u[8] = new VertexPositionColor(new Vector3(x + size, y + size, z + size), colors[1]);
            u[9] = new VertexPositionColor(new Vector3(x, y, z + size), colors[1]);
            u[10] = new VertexPositionColor(new Vector3(x, y + size, z + size), colors[1]);
            u[11] = new VertexPositionColor(new Vector3(x + size, y + size, z + size), colors[1]);
            // right face
            u[12] = new VertexPositionColor(new Vector3(x, y, z), colors[2]);
            u[13] = new VertexPositionColor(new Vector3(x, y, z + size), colors[2]);
            u[14] = new VertexPositionColor(new Vector3(x, y + size, z + size), colors[2]);
            u[15] = new VertexPositionColor(new Vector3(x, y, z), colors[2]);
            u[16] = new VertexPositionColor(new Vector3(x, y + size, z), colors[2]);
            u[17] = new VertexPositionColor(new Vector3(x, y + size, z + size), colors[2]);
            // left face
            u[18] = new VertexPositionColor(new Vector3(x + size, y, z), colors[3]);
            u[19] = new VertexPositionColor(new Vector3(x + size, y, z + size), colors[3]);
            u[20] = new VertexPositionColor(new Vector3(x + size, y + size, z + size), colors[3]);
            u[21] = new VertexPositionColor(new Vector3(x + size, y, z), colors[3]);
            u[22] = new VertexPositionColor(new Vector3(x + size, y + size, z), colors[3]);
            u[23] = new VertexPositionColor(new Vector3(x + size, y + size, z + size), colors[3]);
            // bottom face
            u[24] = new VertexPositionColor(new Vector3(x, y, z), colors[4]);
            u[25] = new VertexPositionColor(new Vector3(x + size, y, z), colors[4]);
            u[26] = new VertexPositionColor(new Vector3(x + size, y, z + size), colors[4]);
            u[27] = new VertexPositionColor(new Vector3(x, y, z), colors[4]);
            u[28] = new VertexPositionColor(new Vector3(x, y, z + size), colors[4]);
            u[29] = new VertexPositionColor(new Vector3(x + size, y, z + size), colors[4]);
            // top face
            u[30] = new VertexPositionColor(new Vector3(x, y + size, z), colors[5]);
            u[31] = new VertexPositionColor(new Vector3(x + size, y + size, z), colors[5]);
            u[32] = new VertexPositionColor(new Vector3(x + size, y + size, z + size), colors[5]);
            u[33] = new VertexPositionColor(new Vector3(x, y + size, z), colors[5]);
            u[34] = new VertexPositionColor(new Vector3(x, y + size, z + size), colors[5]);
            u[35] = new VertexPositionColor(new Vector3(x + size, y + size, z + size), colors[5]);

            return u;
        }
        protected override void LoadContent()
        {
            spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice); 
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) { Exit(); }
            float s = 1f;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                camPosition.Z += s * (float)Math.Sin(camAngle.X * (Math.PI / 180f));
                camPosition.X -= s * (float)Math.Cos(camAngle.X * (Math.PI / 180f));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                camPosition.Z -= s * (float)Math.Sin(camAngle.X * (Math.PI / 180f));
                camPosition.X += s * (float)Math.Cos(camAngle.X * (Math.PI / 180f));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                camPosition.Y -= 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                camPosition.Y += 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                camPosition.X += s * (float)Math.Sin(camAngle.X * (Math.PI / 180f));
                camPosition.Z += s * (float)Math.Cos(camAngle.X * (Math.PI / 180f));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                camPosition.X -= s * (float)Math.Sin(camAngle.X * (Math.PI / 180f));
                camPosition.Z -= s * (float)Math.Cos(camAngle.X * (Math.PI / 180f));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                camAngle.X--;
                if (camAngle.X > 360)
                {
                    camAngle.X -= 360;
                }
                else if (camAngle.X < 0)
                {
                    camAngle.X += 360;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                camAngle.X++;
                if (camAngle.X > 360)
                {
                    camAngle.X -= 360;
                }
                else if (camAngle.X < 0)
                {
                    camAngle.X += 360;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                camAngle.Y++;
                if (camAngle.Y > 90)
                {
                    camAngle.Y = 90;
                }
                else if (camAngle.Y < -90)
                {
                    camAngle.Y = -90;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                camAngle.Y--;
                if (camAngle.Y > 90)
                {
                    camAngle.Y = 90;
                }
                else if (camAngle.Y < -90)
                {
                    camAngle.Y = -90;
                }
            }
            camTarget = new Microsoft.Xna.Framework.Vector3((float)(camPosition.X + Math.Sin(camAngle.X * (Math.PI / 180))), (float)(camPosition.Y + Math.Sin(camAngle.Y * (Math.PI / 180))), (float)(camPosition.Z + Math.Cos(camAngle.X * (Math.PI / 180))));
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Microsoft.Xna.Framework.Vector3.Up);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            basicEffect.Projection = projectionMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.World = worldMatrix;

            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            //Turn off culling so we see both sides of our rendered triangle
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCount * 3);
            }

            base.Draw(gameTime);
        }
    }
}