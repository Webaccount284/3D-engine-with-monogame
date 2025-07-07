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

        // World
        World world = new World(new Vector2(8, 8));

        //Geometric info
        public VertexPositionTexture[] triangleVertices;
        Microsoft.Xna.Framework.Graphics.VertexBuffer vertexBuffer;
        int triangleCount = 1;
        Texture2D tileSheet;
        List<Vector2> loadedChunks = new List<Vector2>();
        List<VertexPositionTexture> chunkMeshes = new List<VertexPositionTexture>();
        bool reloadChunkMesh = true;
        Vector2 chunkLoc;
        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Setup Camera
            camAngle = new Vector2(0f, 0f); // angles of the camera
            camTarget = new Vector3(1f, 100f, 1f); // targeted point
            camPosition = new Vector3(100f, 1000f, 100f);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(80f), GraphicsDevice.DisplayMode.AspectRatio, 1f, 4000f);
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up); 
            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);

            
            // BasicEffect
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.Alpha = 1f;
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = tileSheet;

            // Want to see the colors of the vertices, this needs to be on
            basicEffect.LightingEnabled = false;

            // World
            chunkMeshes.AddRange(world.GetChunkMesh(new Vector2(0, 0), new Vector3(0, 0, 0)));
            chunkMeshes.AddRange(world.GetChunkMesh(new Vector2(1, 0), new Vector3(0, 0, 0)));
            loadedChunks.Add(new Vector2(0, 0)); 
            loadedChunks.Add(new Vector2(1, 0));
            // triangleVertices = LoadBlocks();
            vertexBuffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), triangleCount * 3, BufferUsage.WriteOnly);
            // vertexBuffer.SetData<VertexPositionTexture>(triangleVertices);
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
            camTarget = new Vector3((float)(camPosition.X + Math.Sin(camAngle.X * (Math.PI / 180))), (float)(camPosition.Y + Math.Sin(camAngle.Y * (Math.PI / 180))), (float)(camPosition.Z + Math.Cos(camAngle.X * (Math.PI / 180))));
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);

            triangleVertices = chunkMeshes.ToArray();
            triangleCount = chunkMeshes.Count / 3;

            Vector2[] chunks = CalculateLoadedChunks();
            
            reloadChunkMesh = true;
            
            if (reloadChunkMesh == true)
            {
                reloadChunkMesh = false;
                chunkMeshes.Clear();
                for (int i = 0; i < chunks.Length; i++)
                {
                    chunkMeshes.AddRange(world.GetChunkMesh(chunks[i], new Vector3(0, 0, 0)));
                }
                triangleVertices = chunkMeshes.ToArray();
                triangleCount = chunkMeshes.Count / 3;
                vertexBuffer.Dispose();
                vertexBuffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), triangleCount * 3, BufferUsage.WriteOnly);
                vertexBuffer.SetData<VertexPositionTexture>(triangleVertices);
            }
            base.Update(gameTime);
        }
        private Vector2[] CalculateLoadedChunks()
        {
            chunkLoc = new Vector2(camPosition.X, camPosition.Z);
            chunkLoc.X /= Block.BLOCKSIZE * Chunk.WIDTH;
            chunkLoc.Y /= Block.BLOCKSIZE * Chunk.DEPTH;
            chunkLoc = new Vector2((int)chunkLoc.X, (int)chunkLoc.Y);
            List<Vector2> chunks = new List<Vector2>();
            chunks.Add(chunkLoc);
            return chunks.ToArray();
            
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