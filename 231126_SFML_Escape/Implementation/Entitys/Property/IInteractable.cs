﻿using System;
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
    }
}