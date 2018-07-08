using System;
using LevelEditor.Engine.Mesh;
using Microsoft.Xna.Framework;

namespace LevelEditor.Objects.Lumberjack
{
    class ChainsawMurderer : IMoveable, IAttacker, IEscape, ICollide, IActor, IGameObject
    {

        private int mHealthPoints;
        private int mDamage;
        private int mCoolDown;

        public ChainsawMurderer(Actor actor)
        {
            Actor = actor;
            mHealthPoints = 100;
            mDamage = 20;
            mCoolDown = 5;
        }

        public string Name { get; set; }
        public bool HasSoundEmitter { get; set; }

        public bool IsMoving { get; set; }

        public void Move()
        {
            throw new NotImplementedException();
        }

        public void SetTarget(Vector3 target)
        {
        }

        public void Attack()
        {
            throw new NotImplementedException();
        }

        public void Attacked(int damage)
        {
            mHealthPoints -= damage;
        }

        public void Escape()
        {
            throw new NotImplementedException();
        }

        public void Collide()
        {
            throw new NotImplementedException();
        }

        public Actor Actor { get; }
        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
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
