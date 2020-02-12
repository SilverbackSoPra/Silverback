using System;
using LevelEditor.Engine;
using LevelEditor.Engine.Helper;
using LevelEditor.Engine.Mesh;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using LevelEditor.Collision;
using LevelEditor.Pathfinding;

namespace LevelEditor.Objects.Ape.SubApe
{
    class Capuchin : IMoveable, IAttacker, IEscape, ICollide, IActor, IGameObject, ILoader, ISaver
    {
        public Actor Actor1 {get;}
        public int HP { get; set; }
        public bool IsHealer { get; set; }
        public bool IsApe { get; set; }
        public bool IsAlive { get; set; }
        public bool IsMoving { get; set; }
        public Actor Actor { get; }
        public string Name { get; set; }
        public bool HasSoundEmitter { get; set; }
        public bool IsCrazy { get; set; }

        public Sound.AudioSource mSoundEmitter;


        private Random mRandom;
        private Scene mScene;
        public int mDamage;
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

        private bool mHasAttacked;

        public Vertex mVertex;
        public Vertex mTargetVertex;

        public Capuchin(Mesh m, Terrain terrain, Silverback silverBack, Scene scene, ref Random rand)
        {
            IsApe = true;
            mIsAttacking = false;
            mHasAttacked = false;

            HP = 100;
            mMaxHP = HP;
            mDamage = -5;
            mRange = 1;
            mCoolDown = 1000;
            IsAlive = true;
            IsHealer = true;
            Actor = new Actor(m, this);
            mRandom = rand;
            mSilverback = silverBack;
            mTerrain = terrain;
            Actor.mAnimator.SetStandardAnimation("idle");
            Actor.ModelMatrix = Matrix.CreateTranslation(GenerateSilverbackVector());
            mTargetPosition = new Vector3(0.0f);
            mDirection = new Vector2(0.0f);
            mLastSilverbackLocation = mSilverback.Actor.ModelMatrix.Translation;
            IsMoving = false;
            mScene = scene;
            mQuadTree = scene.mQuadTree;

        }

