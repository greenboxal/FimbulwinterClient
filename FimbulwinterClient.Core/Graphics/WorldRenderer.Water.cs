using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using DrawElementsType = OpenTK.Graphics.OpenGL.DrawElementsType;

namespace FimbulwinterClient.Core.Graphics
{
    public partial class WorldRenderer
    {
        public VertexBuffer WaterBuffer { get; private set; }
        public IndexBuffer WaterIndexes { get; private set; }
        public Texture2D[] WaterTextures { get; private set; }
        public int WaterCurrentTexture { get; private set; }
        public WaterShaderProgram WaterShaderProgram { get; private set; }

        public static Texture2D[][] WaterTextureCache { get; private set; }

        private void LoadWater()
        {
            WaterShaderProgram = new WaterShaderProgram();
            WaterBuffer = new VertexBuffer(VertexPositionNormalTexture.VertexDeclaration);
            WaterIndexes = new IndexBuffer(DrawElementsType.UnsignedShort);

            WaterTextures = CacheWaterTextures(Map.World.WaterInfo.Type);
            WaterCurrentTexture = 0;

            VertexPositionNormalTexture[] vertexdata = new VertexPositionNormalTexture[4];
            short[] indexdata = new short[4];

            Vector3[] position = new Vector3[4];
            Vector2[] tex = new Vector2[4];

            float x0 = (-Map.Ground.Width / 2) * Map.Ground.Zoom;
            float x1 = ((Map.Ground.Width - 1) - Map.Ground.Width / 2 + 1) * Map.Ground.Zoom;

            float z0 = (-Map.Ground.Height / 2) * Map.Ground.Zoom;
            float z1 = ((Map.Ground.Height - 1) - Map.Ground.Height / 2 + 1) * Map.Ground.Zoom;
            
            position[0] = new Vector3(x0, Map.World.WaterInfo.Level, z0);
            position[1] = new Vector3(x1, Map.World.WaterInfo.Level, z0);
            position[2] = new Vector3(x0, Map.World.WaterInfo.Level, z1);
            position[3] = new Vector3(x1, Map.World.WaterInfo.Level, z1);

            tex[0] = new Vector2(0, 0);
            tex[1] = new Vector2(Map.Ground.Width / 8, 0);
            tex[2] = new Vector2(0, Map.Ground.Height / 8);
            tex[3] = new Vector2(Map.Ground.Width / 8, Map.Ground.Height / 8);

            vertexdata[0] = new VertexPositionNormalTexture(position[0], new Vector3(1.0F), tex[0]);
            vertexdata[1] = new VertexPositionNormalTexture(position[1], new Vector3(1.0F), tex[1]);
            vertexdata[2] = new VertexPositionNormalTexture(position[2], new Vector3(1.0F), tex[2]);
            vertexdata[3] = new VertexPositionNormalTexture(position[3], new Vector3(1.0F), tex[3]);

            indexdata[0] = 0;
            indexdata[1] = 1;
            indexdata[2] = 2;
            indexdata[3] = 3;

            WaterBuffer.SetData(vertexdata, BufferUsageHint.StaticDraw);
            WaterIndexes.SetData(indexdata, BufferUsageHint.StaticDraw);
        }

        private static Texture2D[] CacheWaterTextures(int type)
        {
            if (WaterTextureCache == null)
            {
                WaterTextureCache = new Texture2D[8][];
            }

            if (WaterTextureCache[type] == null)
            {
                WaterTextureCache[type] = new Texture2D[32];

                for (int j = 0; j < 32; j++)
                {
                    string sj = j.ToString(CultureInfo.InvariantCulture);

                    if (j < 10)
                        sj = "0" + sj;

                    WaterTextureCache[type][j] = ContentManager.Instance.Load<Texture2D>(string.Format(@"data\texture\¿öÅÍ\water{0}{1,2}.jpg", type, sj), true);
                    WaterTextureCache[type][j].SetWrapMode(TextureWrapMode.MirroredRepeat, TextureWrapMode.MirroredRepeat);
                }
            }

            return WaterTextureCache[type];
        }
    }
}
