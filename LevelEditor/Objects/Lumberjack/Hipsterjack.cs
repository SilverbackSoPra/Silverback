using System;
using LevelEditor.Engine.Mesh;
using Microsoft.Xna.Framework;

namespace LevelEditor.Objects.Lumberjack
{
    class Hipsterjack : IMoveable, IAttacker, IEscape, ICollide, IActor, IGameObject
    {

        private int mHealthPoints;
        private int mDamage;
        private int mCoolDown;

        public Hipsterjack()
        {
            mHealthPoints = 300;
            mDamage = 0;
            mCoolDown = 0;
        }


        public string Name { get; set; }
        public bool HasSoundEmitter { get; set; }

        public void Attack()
        {
            throw new NotImplementedException();
        }

        public void Attacked(int damage)
        {
            mHealthPoints -= damage;
        }

        public void Collide()
        {
            throw new NotImplementedException();
        }

        public void Escape()
        {
            throw new NotImplementedException();
        }

        public bool IsMoving { get; set; }

        public void Move()
        {
            throw new NotImplementedException();
        }

        public void SetTarget(Vector3 target)
        {
        }

        public Actor Actor { get; }
        public void Update(GameTime gameTime)
        {
        }

        public string Save()
        {
            return "";
        }

        public bool Load()
        {
            return false;
        }
    }
}
