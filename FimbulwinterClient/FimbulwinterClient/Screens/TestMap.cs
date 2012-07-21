using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FimbulwinterClient.Content;
using Microsoft.Xna.Framework.Input;
using Extensions;

namespace FimbulwinterClient.Screens
{
    public struct VertexPositionNormalColor
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;
        public Vector2 TexturePosition;

        public VertexPositionNormalColor(Vector3 position, Color color, Vector3 normal)
        {
            Position = position;
            Color = color;
            Normal = normal;
            TexturePosition = new Vector2(0.0f, 0.0f);
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
             (
                  new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                  new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                  new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                  new VertexElement(sizeof(float) * 7, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
             );
    }

    class TestMap : IGameScreen
    {
        VertexBuffer vertexBuffer;
        Dictionary<Texture2D, VertexBuffer> vertexBuffers;
        Texture2D streetTexture;

        Effect effect;
        Matrix viewMatrix;
        Matrix projectionMatrix;

        Vector3 cameraPosition = new Vector3(6, 6, 1200);
        float leftrightRot = MathHelper.PiOver2;
        float updownRot = -MathHelper.Pi / 10.0f;
        const float rotationSpeed = 0.3f;
        const float moveSpeed = 150.0f;
        MouseState originalMouseState;

        public TestMap()
        {
            Mouse.SetPosition(ROClient.Singleton.GraphicsDevice.Viewport.Width / 2, ROClient.Singleton.GraphicsDevice.Viewport.Height / 2);
            originalMouseState = Mouse.GetState();

            effect = ROClient.Singleton.Content.Load<Effect>("Effect2");
            streetTexture = ROClient.Singleton.ContentManager.LoadContent<Texture2D>("data\\texture\\gld2\\gld2_ald_bottom02.bmp");
            SetUpVertices();
        }

        int version;
        int width;
        int height;
        List<Tuple<String, String>> textures;
        List<List<Cube>> cubes;
        List<Tile> tiles;
        private void SetUpVertices()
        {
            System.IO.Stream f = ROClient.Singleton.ContentManager.LoadContent<System.IO.Stream>("data\\lighthalzen.gnd");
            using (System.IO.BinaryReader br = new System.IO.BinaryReader(f))
            {
                var header = br.ReadBytes(4);
                version = br.ReadInt16();

                width = br.ReadInt32();
                height = br.ReadInt32();
                Single scale = br.ReadSingle();
                int nTextures = br.ReadInt32();
                int temp = br.ReadInt32();

                textures = new List<Tuple<String, String>>();
                for (int i = 0; i < nTextures; i++)
                {
                    textures.Add(new Tuple<String, String>(br.ReadCString(40), br.ReadCString(40)));

                }

                int nLightmaps = br.ReadInt32();
                int lightmapWidth = br.ReadInt32();
                int lightmapHeight = br.ReadInt32();
                int gridSizeCell = br.ReadInt32();

                List<Byte[]> lightmaps = new List<Byte[]>();
                for (int i = 0; i < nLightmaps; i++)
                    lightmaps.Add(br.ReadBytes(256));

                int nTiles = br.ReadInt32();
                tiles = new List<Tile>();
                for (int i = 0; i < nTiles; i++)
                    tiles.Add(new Tile(br));


                cubes = new List<List<Cube>>();
                int y;
                int x;
                for (y = 0; y < height; y++)
                {
                    var t = new List<Cube>();
                    cubes.Add(t);
                    for (x = 0; x < width; x++)
                    {
                        t.Add(new Cube(br));
                    }
                }

                calcVertexNormals();
            }

            Dictionary<int, Texture2D> textureCache = new Dictionary<int, Texture2D>();
            Dictionary<Texture2D, List<VertexPositionNormalTexture>> vertices = new Dictionary<Texture2D, List<VertexPositionNormalTexture>>();
            List<VertexPositionNormalColor> vertices_notexture = new List<VertexPositionNormalColor>();
            cameraPosition = new Vector3(6, 6, width * 10);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cube c = cubes[y][x];

                    if (c.tileUp > -1 && c.tileUp < (int)tiles.Count())
                    {
                        Tile t = tiles[c.tileUp];
                        if (t.texture >= (int)textures.Count())
                            t.texture = 0;
                        if (t.texture < 0)
                            t.texture = 0;

                        Texture2D texture = textureCache.FirstOrDefault(txt => txt.Key == t.texture).Value;
                        if (texture == null)
                        {
                            texture = ROClient.Singleton.ContentManager.LoadContent<Texture2D>("data\\texture\\" + textures[t.texture].Item1.Korean());
                            textureCache.Add(t.texture, texture);
                        }

                        if (!vertices.ContainsKey(texture))
                            vertices.Add(texture, new List<VertexPositionNormalTexture>());

                        vertices[texture].Add(new VertexPositionNormalTexture(new Vector3(x * 10, -c.cell1, (height - y) * 10), c.vNormal1, new Vector2(t.u1, 1 - t.v1)));
                        vertices[texture].Add(new VertexPositionNormalTexture(new Vector3(x * 10, -c.cell3, (height - y) * 10 - 10), c.vNormal2, new Vector2(t.u3, 3 - t.v3)));
                        vertices[texture].Add(new VertexPositionNormalTexture(new Vector3(x * 10 + 10, -c.cell2, (height - y) * 10), c.vNormal3, new Vector2(t.u2, 1 - t.v2)));
                        vertices[texture].Add(new VertexPositionNormalTexture(new Vector3(x * 10 + 10, -c.cell4, (height - y) * 10 - 10), c.vNormal4, new Vector2(t.u4, 1 - t.v4)));
                    }
                    else
                    {
                        vertices_notexture.Add(new VertexPositionNormalColor(new Vector3(x * 10, -c.cell1, (height - y) * 10), Color.White, c.vNormal1));
                        vertices_notexture.Add(new VertexPositionNormalColor(new Vector3(x * 10, -c.cell3, (height - y) * 10 - 10), Color.White, c.vNormal2));
                        vertices_notexture.Add(new VertexPositionNormalColor(new Vector3(x * 10 + 10, -c.cell2, (height - y) * 10), Color.White, c.vNormal3));
                        vertices_notexture.Add(new VertexPositionNormalColor(new Vector3(x * 10 + 10, -c.cell4, (height - y) * 10 - 10), Color.White, c.vNormal4));
                    }
                }
            }

            vertexBuffers = new Dictionary<Texture2D, VertexBuffer>();
            foreach (KeyValuePair<Texture2D, List<VertexPositionNormalTexture>> v in vertices)
            {
                vertexBuffer = new VertexBuffer(ROClient.Singleton.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, v.Value.ToArray().Length, BufferUsage.WriteOnly);
                vertexBuffer.SetData(v.Value.ToArray());
                vertexBuffers.Add(v.Key, vertexBuffer);
            }
            if (vertices_notexture.Count > 0)
            {
                vertexBuffer = new VertexBuffer(ROClient.Singleton.GraphicsDevice, VertexPositionNormalColor.VertexDeclaration, vertices_notexture.ToArray().Length, BufferUsage.WriteOnly);
                vertexBuffer.SetData(vertices_notexture.ToArray());
            }
        }

