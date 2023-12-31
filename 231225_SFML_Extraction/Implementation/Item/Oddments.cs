using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal class Oddment : Item
    {
        public Oddment() : base()
        {
            SetupBasicData("잡동사니", "ITEM_Oddment", "잡동사니 아이텝입니다.", 1.0f, new Vector2i(1, 1), Rarerity.COMMON, 2000f);

            if (Program.tm.gmNow is GamemodeIngame gm)
                gm.items.Add(this);

        }

        public override void Dispose()
        {
            if (Program.tm.gmNow is GamemodeIngame gm)
                gm.items.Remove(this);

            base.Dispose();
        }
    }
}
