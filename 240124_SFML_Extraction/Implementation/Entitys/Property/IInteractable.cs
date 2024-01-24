using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal interface IInteractable
    {
        bool IsInteractable(Humanoid caster);
        void BeInteract(Humanoid caster);

        void InitHighlight();
        Text highlightText { get; set; }
        RectangleShape highlightShape { get; set; }
        bool isHighlighed { get; set; }
        float highlighValue { get; set; }
        void DrawHighlight();
    }
}
