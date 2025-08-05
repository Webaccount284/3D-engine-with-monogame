using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace FormGame
{
    /// <summary>
    /// Class for any game object defined by programmer
    /// </summary>
    internal class RenderData
    {
        public static int vertexCount = 0;
        public static bool dirty = true;
        public string id;
        public Vector3 position;
        public VertexPositionTexture[] data;
        public RenderData(string id, Vector3 position, VertexPositionTexture[] data)
        {
            this.id = id;
            this.position = position;
            this.data = data;
            vertexCount += data.Length;
        }
        public void ChangePosition(Vector3 transform)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i].Position += transform;
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

        // World
        World world = new World(new Vector2(32, 32));
        List<RenderData> renderData = new List<RenderData>();
        List<Vector3> loadedChunks = new List<Vector3>(); // these are vector3, not vector2. Vec3 have a y axis component which is obsolete. Can convert to Vec2 if needed

        //Geometric info
        public VertexPositionTexture[] triangleVertices;
        Microsoft.Xna.Framework.Graphics.VertexBuffer vertexBuffer;
        int triangleCount = 0;
        Texture2D tileSheet;

        bool reloadChunkMesh = true;
        Vector2 chunkLoc;
        int chunkRenderDistance = 5; 
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
            loadedChunks.Add(new Vector3(0, 0, 0));
            RenderData chunk = new RenderData("Chunk", new Vector3(0, 0, 0) /*the chunk position*/, world.GetChunkMesh(new Vector2(0, 0), new Vector3(0, 0, 0)));
            renderData.Add(chunk);
            vertexBuffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), (int)triangleCount * 3, BufferUsage.WriteOnly);
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
            Vector3 prevCamPos = camPosition;
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
            // Console.WriteLine("Uh yea it is " + camPosition.X);
            if (camPosition.X < 0 && prevCamPos.X >= 0)
            {
                camPosition = prevCamPos;
            }
            camTarget = new Vector3((float)(camPosition.X + Math.Sin(camAngle.X * (Math.PI / 180))), (float)(camPosition.Y + Math.Sin(camAngle.Y * (Math.PI / 180))), (float)(camPosition.Z + Math.Cos(camAngle.X * (Math.PI / 180))));
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);
            
            // chunk management 
            int chunkHash = GetListHashCode(loadedChunks); // uses hash code to compare lists
            Vector3 chunkPosition = new Vector3((float)Math.Floor(camPosition.X / (Block.BLOCKSIZE * Chunk.WIDTH)), 0, (float)Math.Floor(camPosition.Z / (Block.BLOCKSIZE * Chunk.DEPTH)));
            loadedChunks.Clear();
            for (int i = -chunkRenderDistance; i < chunkRenderDistance; i++) // gets which chunks should be loaded
            {
                for (int j = -chunkRenderDistance; j < chunkRenderDistance; j++)
                {
                    if (Math.Sqrt(i*i + j*j) < chunkRenderDistance * 2) // makes it circular
                    {
                        Vector3 chunk = new Vector3(i + chunkPosition.X, 0, j + chunkPosition.Z);
                        if (chunk.X > 0 && chunk.Z > 0)
                        {
                            loadedChunks.Add(chunk);
                        }
                    }
                }
            }
            if (chunkHash != GetListHashCode(loadedChunks)) // data changed, reload chunks
            {
                RenderData.dirty = true;

                List<Vector3> loadedIntoSheet = new List<Vector3>();

                for (int i = 0; i < renderData.Count; i++)
                {
                    if (renderData[i].id == "Chunk")
                    {
                        if (!loadedChunks.Contains(renderData[i].position)) // delete this chunk
                        {
                            renderData.RemoveAt(i);
                        }
                        else 
                        {
                            loadedIntoSheet.Add(renderData[i].position);
                        }
                    }
                }
                for (int i = 0; i < loadedChunks.Count; i++)
                {
                    if (!loadedIntoSheet.Contains(loadedChunks[i]))
                    {
                        renderData.Add(new RenderData("Chunk", loadedChunks[i], world.GetChunkMesh(new Vector2(loadedChunks[i].X, loadedChunks[i].Z), new Vector3(0, 0, 0))));     
                    }
                }
            }
            if (RenderData.dirty == true) // checks if the data is dirty
            {
                RenderData.dirty = false;
                List<VertexPositionTexture> worldMesh = new List<VertexPositionTexture>();
                for (int i = 0; i < renderData.Count; i++)
                {
                    worldMesh.AddRange(renderData[i].data);
                }
                triangleCount = worldMesh.Count;
                triangleVertices = new VertexPositionTexture[triangleCount];
                triangleVertices = worldMesh.ToArray();
                vertexBuffer.Dispose();
                vertexBuffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), triangleCount * 3, BufferUsage.WriteOnly);
                vertexBuffer.SetData<VertexPositionTexture>(triangleVertices);
            }

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
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCount / 3);
            }
            base.Draw(gameTime);
        }
        static int GetListHashCode(List<Vector3> list)
        {
            unchecked
            {
                int hash = 19;
                foreach (Vector3 item in list)
                {
                    hash = hash * 31 + item.GetHashCode();
                }
                return hash;
            }
        }
    }
}