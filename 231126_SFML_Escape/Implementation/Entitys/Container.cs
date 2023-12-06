using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal class Container : Entity, IInteractable
    {

        //창고
        public Storage storage;

        //상자 열기 제어
        public bool isOpen = false;
        public Humanoid openBy = null;

        public Container(Gamemode gamemode, Vector2f position, ICollision collision) : base(gamemode, position, collision)
        {
        }

        //상자 열기
        public bool Open(Humanoid entity)
        {
            if (isOpen == false)
            {
                isOpen = true;
                openBy = entity;
                return true;
            }
            return false;
        }
        //상자 닫기
        public void Close()
        {
            if (isOpen == false)
            {
                Console.WriteLine("Container - Close 열려있지 않은 컨터이너를 닫으려고 시도");
                return;
            }

            isOpen = false;
            openBy = null;
        }


        public bool IsInteractable(Humanoid caster)
        {
            return true;
        }

        public void BeInteract(Humanoid caster)
        {
            Open(caster);
            //Close는 Humanoid측에서 진행
        }


        protected override void LogicProcess()
        {
            throw new NotImplementedException();
        }

        protected override void PhysicsProcess()
        {
            throw new NotImplementedException();
        }

        protected override void DrawProcess()
        {
            throw new NotImplementedException();
        }

    }

}