        public void Update(GameTime gameTime)
        {
            mQuadTree.Remove(Actor, Actor.mBoundingRectangle.GetAxisAlignedRectangle(1));

            //Changes colors back to normal after attacked and a set time is over (Damage indicator)
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

            //Attacks an enemy
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

            //If the ape wasnt sent anywhere else, then he follows the last attacked lumber
            if (IsMoving == false && mHasAttacked && mClosestFiend != null && mClosestFiend.IActor is IMoveable && mClosestFiend.IActor is IAttacker)
            {
                var enemy = (IMoveable)mClosestFiend.IActor;
                var enemy2 = (IEscape)mClosestFiend.IActor;
                if (enemy.IsMoving && enemy2.IsAlive)
                {
                    SetTarget(mClosestFiend.ModelMatrix.Translation);
                    mHasAttacked = false;
                }
            }

            mTargetCoolDown -= gameTime.ElapsedGameTime.Milliseconds;

            if (IsMoving)
            {
                if (!mIsAttacking)
                {
                    Actor.mAnimator.PlayAnimation("walk", true, 500);
                }

                if (mIsSilverbackTarget && mTargetCoolDown < 0)
                {
                    CheckSilverbackRange();
                    mTargetCoolDown = 6000;
                }

                mCurrentDirection = MathExtension.Mix(mCurrentDirection, mDirection, 10.0f * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                var directVector3 = new Vector3(mCurrentDirection.X, 0.0f, mCurrentDirection.Y);
                directVector3.Normalize();
                var vec = Speed * directVector3 * gameTime.ElapsedGameTime.Milliseconds + Actor.ModelMatrix.Translation;
                vec.Y = mTerrain.GetHeight(vec);

                var dir = new Vector2(mTargetPosition.X - vec.X, mTargetPosition.Z - vec.Z);
                mDirection = Vector2.Normalize(dir);

                if (dir.Length() != 0)
                {
                    var rotationMatrix = Matrix.CreateLookAt(Vector3.Zero,
                        -new Vector3(directVector3.X, 0.0f, -directVector3.Z),
                        Vector3.Up);

                    Actor.ModelMatrix = rotationMatrix * Matrix.CreateTranslation(vec);
                }

                var currentDirection = new Vector2
                {
                    X = mTargetPosition.X - Actor.ModelMatrix.Translation.X,
                    Y = mTargetPosition.Z - Actor.ModelMatrix.Translation.Z
                };
                currentDirection.Normalize();
                if (0 > Vector2.Dot(currentDirection, mPlannedDirection) || dir.Length() == 0)
                {
                    mDirection = new Vector2(0.0f);
                    var quat = Actor.ModelMatrix.Rotation;
                    Actor.ModelMatrix = Matrix.CreateFromQuaternion(quat) * Matrix.CreateTranslation(mTargetPosition);
                    IsMoving = false;
                    mVertex = mVertex?.mNextVertex;
                    if (mVertex != null)
                    {
                        var target = new Vector3(mVertex.Position.X, 0.0f, mVertex.Position.Y);
                        target.Y = mTerrain.GetHeight(target);
                        ChangeTarget(target);
                    }
                }
            }
            else
            {

                if (! mIsAttacking)
                {
                    Actor.mAnimator.PlayAnimation("idle", true, 500);
                }

                if (mTargetCoolDown < 0)
                {
                    var vector = GenerateSilverbackVector();
                    SetSilverbackTarget(vector); 
                    CheckSilverbackRange();
                    mTargetCoolDown = 4000;
                }                

            }

            mLastSilverbackLocation = mSilverback.Actor.ModelMatrix.Translation;
            mQuadTree.Insert(Actor, Actor.mBoundingRectangle.GetAxisAlignedRectangle(1));

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

                    if (elem.IActor is IAttacker)
                    {
                        var entity = (IAttacker)elem.IActor;
                        if (entity.IsApe == true && entity.IsHealer == false)
                        {
                            mIsAttacking = true;
                            mHasAttacked = true;
                            Actor.mAnimator.PlayAnimation("heal", true, 500);
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

            if (mClosestFiend != null && mClosestFiend.IActor is IAttacker)
            {
                
                SetTarget(Actor.mModelMatrix.Translation + 0.01f * (mClosestFiend.mModelMatrix.Translation - Actor.mModelMatrix.Translation));

                var enemy = (IAttacker)mClosestFiend.IActor;
                enemy.Attacked(mDamage);
            }
            else
            {
                mIsAttacking = false;
            }
        }

        public void Attacked(int damage)
        {

            Actor.Color = new Vector3(1.0f, 0.0f, 0.0f);
            mIsAttacked = true;
            if (damage >= 0 & HP > 0)
                {
                    HP -= damage;
                }
            else if (damage < 0)
                {
                    //Cant heal the other healers
                    /*
                    HP -= damage;
                    if (HP > mMaxHP)
                    {
                        HP = mMaxHP;
                    }
                    */
                }
            else
            {
                //remove later
                Escape();
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
            if (target != Actor.ModelMatrix.Translation)
            {
                mVertex = mScene.mVisibilityGraph.GetPath(Actor.ModelMatrix.Translation, target);
                if (mVertex != null)
                {
                    var nextTarget = new Vector3(mVertex.Position.X, 0.0f, mVertex.Position.Y);
                    ChangeTarget(nextTarget);
                    mIsSilverbackTarget = false;
                }
            }
        }

        private void SetSilverbackTarget(Vector3 target)
        {
            if (target != Actor.ModelMatrix.Translation)
            {
                mVertex = mScene.mVisibilityGraph.GetPath(Actor.ModelMatrix.Translation, target);
                if (mVertex != null)
                {
                    var nextTarget = new Vector3(mVertex.Position.X, 0.0f, mVertex.Position.Y);
                    ChangeTarget(nextTarget);
                    mIsSilverbackTarget = true;
                }
            }
        }

        private void ChangeTarget(Vector3 target)
        {
            mTargetPosition.X = target.X;
            mTargetPosition.Z = target.Z;
            mTargetPosition.Y = mTerrain.GetHeight(mTargetPosition);
            mDirection = new Vector2(mTargetPosition.X, mTargetPosition.Z) - new Vector2(Actor.ModelMatrix.Translation.X, Actor.ModelMatrix.Translation.Z);
            mDirection.Normalize();
            mPlannedDirection = new Vector2(mDirection.X, mDirection.Y);
            IsMoving = true;
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

        private bool IsInFrontOfSilverback()
        {

            Vector3.Distance(mSilverback.Actor.ModelMatrix.Translation, Actor.ModelMatrix.Translation);

            return false;
        }
        public void Move()
        {
        }

        public void Escape()
        {
            mSoundEmitter.Dispose();
        }

        public void Collide()
        {
        }


        public string Save()
        {
            return "";
        }

        public bool Load(string str)
        {
            return false;
        }

    }

}
