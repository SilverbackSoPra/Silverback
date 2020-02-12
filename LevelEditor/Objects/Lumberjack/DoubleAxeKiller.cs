using System;
using LevelEditor.Engine.Mesh;
using Microsoft.Xna.Framework;
using LevelEditor.Collision;
using LevelEditor.Engine;
using LevelEditor.Engine.Helper;
using LevelEditor.Objects.Ape;
using System.Collections.Generic;
using LevelEditor.Pathfinding;

namespace LevelEditor.Objects.Lumberjack
{
    public class DoubleAxeKiller : IMoveable, IAttacker, IEscape, ICollide, IActor, IGameObject, ILoader, ISaver
    {
        public int HP { get; set; }
        public bool IsHealer { get; set; }
        public Hut Hut { get; set; }
        public bool IsApe { get; set; }
        public bool IsAlive { get; set; }
        public bool IsMoving { get; set; }
        public Actor Actor { get; }
        public string Name { get; set; }
        public bool IsCrazy { get; set; }
        public bool HasSoundEmitter { get; set; }
        public Sound.AudioSource mSoundEmitter;

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
        private Actor mClosestEnemy;
        private float mClosestDistance;
        private List<Actor> mInRangeActors;
        private bool mIsAttacking;
        private bool mHasAttacked;

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

        private List<Actor> mInViewActors;
        private int mView;

        private Vertex mPath;

        public DoubleAxeKiller(Mesh m, Terrain terrain, QuadTree<Actor> quadTree, Vector3 hutPosition, float innerRange, float outerRange, Scene scene, Silverback silver)
        {

            IsApe = false;
            mSilverback = silver;

            mQuadTree = quadTree;
            IsAlive = true;
            HP = 250;
            mMaxHP = HP;
            mDamage = 12;
            mCoolDown = 1000;
            mRange = 8;
            mView = 8;
            Actor = new Actor(m, this);
            Actor.mAnimator.SetStandardAnimation("idle");
            mTerrain = terrain;
			mIsAttacked = false;
            IsHealer = false;
			mTimeHighlighted = 0;
            mHutPosition = hutPosition;
            mInnerRange = innerRange;
            mOuterRange = outerRange;
            mScene = scene;
            Actor.ModelMatrix = Matrix.CreateTranslation(GetOffsetInRange(innerRange, outerRange) + hutPosition);

            // Lets check if we can spawn there
            while (true)
            {
                var nearby = mQuadTree.QueryRectangle(Actor.mBoundingRectangle.GetAxisAlignedRectangle(1));

                bool collides = false;

                foreach (var actor in nearby)
                {
                    if (actor.mBoundingRectangle.IntersectsRectangle(Actor.mBoundingRectangle, true))
                    {
                        collides = true;
                    }
                }

                if (!collides)
                {
                    break;
                }
                else
                {
                    Actor.ModelMatrix = Matrix.CreateTranslation(GetOffsetInRange(innerRange, outerRange) + hutPosition);
                }
            }

            mHasAttacked = false;
            IsMoving = false;
        }

