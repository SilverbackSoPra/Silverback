using System;
using LevelEditor.Collision;
using LevelEditor.Engine.Mesh;
using Microsoft.Xna.Framework;
using LevelEditor.Engine;
using LevelEditor.Engine.Helper;
using LevelEditor.Screen;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using LevelEditor.Sound;

namespace LevelEditor.Objects.Ape
{
    [Serializable()]
    public class Silverback : IMoveable, IAttacker, IEscape, ICollide, IActor, IGameObject, ISerializable, ILoader, ISaver
    {
        public int HP { get; set; }
        public bool IsHealer { get; set; }
        public bool IsApe { get; set; }
        public bool IsAlive { get; set; }
        public bool IsMoving { get; set; }
        [XmlIgnore]
        public Actor Actor { get; set; }
        public string Name { get; set; }
        public bool IsCrazy { get; set; }
        public bool HasSoundEmitter { get; set; }
        public Sound.AudioSource mSoundEmitter;


        public const float Range = 10.0f;
        public const float InnerRange = 8.0f;

        public static Random mRandom = new Random();
        [XmlIgnore]
        public Scene mScene;
        public int mDamage;
        public int mMaxHP;
        public int mRange;
        public int mCoolDown;
        public int mTimeElapsed;
        // public int mCoolDown;

        public const float Speed = 0.0040f;
        public const int MaxWalkingRangeValue = 128;
        public Vector3 mTargetPosition;
        public Vector2 mDirection;
        public Vector2 mRotation;
        public Vector2 mPlannedDirection;
        public Vector2 mCurrentDirection;

        public readonly Terrain mTerrain;
        public readonly Silverback mSilverback;
        public bool mIsSilverbackTarget;
        public Vector3 mLastSilverbackLocation;
        public int mTargetCoolDown;
        [XmlIgnore]
        public Actor mClosestFiend;
        public float mClosestAngle;
        [XmlIgnore]
        public List<Actor> mInRangeActors;
        public bool mIsAttacking;
        private bool mHasAttacked;

        [XmlIgnore]
        public ActorBatch mActorBatch;

        [XmlIgnore]
        public QuadTree<Actor> mQuadTree;
        public bool mIsAlive;

        public bool mIsAttacked;
        public const int mHighlightTime = 300;
        public int mTimeHighlighted;


        public bool mInRangeMovement = false;
        public float mInnerRange;
        public float mOuterRange;
        public Vector3 mHutPosition;

        public bool mAttacking;

        public Silverback(Vector3 silverbackLocation, Vector2 silverbackRotation, Mesh mesh)
        {
            HP = 300;
            mMaxHP = HP;
            mDamage = 12;
            mCoolDown = 650;
            mRange = 1;
            IsAlive = true;
            IsHealer = false;
            IsApe = true;
            mIsAttacking = false;

            Actor = new Actor(mesh, this) { ModelMatrix = Matrix.CreateTranslation(silverbackLocation) };

            mRotation = silverbackRotation;
        }

        public Silverback(SerializationInfo info, StreamingContext context)
        {
            IsApe = (bool)info.GetValue("IsApe", typeof(bool));
            HP = (int)info.GetValue("HP", typeof(int));
            mDamage = (int)info.GetValue("mDamage", typeof(int));
            mCoolDown = (int)info.GetValue("mCoolDown", typeof(int));
            mRange = (int)info.GetValue("mRange", typeof(int));
            mIsAlive = (bool)info.GetValue("mIsAlive", typeof(bool));
            mIsAttacking = (bool)info.GetValue("mIsAttacking", typeof(bool));
            mIsAttacked = (bool)info.GetValue("mIsAttacked", typeof(bool));
        }

        public Silverback()
        { }

        public bool Update(Terrain terrain, Camera camera, CameraHandler handler, Scene scene, GameTime gameTime)
        {

            //mQuadTree.Remove(Actor, Actor.mBoundingRectangle.GetAxisAlignedRectangle(1));

            //scene.mVisibilityGraph.GetPath(camera.mLocation, new Vector3(20.0f));

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
                mIsAttacking = false;

                mTimeElapsed = 0;
                mInRangeActors = scene.mQuadTree.QueryRectangle(Actor.mBoundingRectangle.GetAxisAlignedRectangle(mRange));
                if (mInRangeActors.Count > 0)
                {
                    Attack();
                }
            }

            if (IsMoving == false && mHasAttacked && mClosestFiend != null && mClosestFiend.IActor is IMoveable)
            {
                var enemy = (IMoveable)mClosestFiend.IActor;
                if (enemy.IsMoving)
                {
                    SetTarget(mClosestFiend.ModelMatrix.Translation);
                    mHasAttacked = false;
                }
                else
                {
                    IsMoving = false;
                }
            }


            var location = camera.mLocation;
            var rotation = new Vector2(camera.mRotation.X + handler.mRotationOffset, 0.0f);

            var oldMatrix = Actor.ModelMatrix;
            scene.mQuadTree.Remove(Actor, Actor.mBoundingRectangle.GetAxisAlignedRectangle(1));

