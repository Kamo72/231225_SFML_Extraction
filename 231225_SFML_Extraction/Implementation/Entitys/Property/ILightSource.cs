using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal interface ILightSource
    {
        //빛의 위상
        LightType lType { get; set; }
        Vector2f lPosition { get; set; }
        float lRotation { get; set; }
        float lScale { get; set; }
        Color lColor { get; set; }
    }
    public enum LightType
    {
        RADIAL,
        CONE,
    }
}
