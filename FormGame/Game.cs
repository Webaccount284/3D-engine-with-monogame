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
    public class Block
    {
        public string name;
        public Face[] textures;
        public Block(string name, Face[] textures)
        {
            this.name = name;
            this.textures = textures;
        }
    }
    public class Face
    {
        public Vector2 topLeft;
        public Vector2 bottomRight;
        public Face(Vector2 topLeft, Vector2 bottomRight)
        {
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
        }
    }
    public class Chunk
    {
        public const int worldHeight = 128;
        public Block[,,] blocks = new Block[16, worldHeight, 16];
        public Chunk()
        {
            Color brown = Color.FromNonPremultiplied(97, 72, 19, 255);
            Face grassDirtFace = new Face(new Vector2(0 / 4f, 0), new Vector2(1 / 4f, 1));
            Face dirtFace = new Face(new Vector2(1 / 4f, 0), new Vector2(2 / 4f, 1));
            Face grassFace = new Face(new Vector2(2 / 4f, 0), new Vector2(3 / 4f, 1));
            Face stoneFace = new Face(new Vector2(3 / 4f, 0), new Vector2(4 / 4f, 1));
            Block grass = new Block("grass", [grassDirtFace, grassDirtFace, grassDirtFace, grassDirtFace, dirtFace, grassFace]);
            Block dirt = new Block("dirt", [dirtFace, dirtFace, dirtFace, dirtFace, dirtFace, dirtFace]);
            Block stone = new Block("stone", [stoneFace, stoneFace, stoneFace, stoneFace, stoneFace, stoneFace]);
            for (int i = 0; i < 16; i++)
            {
                for (int k = 0; k < 16; k++)
                {
                    Random rand = new Random();
                    int h = rand.Next(58, 61);
                    for (int j = 0; j < worldHeight; j++)
                    {
                        if (j < h - 5)
                        {
                            blocks[i, j, k] = stone;
                        }
                        else if (j < h)
                        {
                            blocks[i, j, k] = dirt;
                        }
                        else if (j == h)
                        {
                            blocks[i, j, k] = grass;
                        }

                    }
                }
            }
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
        VertexPositionTexture[] triangleVertices;
        Microsoft.Xna.Framework.Graphics.VertexBuffer vertexBuffer;
        int triangleCount;
        Texture2D tileSheet;

        //Map info
        const int chunkWidth = 1, chunkLength = 1;
        Chunk[,] chunks = new Chunk[chunkWidth, chunkLength];
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
            camPosition = new Microsoft.Xna.Framework.Vector3(0f, 16*60f, -100f);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(80f), GraphicsDevice.DisplayMode.AspectRatio, 1f, 4000f);
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, new Microsoft.Xna.Framework.Vector3(0f, 1f, 0f));// Y up
            worldMatrix = Matrix.CreateWorld(camTarget, Microsoft.Xna.Framework.Vector3.Forward, Microsoft.Xna.Framework.Vector3.Up);

            //Create blocks
            for (int i = 0; i < chunkWidth; i++)
            {
                for (int j = 0; j < chunkLength; j++)
                {
                    chunks[i, j] = new Chunk();
                }
            }


            //BasicEffect
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.Alpha = 1f;
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = tileSheet;

            // Want to see the colors of the vertices, this needs to be on
            basicEffect.LightingEnabled = false;

            //Geometry
            triangleVertices = LoadBlocks();
            triangleCount = triangleVertices.Length / 3;
            vertexBuffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), triangleCount * 3, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionTexture>(triangleVertices);
        }
        private VertexPositionTexture[] LoadBlocks()
        {
            List<VertexPositionTexture> u = new List<VertexPositionTexture>();
            for (int b = 0; b < chunkWidth; b++)
            {       for (int c = 0; c < chunkLength; c++)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        for (int j = 0; j < Chunk.worldHeight; j++)
                        {
                            for (int k = 0; k < 16; k++)
                            {
                                if (chunks[b,c] != null)
                                {
                                    if (chunks[b, c].blocks[i, j, k] != null)
                                    {
                                        VertexPositionTexture[] v = CreateBlock(new Vector3(b * (16 * 16) + i * 16, j * 16, c * (16 * 16) + k * 16), 16, chunks[b, c].blocks[i, j, k].textures);
                                        for (int l = 0; l < v.Length; l++)
                                        {
                                            u.Add(v[l]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return u.ToArray();
        }
        private VertexPositionTexture[] CreateBlock(Vector3 position, float size, Face[] textures)
        {
            float x = position.X;
            float y = position.Y;
            float z = position.Z;

            VertexPositionTexture[] u = new VertexPositionTexture[36];
            
            // front face
            u[0] = new VertexPositionTexture(new Vector3(x, y, z), textures[0].bottomRight); 
            u[1] = new VertexPositionTexture(new Vector3(x + size, y, z), new Vector2(textures[0].topLeft.X, textures[0].bottomRight.Y));
            u[2] = new VertexPositionTexture(new Vector3(x + size, y + size, z), textures[0].topLeft);
            u[3] = new VertexPositionTexture(new Vector3(x, y, z), textures[0].bottomRight);
            u[5] = new VertexPositionTexture(new Vector3(x, y + size, z), new Vector2(textures[0].bottomRight.X, textures[0].topLeft.Y));
            u[4] = new VertexPositionTexture(new Vector3(x + size, y + size, z), textures[0].topLeft);
            // back face
            u[6] = new VertexPositionTexture(new Vector3(x, y, z + size), textures[1].bottomRight);
            u[8] = new VertexPositionTexture(new Vector3(x + size, y, z + size), new Vector2(textures[1].topLeft.X, textures[1].bottomRight.Y));
            u[7] = new VertexPositionTexture(new Vector3(x + size, y + size, z + size), textures[0].topLeft);
            u[9] = new VertexPositionTexture(new Vector3(x, y, z + size), textures[1].bottomRight);
            u[10] = new VertexPositionTexture(new Vector3(x, y + size, z + size), new Vector2(textures[1].bottomRight.X, textures[1].topLeft.Y));
            u[11] = new VertexPositionTexture(new Vector3(x + size, y + size, z + size), textures[1].topLeft);
            // right face
            u[12] = new VertexPositionTexture(new Vector3(x, y, z), textures[2].bottomRight);
            u[14] = new VertexPositionTexture(new Vector3(x, y, z + size), new Vector2(textures[2].topLeft.X, textures[2].bottomRight.Y));
            u[13] = new VertexPositionTexture(new Vector3(x, y + size, z + size), textures[2].topLeft);
            u[15] = new VertexPositionTexture(new Vector3(x, y, z), textures[2].bottomRight);
            u[16] = new VertexPositionTexture(new Vector3(x, y + size, z), new Vector2(textures[2].bottomRight.X, textures[2].topLeft.Y));
            u[17] = new VertexPositionTexture(new Vector3(x, y + size, z + size), textures[2].topLeft);
            // left face
            u[18] = new VertexPositionTexture(new Vector3(x + size, y, z), textures[3].bottomRight);
            u[19] = new VertexPositionTexture(new Vector3(x + size, y, z + size), new Vector2(textures[3].topLeft.X, textures[3].bottomRight.Y));
            u[20] = new VertexPositionTexture(new Vector3(x + size, y + size, z + size), textures[3].topLeft);
            u[21] = new VertexPositionTexture(new Vector3(x + size, y, z), textures[3].bottomRight);
            u[23] = new VertexPositionTexture(new Vector3(x + size, y + size, z), new Vector2(textures[3].bottomRight.X, textures[3].topLeft.Y));
            u[22] = new VertexPositionTexture(new Vector3(x + size, y + size, z + size), textures[3].topLeft);
            // bottom face
            u[24] = new VertexPositionTexture(new Vector3(x, y, z), textures[4].topLeft);
            u[26] = new VertexPositionTexture(new Vector3(x + size, y, z), new Vector2(textures[4].topLeft.X, textures[4].bottomRight.Y));
            u[25] = new VertexPositionTexture(new Vector3(x + size, y, z + size), textures[4].bottomRight);
            u[27] = new VertexPositionTexture(new Vector3(x, y, z), textures[4].topLeft);
            u[28] = new VertexPositionTexture(new Vector3(x, y, z + size), new Vector2(textures[4].bottomRight.X, textures[4].topLeft.Y));
            u[29] = new VertexPositionTexture(new Vector3(x + size, y, z + size), textures[4].bottomRight);
            // top face
            u[30] = new VertexPositionTexture(new Vector3(x, y + size, z), textures[5].topLeft);
            u[31] = new VertexPositionTexture(new Vector3(x + size, y + size, z), new Vector2(textures[5].topLeft.X, textures[5].bottomRight.Y));
            u[32] = new VertexPositionTexture(new Vector3(x + size, y + size, z + size), textures[5].bottomRight);
            u[33] = new VertexPositionTexture(new Vector3(x, y + size, z), textures[5].topLeft);
            u[35] = new VertexPositionTexture(new Vector3(x, y + size, z + size), new Vector2(textures[5].bottomRight.X, textures[5].topLeft.Y));
            u[34] = new VertexPositionTexture(new Vector3(x + size, y + size, z + size), textures[5].bottomRight);

            return u;
        }
        protected override void LoadContent()
        {
            spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);
            tileSheet = Content.Load<Texture2D>("texturesheet");
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
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            //Turn off culling so we see both sides of our rendered triangle
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
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