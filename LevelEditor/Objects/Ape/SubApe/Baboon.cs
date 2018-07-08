using System;
using LevelEditor.Engine;
using LevelEditor.Engine.Mesh;
using Microsoft.Xna.Framework;
using LevelEditor.Engine.Helper;
using System.Collections.Generic;
using LevelEditor.Collision;
using LevelEditor.Pathfinding;

namespace LevelEditor.Objects.Ape.SubApe
{
    internal sealed class Baboon : IMoveable, IAttacker, IEscape, ICollide, IActor, IGameObject
    {

        public Actor Actor { get; }
        public int HP { get; set; }

        public bool IsApe { get; set; }
        
        public bool IsAlive { get; set; }


        private static Random mRandom = new Random();
        private QuadTree<Actor> mQuadTree;
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

        private bool mIsAlive;

        private bool mIsAttacked;
        private const int mHighlightTime = 300;
        private int mTimeHighlighted;


        private bool mInRangeMovement = false;
        private float mInnerRange;
        private float mOuterRange;
        private Vector3 mHutPosition;

        private bool mAttacking;

        public Scene mScene;
        public Vertex mVertex;
        public Vertex mTargetVertex;

        public Baboon(Mesh m, Terrain terrain, Silverback silverBack, QuadTree<Actor> quad, ref Random rand)
        {
            IsApe = true;
            mIsAttacking = false;

            HP = 350;
            mMaxHP = HP;
            mDamage = 15;
            mRange = 2;
            IsAlive = true;
            mCoolDown = 1000;
            mTimeElapsed = 0;
            mInRangeActors = new List<Actor>();
            Actor = new Actor(m, this);
            Actor.mAnimator.SetStandardAnimation("idle");

            if (rand == null)
            {
                rand = new Random();
            }

            var silverbackX = rand.Next((int)Math.Floor(silverBack.Actor.ModelMatrix.Translation.X - 10), (int)Math.Floor(silverBack.Actor.ModelMatrix.Translation.X + 10));

            var silverbackZ = rand.Next((int)Math.Floor(silverBack.Actor.ModelMatrix.Translation.Z - 10), (int)Math.Floor(silverBack.Actor.ModelMatrix.Translation.Z + 10));


            Actor.ModelMatrix = Matrix.CreateTranslation(new Vector3(silverBack.Actor.ModelMatrix.Translation.X, terrain.GetHeight(new Vector3(silverbackX)), silverbackZ));
            // Actor.ModelMatrix = Matrix.CreateTranslation(new Vector3(0.0f, terrain.GetHeight(new Vector3(0.0f)), 0.0f));
            mTargetPosition = new Vector3(0.0f);
            mDirection = new Vector2(0.0f);
            mSilverback = silverBack;
            mLastSilverbackLocation = mSilverback.Actor.ModelMatrix.Translation;
            mTerrain = terrain;
            IsMoving = false;
            mQuadTree = quad;

        }

