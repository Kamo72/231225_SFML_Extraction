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
            if (equipedBy is Humanoid human)
            {

            }
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
    internal abstract class Headgear : Equipable { }

    //플레이트 캐리어
    internal class PlateCarrier : Equipable
    {
        public int plateLevel;  //장착 가능한 장갑판 등급
    }

    //가방
    internal class Backpack : Equipable
    {
        public Storage storage;
    }

    //헬멧
    internal class Helmet : Equipable { }

    //장갑판
    internal class ArmourPlate : Item { }

}