        private void calcVertexNormals()
        {
            int xfrom = 0;
            int yfrom = 0;
            int xto = width;
            int yto = height;

            int x;
            int y;

            for (y = yfrom; y <= yto - 1; y++)
            {
                for (x = xfrom; x <= xto - 1; x++)
                {
                    cubes[y][x].calcNormal();
                }
            }

            for (y = yfrom; y <= yto - 1; y++)
            {
                for (x = xfrom; x <= xto - 1; x++)
                {
                    cubes[y][x].vNormal1 = cubes[y][x].normal;
                    cubes[y][x].vNormal2 = cubes[y][x].normal;
                    cubes[y][x].vNormal3 = cubes[y][x].normal;
                    cubes[y][x].vNormal4 = cubes[y][x].normal;

                    if (y > 0)
                    {
                        cubes[y][x].vNormal1 += cubes[y - 1][x].normal;
                        cubes[y][x].vNormal3 += cubes[y - 1][x].normal;
                        if (x > 0)
                            cubes[y][x].vNormal1 += cubes[y - 1][x - 1].normal;
                        if (x < width - 1)
                            cubes[y][x].vNormal3 += cubes[y - 1][x + 1].normal;
                    }
                    if (x > 0)
                    {
                        cubes[y][x].vNormal1 += cubes[y][x - 1].normal;
                        cubes[y][x].vNormal2 += cubes[y][x - 1].normal;
                        if (y < height - 1)
                            cubes[y][x].vNormal2 += cubes[y + 1][x - 1].normal;
                    }
                    if (y < height - 1)
                    {
                        cubes[y][x].vNormal2 += cubes[y + 1][x].normal;
                        cubes[y][x].vNormal4 += cubes[y + 1][x].normal;
                        if (x < width - 1)
                            cubes[y][x].vNormal4 += cubes[y + 1][x + 1].normal;
                    }
                    if (x < width - 1)
                    {
                        cubes[y][x].vNormal3 += cubes[y][x + 1].normal;
                        cubes[y][x].vNormal4 += cubes[y][x + 1].normal;
                    }
                    cubes[y][x].vNormal1 = cubes[y][x].vNormal1;
                    cubes[y][x].vNormal2 = cubes[y][x].vNormal2;
                    cubes[y][x].vNormal3 = cubes[y][x].vNormal3;
                    cubes[y][x].vNormal4 = cubes[y][x].vNormal4;
                    cubes[y][x].vNormal1.Normalize();
                    cubes[y][x].vNormal2.Normalize();
                    cubes[y][x].vNormal3.Normalize();
                    cubes[y][x].vNormal4.Normalize();
                }
            }
        }

        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            ROClient.Singleton.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

