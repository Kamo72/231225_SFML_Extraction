using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
 

namespace _231109_SFML_Test
{

    public enum Rarerity
    {
        COMMON,
        UNCOMMON,
        RARE,
        UNIQUE,
        QUEST,      //매각 및 처분 불가.
    }

    internal class Storage
    {
        internal struct StorageNode
        {
            public Item item;
            public Vector2i pos;
            public bool isRotated;

            public override string ToString()
            {
                return $"{item.name} {pos} {(isRotated ? "rotated" : "")}";
            }
        }

        public Vector2i size { get; set; } // 저장공간 크기
        List<Type> whiteList; //화이트리스트 whiteList.Count() == 0? 전체 허용
        public List<StorageNode> itemList = new List<StorageNode>();    //저장 노드 리스트

        public Storage(Vector2i size, List<Type> whiteList = null)
        {
            this.size = size;
            this.whiteList = whiteList ?? new List<Type>();
        }

        #region [멤버 함수]
        //stackable에 따른 별도의 처리를 생각해두지 않았당...
        //아예 저장 가능, 저장 불가 외에 일부만 저장 가능이라는 경우가 하나 더 생김...
        //내일의 내가 알아서 하자


        //허용된 유형인가?
        public bool IsWhiteList(Item item)
        {
            if (whiteList.Count == 0) { return true; }

            foreach (Type type in whiteList)
            {
                if (type.IsAssignableFrom(item.GetType()))
                {
                    return true;
                }
            }

            return false;
        }

        //임의 위치에 아이템을 저장 가능
        public bool IsAbleToInsert(Item item)
        {
            Vector2i itemSize = item.size;

            // 모든 가능한 위치에서 시도
            for (int i = 0; i <= size.X - itemSize.X; i++)
            {
                for (int j = 0; j <= size.Y - itemSize.Y; j++)
                {
                    StorageNode newNode = new StorageNode
                    {
                        item = item,
                        pos = new Vector2i(i, j),
                        isRotated = false // 일단은 회전 안 시키도록 설정
                    };

                    // 회전을 고려한 newNode를 만들어서 겹치지 않으면 반환
                    if (!IsOverlapped(newNode))
                    {
                        return true;
                    }

                    // 회전해서 시도
                    newNode.isRotated = true;
                    if (!IsOverlapped(newNode))
                    {
                        return true;
                    }
                }
            }

            return false; // 아무 공간도 찾지 못한 경우
        }

        //특정 위치에 아이템을 저장 가능. - StorageNode가 겹쳐도 StorageNode.item과 item이 같다면 무시
        public bool IsAbleToInsert(Item item, Vector2i pos, bool isRotated)
        {
            StorageNode newNode = new StorageNode
            {
                item = item,
                pos = pos,
                isRotated = isRotated
            };

            return !IsOverlapped(newNode);
        }

        //입력된 노드에 따라 아이템을 저장.
        public bool Insert(StorageNode newNode)
        {
            if (IsWhiteList(newNode.item) == false)
                return false;

            if (IsOverlapped(newNode))
                return false;


            if (newNode.item.onStorage != null)
                newNode.item.onStorage.RemoveItem(newNode.item);

            itemList.Add(newNode);
            newNode.item.onStorage = this;


            return true;
        }

        //자동으로 아이템이 들어갈 수 있는 공간을 찾아 반환
        public StorageNode? GetPosInsert(Item item)
        {
            // 모든 가능한 위치에서 시도
            for (int i = 0; i <= size.Y; i++)
            {
                for (int j = 0; j <= size.X; j++)
                {
                    StorageNode newNode = new StorageNode
                    {
                        item = item,
                        pos = new Vector2i(j, i),
                        isRotated = false // 일단은 회전 안 시키도록 설정
                    };

                    // 회전을 고려한 newNode를 만들어서 겹치지 않으면 반환
                    if (!IsOverlapped(newNode))
                    {
                        return newNode;
                    }

                    // 회전해서 시도
                    newNode.isRotated = true;
                    if (!IsOverlapped(newNode))
                    {
                        return newNode;
                    }
                }
            }

            return null; // 아무 공간도 찾지 못한 경우
        }

