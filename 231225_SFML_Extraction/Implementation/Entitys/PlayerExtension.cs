using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rm = _231109_SFML_Test.ResourceManager;
using Cm = _231109_SFML_Test.CameraManager;
using Dm = _231109_SFML_Test.DrawManager;
using Im = _231109_SFML_Test.InputManager;
using Sm = _231109_SFML_Test.SoundManager;
using Vm = _231109_SFML_Test.VideoManager;
using System.Windows.Forms;

namespace _231109_SFML_Test
{
    internal partial class Player
    {
        //인벤토리
        void InventoryOpen()
        {
            onInventory = true;
            InputManager.mouseAllow = true;
        }
        void InventoryClose()
        {
            onInventory = false;
            InputManager.mouseAllow = false;
        }

        //UI
        #region [UI 인터페이스]
        List<PlayerUiDrawer> uiList;

        void DrawUiInit(params PlayerUiDrawer[] uis) => uiList = uis.ToList();

        void DrawHudProcess() => uiList.ForEach(ui => ui.DrawProcess());

        void DrawUiDispose() => uiList.ForEach(ui => ui?.Dispose());
        #endregion


        void DrawInventoryProcess()
        {
            //본인측 인벤토리 그리기

            if (interactingTarget != null)
            {
                //상대측 인벤토리 그리기
                //구현 해야됨.
            }
        }


        #region [UI 객체들]
        //조상 객체
        abstract class PlayerUiDrawer : IDisposable
        {
            protected Player master;

            public PlayerUiDrawer(Player master)
            {
                this.master = master;
            }
            public abstract void DrawProcess();

            public abstract void Dispose();
        }

        //HP 바
        class PlayerHpDrawer : PlayerUiDrawer
        {
            Vector2f hpUiSize = new Vector2f(600f, 30f);
            RectangleShape hpMaxBox, hpNowBox, hpBleedingBox;

            public PlayerHpDrawer(Player master) : base(master)
            {
                hpMaxBox = new RectangleShape(hpUiSize);
                hpMaxBox.OutlineColor = new Color(20, 20, 20);
                hpMaxBox.OutlineThickness = 6f;
                hpMaxBox.FillColor = new Color(40, 40, 40);
                hpMaxBox.Position = new Vector2f(0f, Vm.resolutionNow.Y) + new Vector2f(hpUiSize.X, -hpUiSize.Y) / 2f + new Vector2f(40f, -40f);
                hpMaxBox.Origin = hpUiSize / 2f;

                hpNowBox = new RectangleShape(hpUiSize);
                hpNowBox.FillColor = new Color(40, 210, 40);
                hpNowBox.Position = new Vector2f(0f, Vm.resolutionNow.Y) + new Vector2f(hpUiSize.X, -hpUiSize.Y) / 2f + new Vector2f(40f, -40f);
                hpNowBox.Origin = hpUiSize / 2f;

                hpBleedingBox = new RectangleShape(new Vector2f(0f, hpUiSize.Y));
                hpBleedingBox.Position = new Vector2f(0f, Vm.resolutionNow.Y) + new Vector2f(hpUiSize.X, -hpUiSize.Y) / 2f + new Vector2f(40f, -40f);
                hpBleedingBox.Origin = hpUiSize / 2f;

            }
            
