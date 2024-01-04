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
using System.Windows.Forms.VisualStyles;
using SFML.Window;

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

        void DrawHudProcess() => uiList?.ForEach(ui => ui?.DrawProcess());
        

        void DrawUiDispose() => uiList?.ForEach(ui => ui?.Dispose());
        #endregion



        #region [UI 객체들]
        //조상 객체
        abstract class PlayerUiDrawer : IDisposable
        {
            protected Player master;

            protected float tabListMargin = 100f;
            protected float tabListWidth = 250f;

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

            public Tab choosenTab;
            List<Tab> tabList;

            public PlayerTabDrawer(Player master) : base(master)
            {
                int tabIndex = 0;
                tabList = new List<Tab> {
                    new TabInventory(master, tabIndex++),

                };
                choosenTab = tabList[0];
            }

            public void LogicProcess() 
            {
               foreach(Tab tab in tabList)
                {
                    tab.LogicProcess();
                };
            }

            public override void DrawProcess()
            {
                if(master.onInventory)
                    foreach (Tab tab in tabList)
                    {
                        tab.DrawTabProcess();
                        if (tab == choosenTab)
                            tab.DrawProcess();
                    }
            }

            public override void Dispose()
            {
            }
        }

        //탭
        abstract class Tab : PlayerUiDrawer
        {
            //기본 상수
            protected Vector2f screen = (Vector2f)Vm.resolutionNow;


            Text tabText;
            RectangleShape tabButton;

            public Tab(Player master, string name, int index) : base(master)
            {
                tabButton = new RectangleShape(new Vector2f(tabListWidth, tabListMargin));
                tabButton.Position = new Vector2f(index * tabListWidth, 0f);
                tabButton.FillColor = new Color(210, 210, 210);

                tabText = new Text(name, Rm.fonts["Jalnan"]);
                tabText.Position = tabButton.Position + new Vector2f(tabListWidth, tabListMargin) / 2f;
                tabText.CharacterSize = 70;
                tabText.Origin = new Vector2f(tabText.GetLocalBounds().Width / 2, tabText.GetLocalBounds().Height / 2);
                tabText.FillColor = new Color(60, 60, 60, 210);

            }

            //탭 버튼
            public void DrawTabProcess()
            {
                Dm.texUiPopup.Draw(tabButton);
                Dm.texUiPopup.Draw(tabText);
            }

            public abstract void LogicProcess();

            //전체 화면 드로우
            public abstract override void DrawProcess();

            public override void Dispose()
            {
                tabButton?.Dispose();
                tabText?.Dispose();
            }
        }


        //인벤토리 탭
        class TabInventory : Tab
        {
            //드로우 도형들
            RectangleShape equipmentBox, inventoryBox, containerBox;
            RectangleShape slotShape;
            RectangleShape outlineShape;
            RectangleShape itemShape;

            RectangleShape dropShape;   //아이템을 놓을 노드들 표시
            RectangleShape dragShape;   //끌려올 아이템을 그릴 도형

            //사전 값
            Dictionary<Rarerity, Color> rareityBackColor;

            float boxMargin = 15f, slotWidth;

            public TabInventory(Player master, int index) : base(master, "장비", index)
            {
                rareityBackColor = new Dictionary<Rarerity, Color>
                {
                    {Rarerity.COMMON , new Color(90, 90, 90, 80)},
                    {Rarerity.UNCOMMON , new Color(220, 220, 220, 80)},
                    {Rarerity.RARE , new Color(30, 30, 220, 80)},
                    {Rarerity.UNIQUE , new Color(220, 30, 220, 80)},
                    {Rarerity.QUEST , new Color(220, 30, 30, 80)},
                };

                slotWidth = (screen.X / 3f - boxMargin * 2f) / 8f;
                slotShape = new RectangleShape(new Vector2f(slotWidth, slotWidth));
                slotShape.FillColor = new Color(30, 30, 30);
                slotShape.OutlineThickness = 2f;
                slotShape.OutlineColor = new Color(15, 15, 15);

                //outlineShape = new RectangleShape();
                //outlineShape.FillColor = Color.Transparent;
                //outlineShape.OutlineThickness = 3f;
                //outlineShape.OutlineColor = Color.White;

                itemShape = new RectangleShape();
                //itemShape.FillColor = new Color(30, 30, 30);
                itemShape.OutlineThickness = 2f;
                itemShape.OutlineColor = new Color(50, 50, 50);


                equipmentBox = new RectangleShape(new Vector2f(screen.X / 3f, screen.Y - tabListMargin));
                equipmentBox.FillColor = new Color(60, 60, 60, 210);
                equipmentBox.Position = new Vector2f(screen.X / 3f * 0, tabListMargin);
                equipmentBox.OutlineColor = new Color(90, 90, 90, 210);
                equipmentBox.OutlineThickness = 10;

                inventoryBox = new RectangleShape(equipmentBox);
                inventoryBox.Position = new Vector2f(screen.X / 3f * 1, tabListMargin);

                containerBox = new RectangleShape(equipmentBox);
                containerBox.Position = new Vector2f(screen.X / 3f * 2, tabListMargin);


                dropShape = new RectangleShape(new Vector2f(slotWidth, slotWidth));
                dropShape.FillColor = Color.Transparent;

                dragShape = new RectangleShape();

            }

            //스토리지 && 슬롯 드로우
            void DrawStorage(Storage storage, Vector2f originPos) 
            {

                Vector2i pSize = storage.size;
                for (int x = 0; x < pSize.X; x++)
                    for (int y = 0; y < pSize.Y; y++)
                    {
                        slotShape.Position = originPos + new Vector2f(x, y) * slotWidth;
                        Dm.texUiPopup.Draw(slotShape);
                    }

                foreach (Storage.StorageNode sNode in storage.itemList)
                {
                    itemShape.Position = originPos + (Vector2f)sNode.pos * slotWidth;
                    itemShape.Size = sNode.isRotated ?
                        (Vector2f)new Vector2i(sNode.item.size.Y, sNode.item.size.X) * slotWidth :
                        (Vector2f)sNode.item.size * slotWidth;

                    itemShape.Texture = null;
                    itemShape.FillColor = rareityBackColor[sNode.item.rare];
                    Dm.texUiPopup.Draw(itemShape);

                    itemShape.Texture = Rm.textures[sNode.item.spriteName];
                    itemShape.FillColor = Color.White;
                    Dm.texUiPopup.Draw(itemShape);

                }
            }
            void DrawEquipSlot(Inventory.EquipSlot equipSlot, Vector2f originPos){ }

            //스토리지 && 슬롯 마우스 체크
            Vector2i? MouseCheckStorage(Storage storage, Vector2f originPos, Vector2f mousePos)
            {
                Point mouseMask = new Point(mousePos);
                Vector2i pSize = storage.size;
                
                for (int x = 0; x < pSize.X; x++)
                    for (int y = 0; y < pSize.Y; y++)
                    {
                        Vector2f pos = originPos + new Vector2f(x + 0.5f, y + 0.5f) * slotWidth;
                        Vector2f size = new Vector2f(1,1) * slotWidth;
                        Box slot = new Box(pos, size);
                        if (slot.IsCollision(mouseMask))
                            return new Vector2i(x, y);
                    }

                return null;
            }
            void MouseCheckEquipSlot(Inventory.EquipSlot equipSlot, Vector2f originPos) { }

            //각 패널에 대한 드로우
            void DrawEquipmentBox()
            {

            }
            void DrawInventoryBox()
            {
                try
                {
                    Inventory inventory = master.inventory;
                    Vector2f pocketPos = inventoryBox.Position + new Vector2f(boxMargin, boxMargin);

                    DrawStorage(inventory.pocket, pocketPos);

                }
                catch (Exception ex) { Console.WriteLine(ex.Message + ex.StackTrace); }

            }
            void DrawContainerBox()
            {
                if (master.interactingTarget == null) return;
            }

            //사용자 조작
            void LogicInput(Storage tStor, Vector2i? tStorPos, Storage.StorageNode? tStorNode, Inventory.EquipSlot tSlot)
            {
                //좌클릭 확인
                if (InputManager.CommandCheck(InputManager.CommandType.FIRE))
                {
                    //onCLick
                    if (clickBefore == false)
                    {
                    
                    }


                    //클릭 값 처리
                    clickBefore = true;
                }
                else 
                {

                    //클릭 값 처리
                    clickBefore = false;
                }
                //우클릭 확인
                if (InputManager.CommandCheck(InputManager.CommandType.AIM)) 
                {
                
                }


            }
            Storage.StorageNode? onDrag = null;
            bool clickBefore = false;   

            //통합 마우스 체크
            public override void LogicProcess()
            {
                //마우스가 올라갔는지 검사할 항목
                Storage tStor = null;   //스토리지
                Vector2i? tStorPos = null;  //스토리지 내 위치
                Storage.StorageNode? tStorNode = null;  //스토리지 내 위치에 따른 노드
                Inventory.EquipSlot tSlot = null;   //장착 슬롯

                //현재 마우스 위치
                Vector2f mousePos = (Vector2f)Mouse.GetPosition();

                Inventory inventory = master.inventory;

                Storage storage = inventory.pocket;
                Vector2f pocketPos = inventoryBox.Position + new Vector2f(boxMargin, boxMargin);

                Vector2i? pPos = MouseCheckStorage(storage, pocketPos, mousePos);
                if (pPos != null)
                {
                    tStor = storage;
                    tStorPos = pPos;
                    tStorNode = storage.GetPosTo((Vector2i)tStorPos);
                }

                Console.WriteLine("mouse on : " + tStorPos);





            }
            //통합 드로우
            public override void DrawProcess()
            {
                Dm.texUiPopup.Draw(equipmentBox);
                Dm.texUiPopup.Draw(inventoryBox);
                Dm.texUiPopup.Draw(containerBox);

                DrawEquipmentBox();
                DrawInventoryBox();
                DrawContainerBox();

            }
            //소멸자
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