        //내부에 pos 위치에 아이템이 있는 확인 후 반환
        public StorageNode? GetPosTo(Vector2i pos)
        {
            foreach (var node in itemList)
            {
                Vector2i itemSize = node.isRotated ? new Vector2i(node.item.size.Y, node.item.size.X) : node.item.size;

                // pos가 해당 아이템의 범위 내에 있는지 확인
                if (pos.X >= node.pos.X && pos.X < node.pos.X + itemSize.X &&
                    pos.Y >= node.pos.Y && pos.Y < node.pos.Y + itemSize.Y)
                {
                    return node;
                }
            }

            return null; // 해당 위치에 아무 아이템도 없는 경우
        }

        //인벤토리 내부의 Item을 찾아 제거.
        public bool RemoveItem(Item item)
        {
            foreach (var node in itemList)
            {
                if (node.item == item)
                {
                    itemList.Remove(node);
                    item.onStorage = null;

                    return true;
                }
            }
            return false;
        }

        private bool IsOverlapped(StorageNode newNode)
        {
            Vector2i newOneSize = newNode.isRotated ? new Vector2i(newNode.item.size.Y, newNode.item.size.X) : newNode.item.size;

            // Storage 범위를 벗어나는지 검사
            if (newNode.pos.X < 0 || newNode.pos.Y < 0 || newNode.pos.X + newOneSize.X > size.X || newNode.pos.Y + newOneSize.Y > size.Y)
            {
                return true; // 범위를 벗어남
            }

            foreach (var node in itemList)
            {
                if (CheckOverlap(node, newNode))
                {
                    return true;
                }
            }
            return false;
        }
        private bool CheckOverlap(StorageNode node1, StorageNode node2)
        {
            Vector2i newOneSize = node1.isRotated ? new Vector2i(node1.item.size.Y, node1.item.size.X) : node1.item.size;
            Vector2i oldOneSize = node2.isRotated ? new Vector2i(node2.item.size.Y, node2.item.size.X) : node2.item.size;

            if (node1.pos.X < node2.pos.X + oldOneSize.X &&
                node1.pos.X + newOneSize.X > node2.pos.X &&
                node1.pos.Y < node2.pos.Y + oldOneSize.Y &&
                node1.pos.Y + newOneSize.Y > node2.pos.Y)
            {
                return true; // Overlapping
            }
            return false; // Not overlapping
        }

        #endregion
    }


    internal abstract class Item : IDisposable
    {
        public string spriteName;       //사용할 스프라이트

        public Storage onStorage = null;

        #region [Data]
        public string name; //아이템 이름
        public string description; //아이템 설명
        public float mass;  //질량
        protected Vector2i sizeOrigin;
        public virtual Vector2i size { get => sizeOrigin; set => sizeOrigin = value; } //인벤토리 크기
        public Rarerity rare;   //희귀도
        public float value; //상점 가치

        public DroppedItem droppedItem;
        #endregion