            location.Y = terrain.GetHeight(location);

            if (handler.mMoving)
            {
                mRotation = MathExtension.Mix(mRotation, rotation, 0.3f);
                Actor.mAnimator.PlayAnimation("walk", true, 250);
            }
            else
            {
                if (mIsAttacking)
                {
                    Actor.mAnimator.PlayAnimation("smash", true, 500);
                }
                else
                {
                    Actor.mAnimator.PlayAnimation("idle", true, 250);
                }
            }

            var rotationMatrix = Matrix.CreateRotationY(mRotation.X);
            var locationMatrix = Matrix.CreateTranslation(location);

            Actor.ModelMatrix = rotationMatrix * locationMatrix;

            var boundingRect = Actor.mBoundingRectangle.GetAxisAlignedRectangle(1);
            var actorList = scene.mQuadTree.QueryRectangle(boundingRect);

            var collisions = new List<Actor>();

            foreach (var actor in actorList)
            {

                if (actor.mBoundingRectangle.IntersectsRectangle(Actor.mBoundingRectangle, true))
                {
                    actor.Collision = true;
                    collisions.Add(actor);
                }
                else
                {
                    actor.Collision = false;
                }

                actor.QuadTree = true;
            }

            if (collisions.Count > 0)
            {
                // We should handle this in a different way because this causes ervey movement to stop
                // The problem with this approach lies in the fact that even if we want to move away from the
                // colliding object and the tail still hits the object the movement is still interupted

                var canMove = true;

                foreach (var actor in collisions)
                {
                    var dir = actor.ModelMatrix.Translation - Actor.ModelMatrix.Translation;
                    dir.Normalize();
                    if (Vector3.Dot(dir, actor.ModelMatrix.Forward) < 0.0)
                    {
                        canMove = false;
                    }
                }

                if (collisions.Count > 0)
                {
                    Actor.ModelMatrix = oldMatrix;
                    handler.mLocation = Actor.ModelMatrix.Translation;
                }
                
            }

            scene.mQuadTree.Insert(Actor, Actor.mBoundingRectangle.GetAxisAlignedRectangle(1));

            return collisions.Count > 0;

        }
            
            // Console.WriteLine("Silverback " + Actor.ModelMatrix.Translation + " | " + Actor.ModelMatrix.Forward);

        public void Update(GameTime gameTime) { }

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
                        if (entity.IsApe == false)
                        {
                            mIsAttacking = true;
                            mHasAttacked = true;
                            var currentAngle = Vector2.Dot(actor2Enemy2D, mCurrentDirection);
                            if (currentAngle < mClosestAngle)
                            {
                                mClosestAngle = currentAngle;
                                mClosestFiend = elem;
                            }
                        }
                    }
                }

                    /*
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
                                Actor.mAnimator.PlayAnimation("attack", true, 500);
                                var currentAngle = Vector2.Dot(actor2Enemy2D, mCurrentDirection);
                                if (currentAngle < mClosestAngle)
                                {
                                    mClosestAngle = currentAngle;
                                    mClosestFiend = elem;
                                }
                            }
                        }
                    }
                    */
            }
            

            if (mClosestFiend != null && mClosestFiend.IActor is IAttacker)
            {
                var enemy = (IAttacker)mClosestFiend.IActor;
                enemy.Attacked(mDamage);
            }
            else
            {
                mIsAttacking = false;
                Actor.mAnimator.PlayAnimation("walk", true, 500);
            }
        }

        public void Attacked(int damage)
        {
            mIsAttacked = true;
            if (damage >= 0 & HP > 0)
                {
                    Actor.Color = new Vector3(1.0f, 0.0f, 0.0f);
                    HP -= damage;
            }
            else if (damage < 0)
                {
                }
            else
            {
                //remove later
                Escape();
            }
            if (damage >0)
            {
                if (!HasSoundEmitter)
                {
                    mSoundEmitter = new AudioSource(new AudioBuffer("../../../../Content/Audio/PunchSoundMono.wav"));
                    Actor.mAudioSources.Add(mSoundEmitter);
                    HasSoundEmitter = true;
                }
                mSoundEmitter.Play();
            }
    }

        public void SetTarget(Vector3 target)
        {
            
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsApe", IsApe);
            info.AddValue("HP", HP);
            info.AddValue("mDamage", mDamage);
            info.AddValue("mCoolDown", mCoolDown);
            info.AddValue("mRange", mRange);
            info.AddValue("mIsAlive", mIsAlive);
            info.AddValue("mIsAttacking", mIsAttacking);
            info.AddValue("mIsAttacked", mIsAttacked);
        }

        public void Move() //Use enum for this one?
        {
            
        }
        public string Save()
        {
            return "";

        }

        public void Collide()
        {
        }

        public void Escape()
        {
            mSoundEmitter.Dispose();
        }

        public bool Load(string str)
        {
            throw new NotImplementedException();
        }
    }
}