            var sf = ROClient.Singleton.GuiManager.Client.Content.Load<SpriteFont>("fb\\Gulim8b");
            sb.Begin();
            sb.DrawString(sf, string.Format("X={0}, Y={1}, Z={2} -> X={3}, Y={4}, Z={5}", cameraPosition.X, cameraPosition.Y, cameraPosition.Y, cameraFinalTarget.X, cameraFinalTarget.Y, cameraFinalTarget.Z), new Vector2(10, 10), Color.White);
            sb.End();

            effect.CurrentTechnique = effect.Techniques["Textured"];

            Matrix worldMatrix = Matrix.Identity;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, ROClient.Singleton.GraphicsDevice.Viewport.AspectRatio, 1.0f, 20000.0f);

            effect.Parameters["xWorld"].SetValue(worldMatrix);
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);

            effect.Parameters["xEnableLighting"].SetValue(true);
            effect.Parameters["xAmbient"].SetValue(0.4f);
            effect.Parameters["xLightDirection"].SetValue(new Vector3(-0.5f, -1, -0.5f));


            effect.Parameters["xTexture"].SetValue(streetTexture);
            ROClient.Singleton.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }
            ROClient.Singleton.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, vertexBuffer.VertexCount);



            foreach (KeyValuePair<Texture2D, VertexBuffer> vb in vertexBuffers)
            {
                effect.Parameters["xTexture"].SetValue(vb.Key);
                ROClient.Singleton.GraphicsDevice.SetVertexBuffer(vb.Value);
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }
                ROClient.Singleton.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, vb.Value.VertexCount);
            }
        }

        public void Update(SpriteBatch sb, GameTime gameTime)
        {
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            ProcessInput(timeDifference);
        }

        private void ProcessInput(float amount)
        {
            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState != originalMouseState)
            {
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;
                leftrightRot -= rotationSpeed * xDifference * amount;
                updownRot -= rotationSpeed * yDifference * amount;
                Mouse.SetPosition(ROClient.Singleton.GraphicsDevice.Viewport.Width / 2, ROClient.Singleton.GraphicsDevice.Viewport.Height / 2);
                UpdateViewMatrix();
            }

            Vector3 moveVector = new Vector3(0, 0, 0);
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
                moveVector += new Vector3(0, 0, -1);
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
                moveVector += new Vector3(0, 0, 1);
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
                moveVector += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
                moveVector += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.Q))
                moveVector += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.Z))
                moveVector += new Vector3(0, -1, 0);
            AddToCameraPosition(moveVector * amount);
        }

        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            cameraPosition += moveSpeed * rotatedVector;
            UpdateViewMatrix();
        }
        Vector3 cameraFinalTarget;
        private void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);

            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            cameraFinalTarget = cameraPosition + cameraRotatedTarget;

            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraFinalTarget, cameraRotatedUpVector);
        }

        public void Dispose()
        {

        }
    }

    public class Cube
    {
        public float cell1;
        public float cell2;
        public float cell3;
        public float cell4;
        public int tileUp;
        public int tileSide;
        public int tileOtherSide;
        bool selected;
        bool hasModelOnTop;
        public Vector3 normal;
        public Vector3 vNormal1;
        public Vector3 vNormal2;
        public Vector3 vNormal3;

        public Vector3 vNormal4;
        public Cube(System.IO.BinaryReader br)
        {
            cell1 = br.ReadSingle();
            cell2 = br.ReadSingle();
            cell3 = br.ReadSingle();
            cell4 = br.ReadSingle();

            tileUp = br.ReadInt32();
            tileSide = br.ReadInt32();
            tileOtherSide = br.ReadInt32();
        }

        public void calcNormal()
        {
            Vector3 b1 = default(Vector3);
            Vector3 b2 = default(Vector3);
            b1 = new Vector3(10, -cell1, -10) - new Vector3(0, -cell4, 0);
            b2 = new Vector3(0, -cell3, -10) - new Vector3(0, -cell4, 0);
            normal = Vector3.Cross(b1, b2);
            normal.Normalize();
        }
    }

    public class Tile
    {
        public float u1;
        public float u2;
        public float u3;
        public float u4;
        public float v1;
        public float v2;
        public float v3;
        public float v4;
        public Int16 texture;
        public Int16 lightmap;
        public byte[] color;
        public bool used;
        public Tile(System.IO.BinaryReader br)
        {
            u1 = br.ReadSingle();
            u2 = br.ReadSingle();
            u3 = br.ReadSingle();
            u4 = br.ReadSingle();
            v1 = br.ReadSingle();
            v2 = br.ReadSingle();
            v3 = br.ReadSingle();
            v4 = br.ReadSingle();
            texture = br.ReadInt16();
            lightmap = br.ReadInt16();
            color = br.ReadBytes(4);
        }
    }
}