        public Item()
        {
            if (Program.tm != null)
                if (Program.tm.gmNow != null)
                    if (Program.tm.gmNow is GamemodeIngame gm)
                    {
                        gm.items.Add(this);
                        return;
                    }

            Thread thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (Program.tm != null)
                        if (Program.tm.gmNow != null)
                            if (Program.tm.gmNow is GamemodeIngame gm)
                            {
                                gm.items.Add(this);
                                break;
                            }

                    Thread.Sleep(100);
                }
            }));
            thread.Start();
        }

        public virtual void SetupBasicData(string name, string spriteName, string description, float mass, Vector2i size, Rarerity rare, float value) 
        {
            this.name = name;
            this.spriteName = spriteName;
            this.description = description;
            this.mass = mass;
            this.size = size;
            this.rare = rare;
            this.value = value;
        }
        public void DroppedItem(Vector2f pos)
        {
            droppedItem = new DroppedItem(Program.tm.gmNow, pos, this);
        }
        
        public Vector2i itemTexSize => (Vector2i)ResourceManager.textures[spriteName].Size;

        //스토리지에 저장.
        public bool Store(Storage storage)
        {
            if (storage.IsAbleToInsert(this))
            {
                Storage.StorageNode storenode = (Storage.StorageNode)storage.GetPosInsert(this);
                return storage.Insert(storenode);
            }
            return false;
        }
        public bool Store(Storage storage, Vector2i pos, bool isRotated)
        {
            if (storage.IsAbleToInsert(this))
            {
                Storage.StorageNode storenode = new Storage.StorageNode() { item = this, pos = pos, isRotated = isRotated };
                return storage.Insert(storenode);
            }
            return false;
        }

        ~Item () { Dispose(); }
        public virtual void Dispose()
        {
            droppedItem?.Dispose();

            if (Program.tm.gmNow is GamemodeIngame gm)
                gm.items.Remove(this);

            //onStorage에서 제거
            if (onStorage != null)
                onStorage.RemoveItem(this);

            GC.SuppressFinalize(this);
        }
    }


    #region [아이템 인터페이스]
    //스택 가능한가?
    internal interface IStackable
    {
        int stackNow { get; set; }
        int stackMax { get; set; }
    }

    //내구도를 가지는가?
    internal interface IDurable
    {
        float durableNow { get; set; }
        float durableMax { get; set; }
        bool zeroToDestruct { get; set; }
    }

    //Humanoid의 손에 장비될 수 있는가?
    internal interface IHandable
    {
        //장비 소요 속도, 장비 진행도
        float equipTime { get; }
        float equipValue { get; }


        void DrawHandable(RenderTexture texture, Vector2f position, float direction, Vector2f scaleRatio, RenderStates renderStates);

        Dictionary<InputManager.CommandType, Action<Humanoid.Hands, bool>> commandsReact { get; set; }
    }


    //Humanoid가 인벤토리 상에서 사용 가능.
    internal interface IClickable { }

    #endregion

    //internal abstract class ItemStackable : Item
    //{
    //    public int maxStack;   //스택 최대치
    //    public int nowStack;   //스택 저장량

    //    public ItemStackable() : base() { }

    //    //다른 스택가능한 아이템에 포함될 것임...
    //    ItemStackable BeAddStack(ItemStackable other)
    //    {
    //        //서로의 형이 다름.
    //        if (other.GetType() != this.GetType()) { return other; }

    //        int ableStack = maxStack - nowStack;
    //        if (ableStack >= other.nowStack)
    //        {
    //            nowStack += other.nowStack;
    //            return null;
    //        }
    //        nowStack = maxStack;
    //        other.nowStack -= ableStack;
    //        return other;
    //    }
    //    public void DoAddStack(ItemStackable target)
    //    {
    //        ItemStackable result = target.BeAddStack(this);
    //        if (result == null)
    //        {
    //            Dispose();
    //        }
    //    }

    //    public ItemStackable GetHalp()
    //    {
    //        if (nowStack == 1)
    //        {
    //            return null;
    //        }

    //        ItemStackable instance = Activator.CreateInstance(GetType()) as ItemStackable;
    //        instance.nowStack = nowStack / 2;
    //        nowStack -= instance.nowStack;

    //        return instance;
    //    }
    //}

    //internal abstract class ItemDurable : Item
    //{
    //    //내구도 정보
    //    public float durableMax;    //스택 최대치
    //    public float durableNow;    //스택 저장량
    //    public bool zeroToDestruct; //내구도 0이 되면 파괴

    //    //내구도 변화
    //    public void DurableDelta(float delta)
    //    {
    //        durableNow += delta;
    //        durableNow = Mathf.Clamp(durableNow, 0f, durableMax);

    //        if (zeroToDestruct == true && durableNow <= 0.0001f)
    //        {
    //            Dispose();
    //        }
    //    }
    //}



}
