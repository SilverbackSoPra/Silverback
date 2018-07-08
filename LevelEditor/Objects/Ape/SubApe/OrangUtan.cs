using System;
using LevelEditor.Engine.Mesh;
using Microsoft.Xna.Framework;
using LevelEditor.Engine;
using System.Collections.Generic;
using LevelEditor.Collision;

namespace LevelEditor.Objects.Ape.SubApe
{
    class OrangUtan : IMoveable, IAttacker, IEscape, ICollide, IActor, IGameObject
    {
        public int HP { get; set; }
        public bool IsApe { get; set; }
        public bool IsAlive { get; set; }

        private static Random mRandom = new Random();
        private Scene mScene;
        private int mDamage;
        private int mMaxHP;
        private int mRange;
        private int mCoolDown;
        private int mTimeElapsed;
        // private int mCoolDown;

        private const float Speed = 0.0040f;
        private const int MaxWalkingRangeValue = 128;
        private Vector3 mTargetPosition;
        private Vector2 mDirection;
        private Vector2 mPlannedDirection;
        private Vector2 mCurrentDirection;

        private readonly Terrain mTerrain;
        private readonly Silverback mSilverback;
        private bool mIsSilverbackTarget;
        private Vector3 mLastSilverbackLocation;
        private int mTargetCoolDown;
        private Actor mClosestFiend;
        private float mClosestAngle;
        private List<Actor> mInRangeActors;
        private bool mIsAttacking;

        private ActorBatch mActorBatch;

        private QuadTree<Actor> mQuadTree;
        private bool mIsAlive;

        private bool mIsAttacked;
        private const int mHighlightTime = 300;
        private int mTimeHighlighted;


        private bool mInRangeMovement = false;
        private float mInnerRange;
        private float mOuterRange;
        private Vector3 mHutPosition;

        private bool mAttacking;

        public OrangUtan(Mesh m)
        {
            IsApe = true;

            HP = 650;
            mDamage = 10;
            IsAlive = true;
            Actor = new Actor(m, this);
            mIsAttacking = false;
        }

        public void Attack()
        {
            if (mIsAttacking)
            {
                mIsAttacking = false;
                Actor.mAnimator.PlayAnimation("idle", true, 500);
            }
            else
            {
                mIsAttacking = true;
                Actor.mAnimator.PlayAnimation("throw", true, 500);
            }
        }

        public void Attacked(int damage)
        {
            if (HP - damage <= mMaxHP & HP > 0)
            {
                HP -= damage;
            }
            else
            {
                HP = mMaxHP;
            }
        }

        public void Collide()
        { }

        public void Escape()
        { }

        public bool IsMoving { get; set; }

        public void Move()
        { }

        public void SetTarget(Vector3 target)
        { }

        public Actor Actor { get; }
        public void Update(GameTime gameTime)
        { }

        public string Name { get; set; }
        public bool HasSoundEmitter { get; set; }
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