        public void Attack()
        {

            mClosestFiend = null;
            mClosestAngle = float.MaxValue;
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
                            if (entity.IsApe == false)
                            {
                                mIsAttacking = true;
                                Actor.mAnimator.PlayAnimation("singing", true, 500);
                                var currentAngle = Vector2.Dot(actor2Enemy2D, mCurrentDirection);
                                if (currentAngle < mClosestAngle)
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
                enemy.Attacked(mDamage);
            }
            else
            {
                Actor.mAnimator.PlayAnimation("walk", true, 500);
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
            else
            {
                //remove later
                Escape();
            }
        }

        public void Collide()
        {
        }

        public void Escape()
        {
        }

        public bool IsMoving { get; set; }

        public void Move()
        {
        }

        public void SetTarget(Vector3 target)
        {
            mVertex = mScene.mVisibilityGraph.GetPath(Actor.ModelMatrix.Translation, target);
            mTargetPosition.X = mVertex.mNextVertex.Position.X;
            mTargetPosition.Z = mVertex.mNextVertex.Position.Y;
            mTargetPosition.Y = mTerrain.GetHeight(mTargetPosition);
            mDirection = new Vector2(mTargetPosition.X, mTargetPosition.Z) - new Vector2(Actor.ModelMatrix.Translation.X, Actor.ModelMatrix.Translation.Z);
            mDirection.Normalize();
            mPlannedDirection = new Vector2(mDirection.X, mDirection.Y);
            IsMoving = true;
            mIsSilverbackTarget = false;
            
        }

        private void SetSilverbackTarget(Vector3 target)
        {
            mVertex = mScene.mVisibilityGraph.GetPath(Actor.ModelMatrix.Translation, target);
            mTargetPosition.X = mVertex.mNextVertex.Position.X;
            mTargetPosition.Z = mVertex.mNextVertex.Position.Y;
            mTargetPosition.Y = mTerrain.GetHeight(mTargetPosition);
            mDirection = new Vector2(mTargetPosition.X, mTargetPosition.Z) - new Vector2(Actor.ModelMatrix.Translation.X, Actor.ModelMatrix.Translation.Z);
            mDirection.Normalize();
            mPlannedDirection = new Vector2(mDirection.X, mDirection.Y);
            IsMoving = true;
            mIsSilverbackTarget = true;
        }

        public void Update(GameTime gameTime)
        {
            mQuadTree.Remove(Actor, Actor.mBoundingRectangle.GetAxisAlignedRectangle(1));

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
                mTimeElapsed = 0;
                mInRangeActors = mQuadTree.QueryRectangle(Actor.mBoundingRectangle.GetAxisAlignedRectangle(mRange));
                if (mInRangeActors.Count > 0)
                {
                    Attack();
                }
            }

            var oldMatrix = Actor.ModelMatrix;

            if (IsMoving)
            {
                if (! mIsAttacking)
                {
                    Actor.mAnimator.PlayAnimation("walk", true, 500);
                }
                mTargetCoolDown -= gameTime.ElapsedGameTime.Milliseconds;

                if (mIsSilverbackTarget && mTargetCoolDown < 0)
                {
                    CheckSilverbackRange();
                    mTargetCoolDown = 2000;
                }

                mCurrentDirection = MathExtension.Mix(mCurrentDirection, mDirection, 3.0f * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                var directVector3 = new Vector3(mCurrentDirection.X, 0.0f, mCurrentDirection.Y);
                directVector3.Normalize();
                var vec = Speed * directVector3 * gameTime.ElapsedGameTime.Milliseconds + Actor.ModelMatrix.Translation;
                vec.Y = mTerrain.GetHeight(vec);

                mDirection = Vector2.Normalize(new Vector2(mTargetPosition.X - vec.X, mTargetPosition.Z - vec.Z));

                var rotationMatrix = Matrix.CreateLookAt(Vector3.Zero,
                    -new Vector3(directVector3.X, 0.0f, -directVector3.Z),
                    Vector3.Up);

                Actor.ModelMatrix = rotationMatrix * Matrix.CreateTranslation(vec);

                var currentDirection = new Vector2
                {
                    X = mTargetPosition.X - Actor.ModelMatrix.Translation.X,
                    Y = mTargetPosition.Z - Actor.ModelMatrix.Translation.Z
                };
                currentDirection.Normalize();
                if (0 > Vector2.Dot(currentDirection, mPlannedDirection))
                {
                    mDirection = new Vector2(0.0f);
                    var quat = Actor.ModelMatrix.Rotation;
                    Actor.ModelMatrix = Matrix.CreateFromQuaternion(quat) * Matrix.CreateTranslation(mTargetPosition);
                    IsMoving = false;
                }

            }
            else
            {

                if (! mIsAttacking)
                {
                    Actor.mAnimator.PlayAnimation("idle", true, 500);
                }

                CheckSilverbackRange();

            }

            mLastSilverbackLocation = mSilverback.Actor.ModelMatrix.Translation;
            mQuadTree.Insert(Actor, Actor.mBoundingRectangle.GetAxisAlignedRectangle(1));
            if (mVertex == null)
            {
                mVertex = mScene.mVisibilityGraph.GetPath(Actor.ModelMatrix.Translation, mTargetPosition);
            }
            if (mVertex != null && mVertex.mNextVertex == null)
            {
                return;
            }
            mVertex = mVertex.mNextVertex;
            if (mVertex == null)
            {
                mVertex = mScene.mVisibilityGraph.GetPath(Actor.ModelMatrix.Translation, mTargetPosition);
            }
            mTargetPosition.X = mVertex.Position.X;
            mTargetPosition.Z = mVertex.Position.Y;
        }

        private void CheckSilverbackRange()
        {

            // Check if the ape is still in the range of the silverback
            var location = Vector3.Zero;
            if (IsMoving)
            {
                location = mTargetPosition;
            }
            else
            {
                location = Actor.ModelMatrix.Translation;
            }
            var silverBackLocation = mSilverback.Actor.ModelMatrix.Translation;

            var vec2 = new Vector2(silverBackLocation.X - location.X, silverBackLocation.Z - location.Z);
            var len = vec2.Length();
            
            if (len < Silverback.InnerRange)
            {
                var vector = GenerateSilverbackVector();


                SetSilverbackTarget(vector);

            }

            if (silverBackLocation == mLastSilverbackLocation)
            {
                return;
            }

            if (len > Silverback.Range)
            {

                var vector = GenerateSilverbackVector();

                SetSilverbackTarget(vector);

            }

            var x = Actor.ModelMatrix.Translation.X;
            var z = Actor.ModelMatrix.Translation.Z;
            // Check if the booban is going to collide with the silverback
            if (mSilverback.Actor.mBoundingRectangle.IntersectsLine(new Vector2(x, z),
                new Vector2(mTargetPosition.X, mTargetPosition.Z)))
            {
                // Generate a new random vector
                var vector = GenerateSilverbackVector();

                // It has 7 attempts to generate a appropriate vector
                for (var i = 0; i < 7; i++)
                {

                    var intersects = mSilverback.Actor.mBoundingRectangle.IntersectsLine(new Vector2(x, z),
                        new Vector2(vector.X, vector.Z));
                    if (!intersects)
                    {
                        break;
                    }
                    vector = GenerateSilverbackVector();
                }

                SetSilverbackTarget(vector);
            }
        }

        private Vector3 GenerateSilverbackVector()
        {

            // NextDouble returns a number between 0 and 1
            // We need to bring the random number to a range of [InnerRange/Range, 1.0f] without losing InnerRange/Range percent of the generated numbers
            var multiplier = (Silverback.Range - Silverback.InnerRange) / Silverback.Range;
            var offset = 1.0f - multiplier;
            var x = (2.0f * (float)mRandom.NextDouble() - 1.0f);
            var z = (float)mRandom.NextDouble();
            var vector = x * mSilverback.Actor.ModelMatrix.Right + z * mSilverback.Actor.ModelMatrix.Backward;
            vector.Normalize();
            vector *= ((float)mRandom.NextDouble() * multiplier + offset) * Silverback.Range;
            vector += mSilverback.Actor.ModelMatrix.Translation;
            vector.Y = mTerrain.GetHeight(vector);
            return vector;

        }

        public string Name
        {
            get; set;
        }

        public bool HasSoundEmitter { get; set; }
        public string Save()
        {
            return "";
        }

        public bool Load()
        {
            return false;
        }

        private bool IsInFrontOfSilverback()
        {

            Vector3.Distance(mSilverback.Actor.ModelMatrix.Translation, Actor.ModelMatrix.Translation);

            return false;
        }
    }
}