        public void Update(GameTime gameTime)
        {

            if (IsCrazy)
            {
                mCoolDown += 2000;
                IsCrazy = false;
            }

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

            mTimeElapsed += gameTime.ElapsedGameTime.Milliseconds;
            if (mTimeElapsed >= mCoolDown)
            {
                IsMoving = true;
                mTimeElapsed = 0;
                mInRangeActors = mScene.mQuadTree.QueryRectangle(Actor.mBoundingRectangle.GetAxisAlignedRectangle(mRange));
                if (mInRangeActors.Count > 0)
                {
                    Attack();
                }
            }
            else
            {

                mClosestDistance = float.MaxValue;
                mClosestEnemy = null;
                var mOfRange = mInRangeActors = mQuadTree.QueryRectangle(Actor.mBoundingRectangle.GetAxisAlignedRectangle(3));
                foreach (var actor in mOfRange)
                {
                    if (actor.IActor is IAttacker)
                    {
                        var entity = (IAttacker)actor.IActor;
                        if (entity.IsApe == false && Vector3.Dot(Actor.mModelMatrix.Translation, actor.mModelMatrix.Translation) < mClosestDistance)
                        {
                            mClosestEnemy = actor;
                            mClosestDistance = Vector3.Dot(Actor.mModelMatrix.Translation, actor.mModelMatrix.Translation); 
                        } 
                    }
                }
                if (mClosestEnemy != null)
                {

                    SetTarget(Actor.mModelMatrix.Translation - 0.01f * (mClosestEnemy.mModelMatrix.Translation - Actor.mModelMatrix.Translation));
                }
            }

            mInViewActors = mScene.mQuadTree.QueryRectangle(Actor.mBoundingRectangle.GetAxisAlignedRectangle(mView));
            mClosestEnemy = null;
            mClosestDistance = float.MaxValue;
            foreach (var actor in mInViewActors)
            {
                if (actor.IActor is IAttacker)
                {
                    var entity = (IAttacker)actor.IActor;
                    var curDist = Vector3.Distance(actor.ModelMatrix.Translation, this.Actor.ModelMatrix.Translation);
                    if (entity.IsApe && curDist < mClosestDistance)
                    {
                        mClosestEnemy = actor;
                        mClosestDistance = curDist;
                    }
                }
            }
            
            if (mClosestEnemy != null && mClosestEnemy.IActor is IAttacker && mHasAttacked)
            {
                mIsAttacking = true;
                Actor.mAnimator.PlayAnimation("hit", true, 500);
            }

            if (mClosestEnemy != null && mClosestEnemy.IActor is IAttacker && IsMoving)
            {

                var entity = (IAttacker)mClosestEnemy.IActor;
                if (entity.IsApe)
                {
                    SetTarget(mClosestEnemy.ModelMatrix.Translation);
                }
            }

            if (IsMoving)
            {

                if (!mIsAttacking)
                {
                    Actor.mAnimator.PlayAnimation("Armature.001Action", true, 500);
                }

                var previousLocation = Actor.ModelMatrix.Translation;

                mCurrentDirection = MathExtension.Mix(mCurrentDirection, mDirection, 3.0f * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                var directVector3 = new Vector3(mCurrentDirection.X, 0.0f, mCurrentDirection.Y);
                directVector3.Normalize();
                var vec = Speed * directVector3 * gameTime.ElapsedGameTime.Milliseconds + Actor.ModelMatrix.Translation;
                vec.Y = mTerrain.GetHeight(vec);

                var dir = new Vector2(mTargetPosition.X - vec.X, mTargetPosition.Z - vec.Z);
                mDirection = Vector2.Normalize(dir);

                // IMPORTANT: Remove before moving the actor (this also solves self-collision)
                mQuadTree.Remove(Actor, Actor.mBoundingRectangle.GetAxisAlignedRectangle(1));

                if (dir.Length() != 0)
                {
                    var rotationMatrix = Matrix.CreateLookAt(Vector3.Zero, -new Vector3(directVector3.X, 0.0f, -directVector3.Z), Vector3.Up);

                    Actor.ModelMatrix = rotationMatrix * Matrix.CreateTranslation(vec);
                }

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

                if (0 > Vector2.Dot(currentDirection, mPlannedDirection) || collision || dir.Length() == 0)
                {

                    var quat = Actor.ModelMatrix.Rotation;

                    if (collision && mInRangeMovement)
                    {
                        Actor.ModelMatrix = Matrix.CreateFromQuaternion(quat) * Matrix.CreateTranslation(previousLocation);

                        if (mInRangeMovement)
                        {
                            ChangeTarget(previousLocation + Vector3.Normalize(Actor.ModelMatrix.Right + Actor.ModelMatrix.Backward * 2), true);
                        }
                    }
                    else if (mInRangeMovement)
                    {
                        StayInRange();
                    }
                    else if (0 > Vector2.Dot(currentDirection, mPlannedDirection) && !mInRangeMovement)
                    {
                        mDirection = new Vector2(0.0f);
                        Actor.ModelMatrix = Matrix.CreateFromQuaternion(quat) * Matrix.CreateTranslation(mTargetPosition);
                        IsMoving = false;
                        mPath = mPath?.mNextVertex;
                        if (mPath != null)
                        {
                            var target = new Vector3(mPath.Position.X, 0.0f, mPath.Position.Y);
                            target.Y = mTerrain.GetHeight(target);
                            ChangeTarget(target, mInRangeMovement);
                        }
                    }

                    if (!IsMoving)
                    {
                        StayInRange();
                    }

                }
                mQuadTree.Insert(Actor, Actor.mBoundingRectangle.GetAxisAlignedRectangle(1));

            }
            else
            {

                if (!mIsAttacking)
                {
                    Actor.mAnimator.PlayAnimation("idle", true, 500);
                }

            }

        }

        public void Attack()
        {

            mClosestFiend = null;
            mClosestAngle = float.MaxValue;

            //Check if IActor is always null
            if (mSilverback.Actor.IActor == null){
                mClosestFiend = null;
            }

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
                    else if (elem == mSilverback.Actor)
                    {
                        mIsAttacking = true;
                        mHasAttacked = true;
                        Actor.mAnimator.PlayAnimation("hit", true, 500);

                        mClosestAngle = 0.0f;
                        mClosestFiend = elem;
                        break;
                    }
                    else
                    {

                        if (elem.IActor is IAttacker)
                        {
                            var entity = (IAttacker)elem.IActor;
                            if (entity.IsApe == true)
                            {
                                mIsAttacking = true;
                                mHasAttacked = true;
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

                SetTarget(Actor.mModelMatrix.Translation + 0.01f * (mClosestFiend.mModelMatrix.Translation - Actor.mModelMatrix.Translation));
                
                var enemy = (IAttacker)mClosestFiend.IActor;
                if (enemy.IsApe == true)
                {
                    IsMoving = false;
                    mIsAttacking = true;
                    mHasAttacked = true;
                    enemy.Attacked(mDamage);
                }
            }
            else
            {
                mIsAttacking = false;
                mHasAttacked = false;
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
            if (damage > 0)
            {
                if (!HasSoundEmitter)
                {
                    mSoundEmitter = new Sound.AudioSource(new Sound.AudioBuffer("../../../../Content/Audio/PunchSoundMono.wav"));
                    Actor.mAudioSources.Add(mSoundEmitter);
                    HasSoundEmitter = true;
                }
                mSoundEmitter.Play();
            }
        }

        public void SetTarget(Vector3 target)
        {

            mInRangeMovement = false;

            if (target != Actor.ModelMatrix.Translation)
            {
                mPath = mScene.mVisibilityGraph.GetPath(Actor.ModelMatrix.Translation, target);
                if (mPath != null)
                {
                    var nextTarget = new Vector3(mPath.Position.X, 0.0f, mPath.Position.Y);
                    ChangeTarget(nextTarget, false);
                }
            }

        }

        public void StayInRange()
        {

            mInRangeMovement = true;

            var vector = GetOffsetInRange(mInnerRange, mOuterRange) + mHutPosition;
            vector.Y = mTerrain.GetHeight(vector);

            ChangeTarget(vector, true);

        }

        private void ChangeTarget(Vector3 target, bool inRangeMovement)
        {
            if (target != Actor.mModelMatrix.Translation)
            {
                mTargetPosition = target;
                mTargetPosition.Y = mTerrain.GetHeight(target);
                mDirection = new Vector2(mTargetPosition.X, mTargetPosition.Z) - new Vector2(Actor.ModelMatrix.Translation.X, Actor.ModelMatrix.Translation.Z);
                mDirection.Normalize();
                mPlannedDirection = new Vector2(mDirection.X, mDirection.Y);
                IsMoving = true;
                mInRangeMovement = inRangeMovement;
            }

        }

        private Vector3 GetOffsetInRange(float innerRange, float outerRange)
        {
            var multiplier = (outerRange - innerRange) / outerRange;
            var offset = 1.0f - multiplier;
            var x = 2.0f * (float)mRandom.NextDouble() - 1.0f;
            var z = 2.0f * (float)mRandom.NextDouble() - 1.0f;
            var vector = new Vector3(x, 0.0f, z);
            vector.Normalize();
            var rand = (float)mRandom.NextDouble();
            vector *= (rand * multiplier + offset) * outerRange;
            return vector;
        }

        public void Escape()
        {
                Hut?.RemoveDoubleAxeKiller(this);
            mSoundEmitter.Dispose();
        }

        public void Collide()
        {
            throw new NotImplementedException();
        }

        public string Save()
        {
            return "";
        }

        public bool Load(string str)
        {
            return false;
        }

        public void Move()
        {
            throw new NotImplementedException();
        }

    }

}
