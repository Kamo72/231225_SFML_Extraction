using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
 
using System.Xml.Linq;

namespace _231109_SFML_Test
{
    #region [부착물 및 피부착물]
    public enum AttachmentType
    {
        BARREL,     //총열
        HANDGUARD,  //총열 덮개
        PISTOL_GRIP,    //권총손잡이
        CHARGING_HANDLE,    //장전 손잡이
        TOP_RECIEVER,       //상부 리시버

        MUZZLE, //총구
        STOCK,  //개머리판

        SMALL_OPTIC,    //소형 조준경 슬롯
        MIDDLE_OPTIC,   //중형 조준경 슬롯
        BIG_OPTIC,      //대형 조준경 슬롯
        TACTICAL_GEAR,  //전술 장비
        FOREGRIP,       //전방 손잡이
    }

    //부착물을 부착 가능한 객체
    internal interface IAttachable
    {
        //부착물 리스트
        List<AttachSocket> attachments { get; set; }
    }

    //부착물
    internal interface IAttachment
    {
        IAttachable attachedBy { get; set; }
        AttachmentType attachmentType { get; set; }

        bool IsAttachable(IAttachable iAttachable);
        bool DoAttach(IAttachable iAttachable, AttachSocket socket);
        bool DoAttach(IAttachable iAttachable);
        bool DoDettach();
    }

    internal struct AttachSocket
    {
        public AttachSocket(AttachmentType attachmentType, Vector2f attachPos, bool isNeccesary = false, List<Type> whiteList = null, IAttachment attachment = null)
        {
            this.attachmentType = attachmentType;
            this.attachPos = attachPos;
            this.isNeccesary = isNeccesary;
            this.whiteList = whiteList ?? new List<Type>();
            this.attachment = attachment;
        }

        /// <summary>
        /// 부착물 유형
        /// </summary>
        public AttachmentType attachmentType;

        /// <summary>
        /// 격발을 위한 필수 부착물인지?
        /// </summary>
        public bool isNeccesary;

        /// <summary>
        /// 부착물 위치
        /// </summary>
        public Vector2f attachPos;

        /// <summary>
        /// 장착 가능한 화이트리스트
        /// </summary>
        public List<Type> whiteList;

        /// <summary>
        /// 장착된 부착물
        /// </summary>
        public IAttachment attachment;
    }

    #endregion

    internal abstract class Attachment : Item, IAttachment
    {
        public Attachment(AttachmentType attachmentType, List<WeaponAdjust> weaponAdjusts = null) : base()
        {
            this.weaponAdjusts = weaponAdjusts;
            this.attachmentType = attachmentType;
            attachedBy = null;
        }

        //무기 보정 옵션들
        public List<WeaponAdjust> weaponAdjusts;
        
        //현재 부착된 총기를 Get
        public Weapon attachedWeapon
        {
            get
            {
                Weapon GetRootWeapon(IAttachable from)
                {
                    //인자가 총기라면 총기 반환
                    if (from is Weapon w)
                        return w;

                    //인자가 중간 부착물이라면 한칸 더 올라감
                    if (from is IAttachment iAtc)
                        return GetRootWeapon(iAtc.attachedBy);

                    //중간 부착물도 아니라면 부착된 총기 없음.
                    return null;
                }

                if (attachedBy == null) return null;

                return GetRootWeapon(attachedBy);
            }
        }

        //인벤토리 칸 보정
        public (int top, int bottom, int left, int right) sizeAdjust = (0, 0, 0, 0); //인벤토리 내 크기 정렬


        public AttachmentType attachmentType { get; set; }
        public IAttachable attachedBy { get; set; }


