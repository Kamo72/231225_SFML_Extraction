using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _231109_SFML_Test
{
    internal class IngameLightDrawer : IDisposable
    {
        public IngameLightDrawer(List<ILightSource> lights, List<Structure> structures)
        {
            this.lights = lights;
            this.structures = structures;
        }
        List<ILightSource> lights;
        List<Structure> structures;

        public void Draw()
        {
            //조건에 부합하는 구조물의 테두리들을 따옵니다.
            List<List<Vector2f>> poligons = new List<List<Vector2f>>();
            foreach (Structure structure in structures)
            {
                //투명한가?
                if (structure.isTransparent == true) continue;
                //카메라에 담길만큼 충분히 가까운가?
                if ((structure.Position - CameraManager.position).Magnitude() >= CameraManager.size.Magnitude()/2f + 200f) continue;

                poligons.Add(structure.GetPoligon());
            }

            CircleShape circle;
            //circle.SetPointCount(0);



        }



        ~IngameLightDrawer() 
        {
            Dispose();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
