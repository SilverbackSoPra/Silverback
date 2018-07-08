using System;
using LevelEditor.Collision;
using LevelEditor.Engine.Mesh;
using Microsoft.Xna.Framework;
using LevelEditor.Engine;
using LevelEditor.Engine.Helper;
using LevelEditor.Screen;
using System.Collections.Generic;

namespace LevelEditor.Objects.Ape
{
    sealed class Silverback : IMoveable, IAttacker, IEscape, ICollide, IActor, IGameObject
    {
        public int HP { get; set; }
        public bool IsApe { get; set; }
        public bool IsAlive { get; set; }

        private Vector2 mRotation;

        public Actor Actor { get; set; }
        public void Update(GameTime gameTime)
        {
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

        public const float Range = 10.0f;
        public const float InnerRange = 8.0f;

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

        public Silverback(Vector3 silverbackLocation, Vector2 silverbackRotation)
        {
            IsApe = true;

            //HP = 5000;
            HP = 50;
            mDamage = 25;
            mCoolDown = 2;
            IsAlive = true;
            IsApe = true;
            mIsAttacking = false;

            mRotation = silverbackRotation;
        }

        public Silverback(int hp, int damage, int cooldown)
        {
            HP = hp;
            mDamage = damage;
            mCoolDown = cooldown;
            IsApe = true;
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
                Actor.mAnimator.PlayAnimation("smash", true, 500);

            }
        }

        public void Attacked(int damage)
        {
            HP -= damage;

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

        public void Collide()
        {
        }

        public void Escape()
        {
        }

        public bool IsMoving { get; set; }

        public void Move() //Use enum for this one?
        {
            throw new NotImplementedException();

            
        }

        public void SetTarget(Vector3 target)
        {
            
        }

        public bool Update(Terrain terrain, Camera camera, CameraHandler handler, Scene scene)
        {

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
                    if (Vector3.Dot(dir, actor.ModelMatrix.Forward) > 0.25)
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

    }
}