        #region [탈부착 함수들]
        bool AttachableVilidation(IAttachable attachable) 
        {
            Item item = attachable as Item;

            //아이템이 창고 안에 있다면 창고 안에서 부착 가능한지?
            if (item.onStorage != null) 
            {
                //부착 대상의 스토리지 노드를 확보.
                Storage stor = item.onStorage;
                Storage.StorageNode resnode = item.onStorage.itemList.Find(node => node.item == item);

                //유효한 스토리지 노드인지 확인 (찾지못했다면 기본값 > 기본값이라면 item = null임.)
                if (resnode.item != item || resnode.item == null) throw new Exception("Critical Error. AttachableVilidation / if(resnode.item == null) ");
                
                //증축할 길이
                int extensionLen;


                //상부 체크
                extensionLen = resnode.isRotated ? sizeAdjust.top : sizeAdjust.right;
                {
                    //창고 크기를 벗어남.
                    if (resnode.pos.Y - extensionLen < 0) return false;

                    //윗변 X에서 (Y - 1) ~ (Y - extensionLen)
                    for (int x = resnode.pos.X; x < resnode.pos.X + resnode.item.size.X - 1; x++)
                        for (int y = resnode.pos.Y - 1; y >= resnode.pos.Y - extensionLen; y--)
                            if (stor.GetPosTo(new Vector2i(x, y)) != null) return false;
                }

                //좌측 체크
                extensionLen = resnode.isRotated ? sizeAdjust.left : sizeAdjust.top;
                {
                    //창고 크기를 벗어남.
                    if (resnode.pos.X + resnode.item.size.X - 1 + extensionLen > stor.size.X) return false;

                    //Y축에서 (X + 1) ~ (X  + item.x - 1 + extensionLen)
                    for (int y = resnode.pos.Y; y < resnode.pos.Y + resnode.item.size.Y - 1; y++)
                        for (int x = resnode.pos.X + 1; x < resnode.pos.X + resnode.item.size.X - 1 + extensionLen; x--)
                            if (stor.GetPosTo(new Vector2i(x, y)) != null) return false;
                }

                //하부 체크
                extensionLen = resnode.isRotated ? sizeAdjust.bottom : sizeAdjust.left;
                {
                    //창고 크기를 벗어남.
                    if (resnode.pos.Y + resnode.item.size.Y - 1 + extensionLen > stor.size.Y) return false;

                    //윗변 X에서 (Y + 1) ~ (Y  + item.Y - 1 + extensionLen)
                    for (int x = resnode.pos.X; x < resnode.pos.X + resnode.item.size.X - 1; x++)
                        for (int y = resnode.pos.Y + 1; y  <= resnode.pos.Y + resnode.item.size.Y - 1 + extensionLen; y--)
                            if (stor.GetPosTo(new Vector2i(x, y)) != null) return false;
                }

                //우측 체크
                extensionLen = resnode.isRotated ? sizeAdjust.right : sizeAdjust.bottom;
                {
                    //창고 크기를 벗어남.
                    if (resnode.pos.X - extensionLen < 0) return false;

                    //Y축에서 (X - 1) ~ (X - extensionLen)
                    for (int y = resnode.pos.Y; y < resnode.pos.Y + resnode.item.size.Y - 1; y++)
                        for (int x = resnode.pos.X - 1; x >= resnode.pos.X - extensionLen; x--)
                            if (stor.GetPosTo(new Vector2i(x, y)) != null) return false;
                }


            }

            return true;

        }
        bool AttachSocketVilidation(AttachSocket socket)
        {
            //이미 다른 곳에 부착되어 있는가?
            if (attachedBy != null) return false;

            //해당 소켓 안에 부착물이 있나?
            if (socket.attachment != null) return false;

            //소켓과 부착물의 유형이 맞는가?
            if (socket.attachmentType != attachmentType) return false;

            //화이트 리스트가 적용됐는데, 허용 대상이 맞는가?
            if (socket.whiteList.Count != 0 && socket.whiteList.Contains(this.GetType()) == false) return false;

            return true;
        }


        public bool DoAttach(IAttachable iAttachable, AttachSocket socket)
        {
            //소켓 검사
            if(AttachSocketVilidation(socket) == false) return false;

            //같이 주어진 요소의 인덱스 얻기
            int index = iAttachable.attachments.IndexOf(socket);

            //부착 수행
            socket.attachment = this;
            iAttachable.attachments[index] = socket;
            attachedBy = iAttachable;

            return true;
        }
        public bool DoAttach(IAttachable iAttachable)
        {
            //피부착물 검사
            if (AttachableVilidation(iAttachable) == false) return false;

            //모든 소켓에 부착을 시도
            foreach (var socket in iAttachable.attachments)
                if (DoAttach(iAttachable, socket) == true) return true;

            return false;
        }
        public bool DoDettach()
        {
            //애초에 부착되어는 있는가?
            if (attachedBy == null) return false;
            
            //인덱스를 얻어온다.
            int index = attachedBy.attachments.FindIndex((item) => item.attachment == this);

            //부착된 위치를 찾지 못했습니다.
            if(index == -1) return false;

            //탈착 수행.
            AttachSocket ats = attachedBy.attachments[index];
            ats.attachment = null;
            attachedBy.attachments[index] = ats;
            

            return true;
        }


        public bool IsAttachable(AttachSocket socket)
        {
            //소켓 검사
            if (AttachSocketVilidation(socket) == false) return false;

            return true;
        }
        public bool IsAttachable(IAttachable iAttachable)
        {
            //피부착물 검사
            if (AttachableVilidation(iAttachable) == false) return false;

            //모든 소켓 순회 검사
            foreach (var socket in iAttachable.attachments)
                if(IsAttachable(socket) == true) return true;
            

            return false;
        }
        #endregion
    }


}
