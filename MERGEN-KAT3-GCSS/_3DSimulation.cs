using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace MERGEN_KAT3_GCSS
{
    public partial class _3DSimulation
    {
        private GLControl glControl1;




        public float x = 0, y = 0, z = 0;



        public _3DSimulation(GLControl glControl)
        {
            this.glControl1 = glControl ?? throw new ArgumentNullException(nameof(glControl));



        }







        public void UpdateRotation(float yaw, float pitch, float roll)
        {

            x = pitch;
            y = roll;
            z = yaw;

            glControl1.Invalidate();
        }


        public void DrawCube()
        {
            GL.PushMatrix();

            float size = 4.0f;
            float parachuteRadius = size * 1.0f;
            float heightOffset = size * 1.2f;

            
           

            // === Küp ===
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.Orange);

            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.Orange);

            // Ön yüz
            GL.Normal3(0, 0, 1);
            GL.Vertex3(-size, -size, size);
            GL.Vertex3(size, -size, size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(-size, size, size);

            // Arka yüz
            GL.Normal3(0, 0, -1);
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(-size, size, -size);
            GL.Vertex3(size, size, -size);
            GL.Vertex3(size, -size, -size);

            // Sol yüz
            GL.Normal3(-1, 0, 0);
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(-size, -size, size);
            GL.Vertex3(-size, size, size);
            GL.Vertex3(-size, size, -size);

            // Sağ yüz
            GL.Normal3(1, 0, 0);
            GL.Vertex3(size, -size, -size);
            GL.Vertex3(size, size, -size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(size, -size, size);

            // Üst yüz
            GL.Normal3(0, 1, 0);
            GL.Vertex3(-size, size, -size);
            GL.Vertex3(-size, size, size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(size, size, -size);

            // Alt yüz
            GL.Normal3(0, -1, 0);
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(size, -size, -size);
            GL.Vertex3(size, -size, size);
            GL.Vertex3(-size, -size, size);
            GL.End();

            

            // === Paraşüt ===
            int slices = 30, stacks = 10;
            GL.Color3(1.0f, 0.1f, 0.1f);

            for (int i = 0; i < stacks; i++)
            {
                float lat0 = (float)Math.PI / 2 * i / stacks;
                float lat1 = (float)Math.PI / 2 * (i + 1) / stacks;

                float y0 = (float)Math.Sin(lat0);
                float y1 = (float)Math.Sin(lat1);

                float r0 = (float)Math.Cos(lat0);
                float r1 = (float)Math.Cos(lat1);

                GL.Begin(PrimitiveType.QuadStrip);
                for (int j = 0; j <= slices; j++)
                {
                    float lon = 2 * (float)Math.PI * j / slices;
                    float x = (float)Math.Cos(lon);
                    float z = (float)Math.Sin(lon);

                    GL.Vertex3(x * parachuteRadius * r0, size + heightOffset + y0 * parachuteRadius, z * parachuteRadius * r0);
                    GL.Vertex3(x * parachuteRadius * r1, size + heightOffset + y1 * parachuteRadius, z * parachuteRadius * r1);
                }
                GL.End();
            }

            // === İpler ===
            Vector3[] topCorners = new Vector3[]
            {
        new Vector3(-size, size, -size),
        new Vector3(-size, size, size),
        new Vector3(size, size, size),
        new Vector3(size, size, -size)
            };

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(0.4f, 0.0f, 0.0f);
            foreach (var corner in topCorners)
            {
                GL.Vertex3(corner);
                GL.Vertex3(corner.X * 0.5f, size + heightOffset + parachuteRadius * 0.3f, corner.Z * 0.5f);
            }
            GL.End();

            GL.PopMatrix();
        }










    }
}
