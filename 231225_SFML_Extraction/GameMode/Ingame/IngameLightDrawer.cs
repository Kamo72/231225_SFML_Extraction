using SFML.Graphics;
using SFML.Graphics.Glsl;
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
            DrawManager.texWrLight.Clear(new Color(0, 0, 0, 255));


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


            RenderStates renderStates = new RenderStates()
            {
                Transform = CameraManager.worldRenderState.Transform,
                //BlendMode = new BlendMode(BlendMode.Factor.DstAlpha, BlendMode.Factor.DstAlpha, BlendMode.Equation.ReverseSubtract),
                BlendMode = new BlendMode(BlendMode.Factor.OneMinusDstColor, BlendMode.Factor.OneMinusSrcColor, BlendMode.Equation.ReverseSubtract),
            };

            CircleShape circleShape = new CircleShape(1000);
            circleShape.Origin = new Vector2f(1f, 1f) * circleShape.Radius;
            circleShape.Texture = ResourceManager.textures["LIGHT_radial"];
            //circleShape.TextureRect = new IntRect((Vector2i)circleShape.Position - new Vector2i(1, 1) * (int)circleShape.Radius, new Vector2i((int)circleShape.Radius *2, (int)circleShape.Radius *2));
            DrawManager.texWrLight.Draw(circleShape, renderStates);


            circleShape = new CircleShape(500f);
            circleShape.Position = new Vector2f(700f, 700f);
            circleShape.Origin = new Vector2f(1f, 1f) * circleShape.Radius;
            circleShape.Texture = ResourceManager.textures["LIGHT_radial"];
            //circleShape.TextureRect = new IntRect((Vector2i)circleShape.Position - new Vector2i(1, 1) * (int)circleShape.Radius, new Vector2i((int)circleShape.Radius *2, (int)circleShape.Radius *2));
            DrawManager.texWrLight.Draw(circleShape, renderStates);

            try
            {
                foreach (ILightSource light in lights)
                {
                    switch (light.lType)
                    {
                        case LightType.RADIAL:
                            CircleShape radialDraw = new CircleShape(light.lScale);
                            radialDraw.Origin = new Vector2f(1f, 1f) * radialDraw.Radius;
                            radialDraw.FillColor = light.lColor;
                            radialDraw.Position = light.lPosition; 
                            radialDraw.Texture = ResourceManager.textures["LIGHT_radial"];

                            DrawManager.texWrLight.Draw(radialDraw, renderStates);
                            radialDraw.FillColor = new Color(light.lColor.R, light.lColor.G, light.lColor.B, (byte)(light.lColor.A  * 0.7f));
                            radialDraw.Radius /= 2f;
                            radialDraw.Origin = new Vector2f(1f, 1f) * radialDraw.Radius;
                            DrawManager.texWrLight.Draw(radialDraw, CameraManager.worldRenderState);
                            break;
                        case LightType.CONE:
                            break;
                    }


                }
            }
            catch (Exception ex){ Console.WriteLine(ex.Message + ex.StackTrace); }

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
