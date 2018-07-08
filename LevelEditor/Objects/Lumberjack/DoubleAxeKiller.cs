using System;
using LevelEditor.Engine.Mesh;
using Microsoft.Xna.Framework;
using LevelEditor.Collision;
using LevelEditor.Engine;
using LevelEditor.Engine.Helper;
using LevelEditor.Objects.Ape;
using System.Collections.Generic;

namespace LevelEditor.Objects.Lumberjack
{
    class DoubleAxeKiller : IMoveable, IAttacker, IEscape, ICollide, IActor, IGameObject
    {
        public int HP { get; set; }
        public Hut Hut { get; set; }
        public bool IsApe { get; set; }
        public bool IsAlive { get; set; }

        private static Random mRandom = new Random(10);
        private Scene mScene;
        private int mDamage;
        private int mMaxHP;
        private int mRange;
        private int mCoolDown;
        private int mTimeElapsed;
        // private int mCoolDown;

        private const float Speed = 0.0016f;
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

        public DoubleAxeKiller(Mesh m, Terrain terrain, QuadTree<Actor> quadTree, Vector3 hutPosition, float innerRange, float outerRange)
        {

            IsApe = false;

            mQuadTree = quadTree;
            IsAlive = true;
            HP = 350;
            mMaxHP = HP;
            mDamage = 10;
            mCoolDown = 2;
            Actor = new Actor(m, this);
            Actor.mAnimator.SetStandardAnimation("idle");
            mTerrain = terrain;
			mIsAttacked = false;
			mTimeHighlighted = 0;
            mHutPosition = hutPosition;
            mInnerRange = innerRange;
            mOuterRange = outerRange;
            Actor.ModelMatrix = Matrix.CreateTranslation(GetOffsetInRange(innerRange, outerRange) + hutPosition);

            mAttacking = false;
            IsMoving = false;
        }

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

        public bool IsMoving { get; set; }

        public void Move()
        {
            throw new NotImplementedException();
        }


        public void Attack()
        {
            foreach (var elem in mInRangeActors)
            {
                if (elem == Actor)// || elem.Data.mMesh.mMeshData.mIsTerrain)
                {
                    continue;
                }
                else
                {
                    var actorToEnemy = elem.ModelMatrix.Translation - Actor.ModelMatrix.Translation;
                    var actor2Enemy2D = new Vector2(actorToEnemy.X, actorToEnemy.Z);

                    if (mClosestFiend == null)
                    {

                        mClosestFiend = elem;
                        mClosestAngle = Vector2.Dot(actor2Enemy2D, mCurrentDirection);
                    }
                    else
                    {

                        if (elem.IActor is IAttacker)
                        {
                            var entity = (IAttacker)elem.IActor;
                            if (entity.IsApe == true)
                            {
                                mIsAttacking = true;
                                Actor.mAnimator.PlayAnimation("hit", true, 500);
                                var currentAngle = Vector2.Dot(actor2Enemy2D, mCurrentDirection);
                                if (mClosestFiend == null || currentAngle < mClosestAngle)
                                {
                                    mClosestAngle = currentAngle;
                                    mClosestFiend = elem;
                                }
                            }
                             
                            
                        }
                    }
                }
            }

            if (mClosestFiend != null && mClosestFiend.IActor is IAttacker)
            {
                
                var enemy = (IAttacker)mClosestFiend.IActor;
                if (enemy.IsApe == true)
                {
                    enemy.Attacked(mDamage);
                }
            }
            else
            {
                Actor.mAnimator.PlayAnimation("Armature.001Action", true, 500);
            }
        }

        public void Attacked(int damage)
        {
                Actor.Color = new Vector3(1.0f, 0.0f, 0.0f);
                mIsAttacked = true;
                if (HP - damage <= mMaxHP & HP > 0)
                {
                    HP -= damage;
                }
                else if (damage < 0)
                {
                    HP = mMaxHP;
                }
                else {
                    //remove later
                    Escape();
                }
        }

        public void Escape()
        {
                Hut?.RemoveDoubleAxeKiller(this);
        }

        public void Collide()
        {
            throw new NotImplementedException();
        }