            public override void DrawProcess()
            {
                Health health = master.health;

                //체력 게이지
                float hpRatio = Mathf.Clamp(0f, health.healthNow / health.healthMax, 1f);
                hpNowBox.Size = new Vector2f(hpUiSize.X * hpRatio, hpUiSize.Y);
                DrawManager.texUiInterface.Draw(hpMaxBox);
                DrawManager.texUiInterface.Draw(hpNowBox);

                //출혈
                float tTime = Math.Abs(Vm.GetTimeTotal() * 4f % 2f - 1f); //깜빡이는 이펙트
                hpBleedingBox.FillColor = new Color((byte)(40 * 170 * tTime), 40, 40);

                float bleedingRatio = Mathf.Clamp(0f, master.health.bleeding / health.healthNow, 1f);
                hpBleedingBox.Size = new Vector2f(hpNowBox.Size.X * bleedingRatio, hpUiSize.Y);
                hpBleedingBox.Position = new Vector2f(0f, Vm.resolutionNow.Y) + new Vector2f(hpUiSize.X, -hpUiSize.Y) / 2f + new Vector2f(40f, -40f) + new Vector2f(hpNowBox.Size.X * (1f - bleedingRatio), 0f);

                DrawManager.texUiInterface.Draw(hpBleedingBox);
            }

            public override void Dispose()
            {
                hpMaxBox?.Dispose();
                hpNowBox?.Dispose(); 
                hpBleedingBox?.Dispose();
            }
        }

        //조준점
        class PlayerAimDrawer : PlayerUiDrawer
        {
            CircleShape aimPoint;

            public PlayerAimDrawer(Player master) : base(master)
            {

                aimPoint = new CircleShape(5f);
                aimPoint.FillColor = Color.White;
                aimPoint.Origin = new Vector2f(aimPoint.Radius, aimPoint.Radius);
            }

            public override void DrawProcess()
            {
                aimPoint.Position = master.aimPosition + (Vector2f)Vm.resolutionNow / 2f;
                DrawManager.texUiInterface.Draw(aimPoint);
            }
            public override void Dispose()
            {
                aimPoint?.Dispose();
            }
        }


        //탭 드로워
        class PlayerTabDrawer : PlayerUiDrawer
        {
            //기본 상수
            protected Vector2f screen = (Vector2f)Vm.resolutionNow;
            protected float tabListMargin = 35f;

            Tab choosenTab;
            List<Tab> tabList;

            public PlayerTabDrawer(Player master) : base(master)
            {


            }


            public override void DrawProcess()
            {
            }

            public override void Dispose()
            {
            }
        }

        //탭
        class Tab : PlayerUiDrawer
        {
            //기본 상수
            protected Vector2f screen = (Vector2f)Vm.resolutionNow;
            protected float tabListMargin = 35f;

            string name;
            PlayerTabDrawer tabDrawer;

            public Tab(Player master, PlayerTabDrawer tabDrawer, string name) : base(master)
            {
                this.tabDrawer = tabDrawer;
                this.name = name;
            }

            //탭 버튼
            public void DrawTabProcess()
            {
            }

            //전체 화면 드로우
            public override void DrawProcess()
            {
            }

            public override void Dispose()
            {
            }
        }


        //인벤토리 탭
        class TabInventory : Tab
        {
            RectangleShape equipmentBox, inventoryBox, containerBox;

            public TabInventory(Player master, PlayerTabDrawer tabDrawer) : base(master, tabDrawer, "장비")
            {
                equipmentBox = new RectangleShape(new Vector2f(screen.X / 3f, screen.Y - tabListMargin));
                equipmentBox.FillColor = new Color(60, 60, 60, 210);
                equipmentBox.Position = new Vector2f(screen.X / 3f * 0, tabListMargin);

                inventoryBox = new RectangleShape(equipmentBox);
                inventoryBox.Position = new Vector2f(screen.X / 3f * 1, tabListMargin);

                containerBox = new RectangleShape(equipmentBox);
                containerBox.Position = new Vector2f(screen.X / 3f * 2, tabListMargin);
            }

            public override void DrawProcess()
            {
                Dm.texUiPopup.Draw(equipmentBox);
                Dm.texUiPopup.Draw(inventoryBox);
                Dm.texUiPopup.Draw(containerBox);
            }
            public override void Dispose()
            {
                equipmentBox.Dispose();
                inventoryBox.Dispose();
                containerBox.Dispose();
            }

        }



        #endregion





    }
}
