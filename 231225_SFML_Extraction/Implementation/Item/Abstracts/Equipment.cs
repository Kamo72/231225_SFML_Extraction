using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{

    //장비 가능한 아이템
    internal abstract class Equipable : Item, IEquipable
    {

        public Equipable() : base() { }

        public EquipmentType equipmentType { get; set; }
        public Humanoid equipedBy { get; set; }

        //장착 및 장착 해제
        public void BeEquip(Humanoid entity)
        {
            if (onStorage != null)
            {
                onStorage.RemoveItem(this);
            }

            equipedBy = entity;
        }
        public void UnEquip(Humanoid entity)
        {
            if (equipedBy == null) throw new Exception("Equipable - UnEquip - ERROR : 장착하지 않은 아이템을 장착 해제하려고 합니다!");
            if (equipedBy is Humanoid human)
            {

            }
            equipedBy = null;
        }
    }

    public enum EquipmentType 
    {
        HEADGEAR,
        PLATE_CARRIER,
        BACKPACK,
        HELMET,
        WEAPON,
    }

    internal interface IEquipable
    {
        Humanoid equipedBy { get; set; }
        EquipmentType equipmentType { get; set; }
        void BeEquip(Humanoid entity);
        void UnEquip(Humanoid entity);
    }


    //헤드기어 - 중간 조상
    internal abstract class Headgear : Equipable
    {
        public Headgear() : base() { }

    }

    //플레이트 캐리어
    internal class PlateCarrier : Equipable
    {
        public PlateCarrier(float equipablePlateLevel) : base()
        {
            this.equipablePlateLevel = equipablePlateLevel;
        }

        public float equipablePlateLevel;  //장착 가능한 장갑판 등급
        public ArmourPlate armourPlate = null;
    }

    //가방
    internal class Backpack : Equipable
    {
        public Backpack(Vector2i storageSize) : base()
        {
            storage = new Storage(storageSize);
        }
        public Storage storage;
    }

    //헬멧
    internal class Helmet : Equipable
    {
        public Helmet(float armourLevel) : base()
        {
            this.armourLevel = armourLevel;
        }

        public float armourLevel;
    }

    //장갑판
    internal class ArmourPlate : Item
    {
        public ArmourPlate(float armourLevel) : base()
        {
            this.armourLevel = armourLevel;
        }

        public float armourLevel;
    }

}