        public Actor Actor { get; }
        public void Update(GameTime gameTime)
        {

            if (mIsAttacked)
            {
                mTimeHighlighted += gameTime.ElapsedGameTime.Milliseconds;
                if (mTimeHighlighted > mHighlightTime)
                {
                    mTimeHighlighted = 0;
                    mIsAttacked = false;
                    Actor.Color = new Vector3(1.0f, 1.0f, 1.0f);
                }
            }
            if (IsMoving)
            {

                Actor.mAnimator.PlayAnimation("Armature.001Action", true, 500);

                var previousLocation = Actor.ModelMatrix.Translation;

                mCurrentDirection = MathExtension.Mix(mCurrentDirection, mDirection, 3.0f * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                var directVector3 = new Vector3(mCurrentDirection.X, 0.0f, mCurrentDirection.Y);
                directVector3.Normalize();
                var vec = Speed * directVector3 * gameTime.ElapsedGameTime.Milliseconds + Actor.ModelMatrix.Translation;
                vec.Y = mTerrain.GetHeight(vec);

                mDirection = Vector2.Normalize(new Vector2(mTargetPosition.X - vec.X, mTargetPosition.Z - vec.Z));

                var rotationMatrix = Matrix.CreateLookAt(Vector3.Zero, -new Vector3(directVector3.X, 0.0f, -directVector3.Z), Vector3.Up);

                // IMPORTANT: Remove before moving the actor (this also solves self-collision)
                mQuadTree.Remove(Actor, Actor.mBoundingRectangle.GetAxisAlignedRectangle(1));
                Actor.ModelMatrix = rotationMatrix * Matrix.CreateTranslation(vec);

                var currentDirection = new Vector2
                {
                    X = mTargetPosition.X - Actor.ModelMatrix.Translation.X,
                    Y = mTargetPosition.Z - Actor.ModelMatrix.Translation.Z
                };
                currentDirection.Normalize();

                var boundingRect = Actor.mBoundingRectangle.GetAxisAlignedRectangle(1);
                var actorList = mQuadTree.QueryRectangle(boundingRect);
                var collision = false;

                foreach (var actor in actorList)
                {

                    if (actor.mBoundingRectangle.IntersectsRectangle(Actor.mBoundingRectangle, true))
                    {
                        actor.Collision = true;
                        collision = true;
                    }
                    else
                    {
                        actor.Collision = false;
                    }

                    actor.QuadTree = true;

                }

                if (0 > Vector2.Dot(currentDirection, mPlannedDirection) || collision)
                {

                    var quat = Actor.ModelMatrix.Rotation;

                    // Do something here
                    if (!collision)
                    {
                        if (mInRangeMovement)
                        {
                            StayInRange();
                        }
                        else
                        {
                            Actor.ModelMatrix = Matrix.CreateFromQuaternion(quat) * Matrix.CreateTranslation(mTargetPosition);
                        }
                    }
                    else
                    {

                        Actor.ModelMatrix = Matrix.CreateFromQuaternion(quat) * Matrix.CreateTranslation(previousLocation);

                        if (mInRangeMovement)
                        {
                            SetTarget(previousLocation + Vector3.Normalize(Actor.ModelMatrix.Right + Actor.ModelMatrix.Backward * 2), true);
                        }                        

                    }     
                    
                    if (!mInRangeMovement)
                    {
                        IsMoving = false;
                        mDirection = new Vector2(0.0f);
                    }

                }

                mQuadTree.Insert(Actor, Actor.mBoundingRectangle.GetAxisAlignedRectangle(1));

            }
            else
            {

                Actor.mAnimator.PlayAnimation("idle", true, 500);

            }
        }

        public void SetTarget(Vector3 target)
        {

            SetTarget(target, false);

        }

        public void SetTargetRange(Vector3 target, float range)
        {

            var x = 2.0f * (float)mRandom.NextDouble() - 1.0f;
            var z = 2.0f * (float)mRandom.NextDouble() - 1.0f;
            var offset = new Vector3(x, 0.0f, z);
            offset.Normalize();
            offset *= range * (float)mRandom.NextDouble();

            SetTarget(target + offset, false);

        }

        public void StayInRange()
        {

            mInRangeMovement = true;

            var vector = GetOffsetInRange(mInnerRange, mOuterRange) + mHutPosition;
            vector.Y = mTerrain.GetHeight(vector);

            SetTarget(vector, true);

        }

        private void SetTarget(Vector3 target, bool inRangeMovement)
        {

            mTargetPosition = target;
            mTargetPosition.Y = mTerrain.GetHeight(target);
            mDirection = new Vector2(mTargetPosition.X, mTargetPosition.Z) - new Vector2(Actor.ModelMatrix.Translation.X, Actor.ModelMatrix.Translation.Z);
            mDirection.Normalize();
            mPlannedDirection = new Vector2(mDirection.X, mDirection.Y);
            IsMoving = true;
            mInRangeMovement = inRangeMovement;

        }

        private Vector3 GetOffsetInRange(float innerRange, float outerRange)
        {
            var multiplier = (outerRange - innerRange) / outerRange;
            var offset = 1.0f - multiplier;
            var x = 2.0f * (float)mRandom.NextDouble() - 1.0f;
            var z = 2.0f * (float)mRandom.NextDouble() - 1.0f;
            var vector = new Vector3(x, 0.0f, z);
            vector.Normalize();
            vector *= ((float)mRandom.NextDouble() * multiplier + offset) * outerRange;
            return vector;
        }

    }

}
