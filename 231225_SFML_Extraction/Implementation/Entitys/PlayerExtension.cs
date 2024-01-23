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
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using System.Drawing.Printing;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace _231109_SFML_Test
{
    internal partial class Player
    {
        //인벤토리
        public void InventoryOpen()
        {
            hands.onInventory = true;
            InputManager.mouseAllow = true;
        }
        public void InventoryClose()
        {
            hands.onInventory = false;
            InputManager.mouseAllow = false;

            hands.interactingTarget = null;

            if (hands.interactingTarget is Container container)
                container.Close();
        }

        //UI
        #region [UI 인터페이스]
        List<PlayerUiDrawer> uiList;

        //UI methods
        void DrawUiInit(params PlayerUiDrawer[] uis) => uiList = uis.ToList();

        void DrawHudProcess()
        {
            try
            {
                uiList?.ForEach(ui => ui?.DrawProcess());
            }
            catch (System.AccessViolationException ex) { Console.WriteLine(ex.Message + ex.StackTrace); }
        }
        
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
        class PlayerCrosshairDrawer : PlayerUiDrawer
        {
            CircleShape staticDot;
            RectangleShape[] dynamicDotCrosshair;

            public PlayerCrosshairDrawer(Player master) : base(master)
            {

                staticDot = new CircleShape(4f);
                staticDot.FillColor = new Color(255, 255, 255, 255);
                staticDot.Origin = new Vector2f(staticDot.Radius, staticDot.Radius);

                
                dynamicDotCrosshair = new RectangleShape[]
                {
                    new RectangleShape(new Vector2f(20f, 3f)),
                    new RectangleShape(new Vector2f(20f, 3f)),
                    new RectangleShape(new Vector2f(20f, 3f)),
                    new RectangleShape(new Vector2f(20f, 3f)),
                };

                for (int i = 0; i < 4; i++)
                {
                    dynamicDotCrosshair[i].Rotation = 90f * i;
                    dynamicDotCrosshair[i].Origin = new Vector2f(-3f, dynamicDotCrosshair[0].Size.Y / 2f);
                    dynamicDotCrosshair[i].FillColor = new Color(255, 255, 255, 255);
                }

            }

            public override void DrawProcess()
            {
                staticDot.Position = master.aimPosition + (Vector2f)Vm.resolutionNow / 2f;
                DrawManager.texUiInterface.Draw(staticDot);

                Vector2f dynamicDot = master.aim.dynamicDot;
                float aimSpray = master.aim.hipSpray;

                float crosshairAlpha = Mathf.Clamp(0f, 1f - Math.Abs(master.aim.adsValue) * 2f, 1f);

                dynamicDotCrosshair[0].Position = dynamicDot + new Vector2f(aimSpray, 0f);
                dynamicDotCrosshair[1].Position = dynamicDot + new Vector2f(0f, +aimSpray);
                dynamicDotCrosshair[2].Position = dynamicDot + new Vector2f(-aimSpray, 0f);
                dynamicDotCrosshair[3].Position = dynamicDot + new Vector2f(0f, -aimSpray);

                foreach (var item in dynamicDotCrosshair)
                {
                    item.FillColor = new Color(item.FillColor) { A = (byte)(255 * crosshairAlpha) };
                    DrawManager.texUiInterface.Draw(item);
                }
            }
            public override void Dispose()
            {
                staticDot?.Dispose();
                foreach (var item in dynamicDotCrosshair)
                    item?.Dispose();
            }
        }


        class PlayerAdsDrawer : PlayerUiDrawer
        {
            static RectangleShape drawable;
            public AdsData sightAds;
            public AdsData weaponAds;
            public Weapon weapon;

            public PlayerAdsDrawer(Player master) : base(master)
            {
                drawable = new RectangleShape(new Vector2f(100f, 100f));
                drawable.Origin = drawable.Size / 2f;
            }
            static Random random = new Random();
            public override void DrawProcess()
            {
                try
                {
                    if (master.hands.handling is Weapon weapon)
                    {
                        if (this.weapon != weapon)
                        {
                            this.weapon = weapon;
                            //무기 초기화
                            weaponAds = AdsData.adsLibrary[weapon.status.aimDt.ads.adsName];
                            //sightAds = weapon.
                        }
                        
                    }


                    if (this.weapon != null)
                    {
                        //값가져오기
                        float adsAlpha = Mathf.Clamp(0f, (master.aim.adsValue - 0.5f) * 2f, 1f);
                        float size = 1f  + master.aim.recoilVec.Magnitude() / 500f;
                        Vector2f tremble = new Vector2f(
                            ((float)random.NextDouble() - 0.5f) * master.aim.recoilVec.Magnitude(),
                            ((float)random.NextDouble() - 0.5f) * master.aim.recoilVec.Magnitude());
                        Vector2f toTrag = master.aim.staticDot - master.aim.traggingDot;
                        Vector2f adsGet = new Vector2f(100f, 100f) * (float)Math.Pow(1f-adsAlpha, 1.5f);

                        Vector2f posTo = master.aim.dynamicDot + master.aim.recoilVec;
                        Vector2f posFrom = master.aim.traggingDot + (Vector2f)VideoManager.resolutionNow / 2f
                            - master.aim.adsStanceVec * 1f
                            + tremble * 0.5f
                            - toTrag * 0.4f
                            + adsGet;

                        //드로우
                        DrawAds(DrawManager.texWrAugment, weaponAds, posTo, posFrom, adsAlpha, size + (float)Math.Pow(1f - adsAlpha, 1.5f));
                    }



                }
                catch (Exception ex) { Console.WriteLine(ex.Message + ex.StackTrace); }
            }

            public override void Dispose()
            {
                drawable.Dispose();
            }


            void DrawAds(RenderTexture rTexture, AdsData ads, Vector2f aimPos, Vector2f screenPos, float alpha = 1f, float size = 1f)
            {
                try
                {
                    //if (ads.adsType == AdsData.AdsType.SCOPE || ads.adsType == AdsData.AdsType.DOT_SIGHT)
                    //{
                    //    //TODO!
                    //}

                    float originDepth = 100f;
                    Vector2f posDelta = (screenPos - aimPos) / originDepth;

                    foreach (var pair in ads.depthSpritePair)
                    {
                        float depth = pair.Key;
                        Texture texture = ResourceManager.textures[pair.Value];

                        drawable.Texture = texture;
                        drawable.FillColor = new Color(Color.White) { A = (byte)(255 * alpha) };
                        drawable.Scale = new Vector2f(1, 1) * size * (originDepth + depth) / originDepth;
                        drawable.Position = aimPos + posDelta * depth;

                        rTexture.Draw(drawable);
                    }

                }
                catch (Exception ex) { Console.WriteLine(ex.Message + ex.StackTrace); }
                
            }
        }

        public class AdsData 
        {
            public static Dictionary<string, AdsData> adsLibrary = new Dictionary<string, AdsData>();

            public enum AdsType 
            {
                IRON_SIGHT, //기계식 조준경 - X
                DOT_SIGHT,  //전자식 조준경 - 정확한 조준점, 
                SCOPE,      //광학 조준경 - 정확한 조준점, 조준점에서 멀면 까맣게 보임.
            }
            
            public AdsType adsType;
            public string sightMask;
            public string sightSprite;
            public Dictionary<int, string> depthSpritePair;
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
                if(master.hands.onInventory)
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
            #region [그리기 도형 및 변수 선언]
            //드로우 도형들
            RectangleShape equipmentBox, inventoryBox, containerBox;
            RectangleShape itemShape, cellShape;
            RectangleShape outlineShape;

            //슬롯
            RectangleShape slotShape;
            Text slotText, containerText;

            RectangleShape dropShape;   //아이템을 놓을 노드들 표시
            RectangleShape dragShape;   //끌려올 아이템을 그릴 도형

            //사전 값
            Dictionary<Rarerity, Color> rareityBackColor;

            float boxMargin = 15f, slotWidth;

            //장비
            RectangleShape weaponFirstShape, weaponSecondShape, weaponSubShape,
                helmetShape, headgearShape, plateCarrierShape, armourPlateShape, backpackShape;
            RectangleShape smallEquipShape, bigEquipShape, smallItemShape, bigItemShape;
            const float equipmentSep = 0.025f, equipmentWid = 0.250f, equipmentSepOuter = 0.075f;

            #endregion 

            public TabInventory(Player master, int index) : base(master, "장비", index)
            {
                #region [Inventory & Container]

                rareityBackColor = new Dictionary<Rarerity, Color>
                {
                    {Rarerity.COMMON , new Color(90, 90, 90, 80)},
                    {Rarerity.UNCOMMON , new Color(220, 220, 220, 80)},
                    {Rarerity.RARE , new Color(30, 30, 220, 80)},
                    {Rarerity.UNIQUE , new Color(220, 30, 220, 80)},
                    {Rarerity.QUEST , new Color(220, 30, 30, 80)},
                };

                slotWidth = (screen.X / 3f - boxMargin * 2f) / 8f;

                cellShape = new RectangleShape(new Vector2f(slotWidth, slotWidth));
                cellShape.FillColor = new Color(30, 30, 30);
                cellShape.OutlineThickness = 2f;
                cellShape.OutlineColor = new Color(15, 15, 15);

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

                slotShape = new RectangleShape(new Vector2f(2f, 2f) * slotWidth);
                slotShape.FillColor = new Color(30, 30, 30);
                slotShape.OutlineThickness = 2f;
                slotShape.OutlineColor = new Color(15, 15, 15);

                slotText = new Text("슬롯", Rm.fonts["Jalnan"], 15);
                slotText.Origin = new Vector2f(0f, slotText.CharacterSize);
                slotText.FillColor = Color.White;
                slotText.OutlineThickness = 1f;
                slotText.OutlineColor = new Color(30, 30, 30);

                containerText = new Text("슬롯", Rm.fonts["Jalnan"], 25);
                containerText.FillColor = Color.White;
                containerText.OutlineColor = Color.Black;
                containerText.Position = containerBox.Position + new Vector2f(boxMargin, boxMargin) + new Vector2f(10f, 10f);

                #endregion

                #region [Equipment]

                smallEquipShape = new RectangleShape(new Vector2f(
                        equipmentBox.Size.X * (equipmentWid * 1f),
                        equipmentBox.Size.X * (equipmentWid * 1f)
                        ));
                smallEquipShape.FillColor = new Color(30, 30, 30);
                smallEquipShape.OutlineThickness = 2f;
                smallEquipShape.OutlineColor = new Color(15, 15, 15);

                bigEquipShape = new RectangleShape(smallEquipShape);
                bigEquipShape.Size = new Vector2f(
                        equipmentBox.Size.X * (equipmentWid * 2f + equipmentSep),
                        equipmentBox.Size.X * (equipmentWid * 1f)
                        );

                smallItemShape = new RectangleShape(smallEquipShape.Size);
                bigItemShape = new RectangleShape(bigEquipShape.Size);

                #endregion
            }

            #region [스토리지, 창고 제어]
            //스토리지 && 슬롯 드로우
            void DrawStorage(Storage storage, Vector2f originPos) 
            {
                Vector2i pSize = storage.size;
                for (int x = 0; x < pSize.X; x++)
                    for (int y = 0; y < pSize.Y; y++)
                    {
                        cellShape.Position = originPos + new Vector2f(x, y) * slotWidth;
                        Dm.texUiPopup.Draw(cellShape);
                    }

                //그래그 중인 아이템 잔상
                if (tStor == storage) 
                {
                    if (tStorPos != null) 
                    {
                        if (onDrag != null) 
                        {
                            Vector2i ttStorPos = (Vector2i)tStorPos;
                            Storage.StorageNode tOnDrag = (Storage.StorageNode)onDrag;
                            Vector2i sepDragRotated = isRotated != tOnDrag.isRotated ? new Vector2i(sepDrag.Y, sepDrag.X) : sepDrag;


                            //삽입 체크를 위한 드래그 중의 아이템 제거
                            storage.RemoveItem(tOnDrag.item);
                            //삽입 가능한가?
                            bool isInsertable = storage.IsAbleToInsert(tOnDrag.item, ttStorPos - sepDragRotated, isRotated);
                            //삽입 체크를 위한 드래그 중의 아이템 제거
                            storage.Insert(tOnDrag);


                            //시작 지점 및 끝지점 
                            Vector2i startPos = ttStorPos - sepDragRotated;
                            Vector2i endPos = ttStorPos - sepDragRotated +
                                (isRotated? new Vector2i(tOnDrag.item.size.Y, tOnDrag.item.size.X) : tOnDrag.item.size);// - new Vector2i(-1, -1)


                            for (int x = Math.Max(0, startPos.X); x < Math.Min(storage.size.X, endPos.X); x++)
                                for (int y = Math.Max(0, startPos.Y); y < Math.Min(storage.size.Y, endPos.Y); y++) 
                                {
                                    dragShape.Position = originPos + new Vector2f(x, y) * slotWidth;

                                    dragShape.Size = new Vector2f(1, 1) * slotWidth;
                                    dragShape.Rotation = 0f;
                                    dragShape.Texture = null;
                                    dragShape.FillColor = isInsertable? Color.Green : Color.Red;
                                    Dm.texUiPopup.Draw(dragShape);
                                }




                        }
                    }
                }

                //모든 아이템 그리기
                foreach (Storage.StorageNode sNode in storage.itemList)
                {
                    itemShape.Position = originPos + (Vector2f)sNode.pos * slotWidth
                        + ( sNode.isRotated ? new Vector2f() { Y = slotWidth * sNode.item.size.X } : new Vector2f() );

                    itemShape.Size = (Vector2f)sNode.item.size * slotWidth;
                    itemShape.Rotation = sNode.isRotated ? -90f : 0f;

                    itemShape.Texture = null;
                    itemShape.FillColor = rareityBackColor[sNode.item.rare];
                    Dm.texUiPopup.Draw(itemShape);
                    //희귀도에 따른 배경색

                    itemShape.Texture = Rm.textures[sNode.item.spriteName];
                    itemShape.FillColor = new Color(255, 255, 255, (byte)(sNode.Equals(onDrag)? 63 : 255));

                    Dm.texUiPopup.Draw(itemShape);
                    //아이템 그림 드로우
                }

            }
            void DrawEquipSlot(Inventory.EquipSlot equipSlot, Vector2f originPos)
            {
                RectangleShape socketShape = null;
                switch (equipSlot.equipmentType)
                {
                    case EquipmentType.WEAPON:
                        if (equipSlot is Inventory.EquipSlotWeapon wSlot)
                        {
                            //보조무기
                            if (wSlot.isMain == false)
                            {
                                socketShape = smallEquipShape;
                            }
                            //주 무기
                            else 
                            {
                                socketShape = bigEquipShape;
                            }
                        }
                        break;
                    default:
                        socketShape = smallEquipShape;
                        break;
                }

                RectangleShape itemShape = socketShape == smallEquipShape? smallItemShape : bigItemShape;


                socketShape.Position = originPos;

                if (equipSlot.item != null)
                {   
                    socketShape.FillColor = new Color(45, 45, 45, 255);
                    socketShape.OutlineThickness = 2f;
                    socketShape.OutlineColor = new Color(90, 90, 90, 255);
                    DrawManager.texUiPopup.Draw(socketShape);

                    if (equipSlot.item is IHandable ih)
                    {
                        float drawSize = 2.5f;
                        ih.DrawHandable(DrawManager.texUiPopup, socketShape.Position + new Vector2f(socketShape.Size.X / 3f, socketShape.Size.Y / 2f), 0f, new Vector2f(1f, 1f) * drawSize, RenderStates.Default);
                    }
                    else
                    {
                        itemShape.Position = socketShape.Position;
                        itemShape.Texture = ResourceManager.textures[((Item)equipSlot.item).spriteName];
                        DrawManager.texUiPopup.Draw(itemShape);
                    }
                }
                else
                {
                    socketShape.FillColor = new Color(20, 20, 20, 210);
                    socketShape.OutlineThickness = 0f;
                    DrawManager.texUiPopup.Draw(socketShape);
                }

                slotText.Position = socketShape.Position;
                slotText.DisplayedString = equipSlot.GetCartegory();
                DrawManager.texUiPopup.Draw(slotText);

            }
            void DrawPlateSlot(Vector2f originPos)
            {
                smallEquipShape.Position = originPos;
                //new Vector2f(
                //        equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 2f),
                //        equipmentBox.Size.Y * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 2f)
                //        );

                if (master.inventory.plateCarrier.item is PlateCarrier pc && pc.armourPlate != null)
                {
                    smallEquipShape.FillColor = new Color(45, 45, 45, 255);
                    smallEquipShape.OutlineThickness = 2f;
                    smallEquipShape.OutlineColor = new Color(90, 90, 90, 255);
                    DrawManager.texUiPopup.Draw(smallEquipShape);

                    smallItemShape.Position = smallEquipShape.Position;
                    smallItemShape.Texture = ResourceManager.textures[pc.armourPlate.spriteName];
                    DrawManager.texUiPopup.Draw(smallItemShape);
                }
                else 
                {
                    smallEquipShape.FillColor = new Color(20, 20, 20, 210);
                    smallEquipShape.OutlineThickness = 0f;
                    DrawManager.texUiPopup.Draw(smallEquipShape);
                }

                slotText.Position = smallEquipShape.Position;
                slotText.DisplayedString = "방탄판";
                DrawManager.texUiPopup.Draw(slotText);
            }

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
            #endregion

            #region [패널 및 커서]
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
                if (master.hands.interactingTarget is Container container)
                {

                    Vector2f containerPos = containerBox.Position + new Vector2f(boxMargin, boxMargin) + new Vector2f(0f, 60f);
                    DrawStorage(container.storage, containerPos);

                    
                    containerText.DisplayedString = container.name;
                    Dm.texUiPopup.Draw(containerText);
                }
            }
            void DrawCursor()
            {
                //집은 아이템 없음
                if (onDrag == null)
                {

                }
                //집은 아이템 있음
                else
                {
                    Storage.StorageNode tOnDrag = (Storage.StorageNode)onDrag;

                    //오리진 계산
                    Vector2f sepRotated = tOnDrag.isRotated != isRotated ? new Vector2f(sepDrag.Y, sepDrag.X) : (Vector2f)sepDrag;
                    Vector2f sepVec = -(sepRotated + new Vector2f(0.5f, 0.5f)) * slotWidth;


                    //위치 변경
                    dragShape.Position = (Vector2f)Mouse.GetPosition() + sepVec + new Vector2f() { Y = isRotated ? slotWidth * tOnDrag.item.size.X : 0f };

                    dragShape.Size = (Vector2f)tOnDrag.item.size * slotWidth;
                    dragShape.Rotation = isRotated ? -90f : 0f;

                    dragShape.Texture = Rm.textures[tOnDrag.item.spriteName];
                    dragShape.FillColor = Color.White;
                    Dm.texUiPopup.Draw(dragShape);
                }


            }
            #endregion

            #region [사용자 입력 제어]
            //사용자 조작
            Storage onDragStorage = null;
            Storage.StorageNode? onDrag = null;
            Vector2i sepDrag = new Vector2i();
            bool clickBefore = false;
            bool isRotated;


            //마우스가 올라갔는지 검사할 항목
            Storage tStor = null;   //스토리지
            Vector2i? tStorPos = null;  //스토리지 내 위치
            Storage.StorageNode? tStorNode = null;  //스토리지 내 위치에 따른 노드
            Inventory.EquipSlot tSlot = null;   //장착 슬롯

            //논리 프로세스
            void LogicInput(Storage tStor, Vector2i? tStorPos, Storage.StorageNode? tStorNode, Inventory.EquipSlot tSlot)
            {
                //좌클릭 확인
                if (InputManager.CommandCheck(InputManager.CommandType.FIRE))
                {
                    //onClick
                    if (clickBefore == false)
                    {
                        //현재 위치에 있는 아이템이 있다면
                        if (tStorNode.HasValue)
                        {
                            //버리기
                            if (Keyboard.IsKeyPressed(Keyboard.Key.LShift))
                            {
                                if (tStorNode.HasValue)
                                    master.inventory.ThrowItem(tStorNode.Value.item);
                            }
                            //빠른 옮기기
                            else if (Keyboard.IsKeyPressed(Keyboard.Key.LControl))
                            {
                                onDragStorage = tStor;

                                if (tStorNode.HasValue)
                                    master.inventory.TakeItem(tStorNode.Value.item);
                            }
                            //빠른 장착
                            else if (Keyboard.IsKeyPressed(Keyboard.Key.LShift))
                            {
                                if (tStorNode.HasValue) 
                                    master.inventory.EquipItemQuick(tStorNode.Value.item);
                            }
                            //집기
                            else
                            {
                                //현재 위치에 있는 아이템을 집는다.
                                onDrag = tStorNode;
                                sepDrag = (Vector2i)tStorPos - ((Storage.StorageNode)tStorNode).pos;
                                isRotated = ((Storage.StorageNode)tStorNode).isRotated;
                                onDragStorage = tStor;
                            }
                        }
                    }

                    if (onDrag != null)
                    { 
                        //아이템 회전
                        if (Im.CommandCheck(Im.CommandType.MAGAZINE_CHANGE)) 
                        {
                            isRotated = !isRotated;
                        }
                    }

                    //클릭 값 처리
                    clickBefore = true;
                }
                //좌클릭 없음
                else
                {
                    //onRealease
                    if (clickBefore == true)
                    {
                        //드래그 중인 아이템이 있다면
                        if (onDrag != null)
                        {
                            Console.WriteLine("아이템 놓기 입력");

                            //가방 안의 대상 위치가 비어있다면
                            if (tStor != null && tStorPos != null)
                            {
                                Console.WriteLine("아이템 놓기 시도 ");


                                //형변환
                                Storage ttStor = tStor ;
                                Storage.StorageNode tOnDrag = (Storage.StorageNode)onDrag;
                                Vector2i ttStorPos = (Vector2i)tStorPos;
                                Vector2i sepDragRotated = isRotated != tOnDrag.isRotated ? new Vector2i(sepDrag.Y, sepDrag.X) : sepDrag;

                                //삽입 체크를 위한 드래그 중의 아이템 제거
                                ttStor.RemoveItem(tOnDrag.item);

                                //삽입 가능한가?
                                bool isInsertable = ttStor.IsAbleToInsert(tOnDrag.item, ttStorPos - sepDragRotated, isRotated);
                                if (isInsertable)
                                {
                                    Storage.StorageNode newNode = new Storage.StorageNode()
                                    {
                                        item = tOnDrag.item,
                                        pos = ttStorPos - sepDragRotated,
                                        isRotated = isRotated
                                    };

                                    //삽입 시도 및 결과 반환
                                    bool isInserted = ttStor.Insert(newNode);

                                    Console.WriteLine($"아이템 놓기 : {isInsertable}/ 신규 노드 결과 : {isInserted} - {newNode.pos} {newNode.isRotated}");
                                }
                                else 
                                {
                                    //뺐던거 다시 복구
                                    bool isInserted = ttStor.Insert(tOnDrag);

                                    Console.WriteLine($"아이템 놓기 : {isInsertable}/ 원상 복구 결과 : {isInserted} - {tOnDrag.pos} {tOnDrag.isRotated}");
                                }

                            }

                            //드래그 중인 아이템 해제
                            onDrag = null;
                            onDragStorage = tStor;
                        }

                    }

                    //클릭 값 처리
                    clickBefore = false;
                }


                //우클릭 확인
                if (InputManager.CommandCheck(InputManager.CommandType.AIM))
                {

                }
                //우클릭 없음
                else 
                {
                
                }
            }
            #endregion


            //통합 마우스 체크
            public override void LogicProcess()
            {
                tStor = null;   //스토리지
                tStorPos = null;  //스토리지 내 위치
                tStorNode = null;  //스토리지 내 위치에 따른 노드
                tSlot = null;   //장착 슬롯

                //현재 마우스 위치
                Vector2f mousePos = (Vector2f)Mouse.GetPosition();

                //포켓 클릭 제어
                Inventory inventory = master.inventory;
                Storage storage = inventory.pocket;
                Vector2f pocketPos = inventoryBox.Position + new Vector2f(boxMargin, boxMargin);

                Vector2i? pPos = MouseCheckStorage(storage, pocketPos, mousePos);
                if (pPos.HasValue)
                {
                    tStor = storage;
                    tStorPos = pPos;
                    tStorNode = storage.GetPosTo((Vector2i)tStorPos);
                }


                //상호작용 중인 컨테이너 제어
                if (master.hands.interactingTarget is Container container)
                {
                    storage = container.storage;
                    Vector2f containerPos = containerBox.Position + new Vector2f(boxMargin, boxMargin) + new Vector2f(0f, 50f);


                    Vector2i? cPos = MouseCheckStorage(storage, containerPos, mousePos);
                    if (cPos.HasValue)
                    {
                        tStor = storage;
                        tStorPos = cPos;
                        tStorNode = storage.GetPosTo((Vector2i)tStorPos);
                    }
                }








                LogicInput(tStor, tStorPos, tStorNode, null);
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

                DrawEquipSlot(master.inventory.weaponPrimary, new Vector2f(
                        equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 0f),
                        equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 3f)
                        ));
                DrawEquipSlot(master.inventory.weaponSecondary, new Vector2f(
                        equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 0f),
                        equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 4f)
                        ));
                DrawEquipSlot(master.inventory.weaponSub, new Vector2f(
                        equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 2f),
                        equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 3f)
                        ));

                DrawEquipSlot(master.inventory.helmet, new Vector2f(
                       equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 1f),
                       equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 1f)
                       ));
                DrawEquipSlot(master.inventory.headgear, new Vector2f(
                       equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 0f),
                       equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 1f)
                       ));
                DrawEquipSlot(master.inventory.backpack, new Vector2f(
                       equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 2f),
                       equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 1f)
                       ));
                DrawEquipSlot(master.inventory.plateCarrier, new Vector2f(
                       equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 1f),
                       equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 2f)
                       ));

                DrawPlateSlot(new Vector2f(
                        equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 2f),
                        equipmentBox.Size.X * (equipmentSepOuter * 1f + (equipmentWid + equipmentSep) * 2f)
                        ));

                DrawCursor();

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
