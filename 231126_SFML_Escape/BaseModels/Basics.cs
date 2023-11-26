using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal abstract class Entity : IDisposable
    {
        Gamemode gamemode;

        public Entity(Gamemode gamemode, Vector2f position, ICollision collision)
        {
            mask = collision;
            Position = position;
            Direction = 0f;
            
            this.gamemode = gamemode;
            gamemode.DisposablesAdd(this);
            gamemode.logicEvent += LogicProcess;
            gamemode.logicEvent += PhysicsProcess;
            gamemode.drawEvent += DrawProcess;
        }

        public float Direction;
        public Vector2f Position { 
            set 
            {
                if (mask is Box box)
                    box.Position = value;
                else if(mask is Circle circle)
                    circle.Position = value;
                else if(mask is Point point)
                    point.position = value;
                else
                    throw new NotImplementedException();
            }
            get 
            {
                if (mask is Box box)
                    return box.Position;
                else if (mask is Circle circle)
                    return circle.Position;
                else if (mask is Point point)
                    return point.position;
                else
                    throw new NotImplementedException();
            } 
        }

        public ICollision mask;

        protected abstract void LogicProcess();
        protected abstract void PhysicsProcess();
        protected abstract void DrawProcess();

        ~Entity() { Dispose(); }
        public void Dispose()
        {
            gamemode.DisposablesRemove(this);
            gamemode.logicEvent -= LogicProcess;
            gamemode.logicEvent -= PhysicsProcess;
            gamemode.drawEvent -= DrawProcess;
            GC.SuppressFinalize(this);
        }
    }

    internal abstract class Ui : RectangleShape
    {
        Gamemode gamemode;

        public Ui(Gamemode gamemode, Vector2f position, Vector2f size)
        {
            Position = position;
            Size = size;

            this.gamemode = gamemode;
            gamemode.DisposablesAdd(this);
            gamemode.logicEvent += LogicProcess;
            gamemode.logicEvent += ClickProcess;
            gamemode.drawEvent += DrawProcess;
        }

        public bool IsMouseOn()
        {
            Vector2f mousePos = (Vector2f)Mouse.GetPosition();
            FloatRect floatRect = this.GetGlobalBounds();
            bool IsOn = floatRect.Contains(mousePos.X, mousePos.Y);
            return IsOn;
        }

        protected abstract void DrawProcess();
        protected abstract void LogicProcess();

        bool pressedBefore = false;

        void ClickProcess()
        {
            if (pressedBefore == false)
                if (IsMouseOn())
                    if (Mouse.IsButtonPressed(Mouse.Button.Left))
                    {
                        Clicked?.Invoke();
                        pressedBefore = true;
                        return;
                    }
                
            pressedBefore = Mouse.IsButtonPressed(Mouse.Button.Left);
        }
        public event Action Clicked;

        ~Ui() { Dispose(); }

        public new void Dispose()
        {
            gamemode.DisposablesRemove(this);
            gamemode.logicEvent -= LogicProcess;
            gamemode.logicEvent -= ClickProcess;
            gamemode.drawEvent -= DrawProcess;

            base.Dispose();
            GC.SuppressFinalize(this);
        }

    }

    internal abstract class Particle : IDisposable
    {
        Gamemode gamemode;
        public Particle(Gamemode gamemode, int lifeTime, Vector2f position, Vector2f scale, float rotation = 0f) 
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;

            lifeMax = lifeTime;
            lifeNow = lifeTime;

            this.gamemode = gamemode;
            gamemode.DisposablesAdd(this);
            gamemode.drawEvent += DrawProcess;
            gamemode.logicEvent += LogicProcess;
            gamemode.logicEvent += LifeProcess;
        }

        public int lifeMax, lifeNow;
        public float lifeRatio { get { return (float)lifeMax / lifeNow; } }
        
        public Vector2f position;
        public Vector2f scale;
        public float rotation;

        void LifeProcess() 
        {
            lifeNow--;
            if(lifeNow == 0) Dispose();
        }

        public abstract void DrawProcess();
        public abstract void LogicProcess();


        ~Particle() { Dispose(); }
        public void Dispose()
        {
            gamemode.DisposablesRemove(this);
            gamemode.drawEvent -= DrawProcess;
            gamemode.logicEvent -= LogicProcess;
            gamemode.logicEvent -= LifeProcess;

            GC.SuppressFinalize(this);
        }
    }

    internal abstract class Projectile : IDisposable
    {
        Gamemode gamemode;
        public Projectile(Gamemode gamemode, int lifeTime, ICollision mask, Vector2f position, float rotation = 0f, float speed = 0f)
        {
            this.mask = mask;
            this.position = position;
            this.rotation = rotation;
            this.speed = rotation.ToVector() * speed;

            lifeMax = lifeTime;
            lifeNow = lifeTime;

            this.gamemode = gamemode;
            gamemode.DisposablesAdd(this);
            gamemode.drawEvent += DrawProcess;
            gamemode.logicEvent += LogicProcess;
            gamemode.logicEvent += PhysicProcess;
            gamemode.logicEvent += LifeProcess;
        }

        public int lifeMax, lifeNow;
        public float lifeRatio { get { return (float)lifeMax / lifeNow; } }

        public Vector2f position
        {
            set
            {
                if (mask is Box box)
                    box.Position = value;
                else if (mask is Circle circle)
                    circle.Position = value;
                else if (mask is Point point)
                    point.position = value;
                else
                    throw new NotImplementedException();
            }
            get
            {
                if (mask is Box box)
                    return box.Position;
                else if (mask is Circle circle)
                    return circle.Position;
                else if (mask is Point point)
                    return point.position;
                else
                    throw new NotImplementedException();
            }
        }
        public float rotation;
        public Vector2f speed;

        public ICollision mask;

        void LifeProcess()
        {
            lifeNow--;
            if (lifeNow == 0) Dispose();
        }
        void PhysicProcess()
        {
            float deltaTime = 1000f / (float)gamemode.logicFps;
            position += speed * (float)deltaTime;
        }

        public abstract void DrawProcess();
        public abstract void LogicProcess();


        ~Projectile(){ Dispose(); }
        public void Dispose()
        {
            gamemode.DisposablesRemove(this);
            gamemode.drawEvent -= DrawProcess;
            gamemode.logicEvent -= LogicProcess;
            gamemode.logicEvent -= PhysicProcess;
            gamemode.logicEvent -= LifeProcess;

            GC.SuppressFinalize(this);
        }
    }

    internal abstract class Structure : IDisposable
    {
        Gamemode gamemode;
        public Structure(Gamemode gamemode, Vector2f position, ICollision collision)
        {
            mask = collision;
            this.position = position;

            this.gamemode = gamemode;
            gamemode.DisposablesAdd(this);
            gamemode.drawEvent += DrawProcess;
        }

        public ICollision mask;
        public Vector2f position
        {
            set
            {
                if (mask is Box box)
                    box.Position = value;
                else if (mask is Circle circle)
                    circle.Position = value;
                else if (mask is Point point)
                    point.position = value;
                else
                    throw new NotImplementedException();
            }
            get
            {
                if (mask is Box box)
                    return box.Position;
                else if (mask is Circle circle)
                    return circle.Position;
                else if (mask is Point point)
                    return point.position;
                else
                    throw new NotImplementedException();
            }
        }

        protected abstract void DrawProcess();


        ~Structure() { Dispose(); }
        public void Dispose()
        {
            gamemode.DisposablesRemove(this);
            gamemode.drawEvent -= DrawProcess;

            GC.SuppressFinalize(this);
        }
    }

}
