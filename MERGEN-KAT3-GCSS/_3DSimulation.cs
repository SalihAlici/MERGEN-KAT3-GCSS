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
            float size = 4.0f;

            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.Orange); // Tüm yüzeyleri turuncu yap

            // Ön yüz
            GL.Vertex3(-size, -size, size);
            GL.Vertex3(size, -size, size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(-size, size, size);

            // Arka yüz
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(-size, size, -size);
            GL.Vertex3(size, size, -size);
            GL.Vertex3(size, -size, -size);

            // Sol yüz
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(-size, -size, size);
            GL.Vertex3(-size, size, size);
            GL.Vertex3(-size, size, -size);

            // Sağ yüz
            GL.Vertex3(size, -size, -size);
            GL.Vertex3(size, size, -size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(size, -size, size);

            // Üst yüz
            GL.Vertex3(-size, size, -size);
            GL.Vertex3(-size, size, size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(size, size, -size);

            // Alt yüz
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(size, -size, -size);
            GL.Vertex3(size, -size, size);
            GL.Vertex3(-size, -size, size);

            GL.End();

            float parachuteSize = size * 1.2f;

            // Paraşüt kenarları (küre şeklinde)
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Color3(1.0f, 0.0f, 0.0f); // Kırmızı

            // Merkez nokta (aşağıda)
            GL.Vertex3(0, size + size * 0.8f, 0);

            // Kenar noktalar
            for (int i = 0; i <= 20; i++)
            {
                float angle = (float)i / 20 * 2 * (float)Math.PI;
                GL.Vertex3(
                    (float)Math.Cos(angle) * parachuteSize,
                    size + size * 0.3f,
                    (float)Math.Sin(angle) * parachuteSize
                );
            }
            GL.End();

            // 3. İPLER (4 ÜST KÖŞEYE BAĞLI)
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(0.7f, 0.0f, 0.0f); // Koyu kırmızı

            // 4 köşe noktası
            Vector3[] topCorners = new Vector3[]
            {
        new Vector3(-size, size, -size), // Sol-arka
        new Vector3(-size, size, size),  // Sol-ön
        new Vector3(size, size, size),   // Sağ-ön
        new Vector3(size, size, -size)   // Sağ-arka
            };

            // Her köşe için ip çiz
            foreach (Vector3 corner in topCorners)
            {
                // Paraşüt kenarına bağla
                GL.Vertex3(corner.X, corner.Y, corner.Z);
                GL.Vertex3(
                    corner.X * 0.3f,
                    size + size * 0.5f,
                    corner.Z * 0.3f
                );
            }
            GL.End();

        }










    }
}
