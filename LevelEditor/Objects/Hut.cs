using LevelEditor.Engine;
using LevelEditor.Engine.Mesh;
using LevelEditor.Objects.Ape;
using System;
using System.Collections.Generic;
using LevelEditor.Engine.Loader;
using LevelEditor.Objects.Lumberjack;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;

namespace LevelEditor.Objects
{
    class Hut : IEscape, ICollide, IActor, IGameObject
    {

        private static Random mRandom = new Random();

        private const float InnerRange = 20.0f;
        private const float OuterRange = 30.0f;

        private int mNumLumberjacks;
        private bool mSpawned;
        private Scene mScene;
        private Mesh mLumberjackMesh;
        private Mesh mDoubleAxeKillerMesh;
        private Mesh mAxeMesh;
        private Silverback mSilverback;
        private float mDistanceSilverbackHut;
        private bool mWasInInnerRange = false;

        private List<Lumberjack.Lumberjack> mLumberjacks;
        private List<DoubleAxeKiller> mDoubleAxeKillers;

        public int HP { get; set; }
        public bool IsAlive { get; set; }

        public Actor Actor { get; }

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

        public void Escape()
        {
            //Needs to remove the hut from the Actorbatch(Scene)
            //mLevel.RemoveActor()
        }

        public void Collide()
        {
        }

        public void RemoveLumberjack(Lumberjack.Lumberjack lumberjack)
        {
            HP -= 1;
            mLumberjacks.Remove(lumberjack);
        }

        public void RemoveDoubleAxeKiller(DoubleAxeKiller doubleAxeKiller)
        {
            HP -= 1;
            mDoubleAxeKillers.Remove(doubleAxeKiller);
        }

        public void Update(GameTime gameTime)
        {
            
            var inRange = mScene.mQuadTree.QueryRectangle(Actor.mBoundingRectangle.GetAxisAlignedRectangle((int)Math.Floor(OuterRange)));

            mDistanceSilverbackHut = Vector3.Distance(mSilverback.Actor.ModelMatrix.Translation, Actor.ModelMatrix.Translation);

            if (!mSpawned && mDistanceSilverbackHut <= OuterRange)
            {
                mNumLumberjacks = LumberChoice.NumLumberjacks(HP);

                // Spawning is not enough, we should give them a target
                // Also I suggest that we use something like an inner and an outer range.
                // If the distance to the siverback is less than the outer range, we spawn the lumberjacks
                // If the distance to the silverback is less than the inner range, we set the targets of the lumberjacks
                for (var i = 0; i < mNumLumberjacks; i++)
                {
                    var lumberjack = new Lumberjack.Lumberjack(mLumberjackMesh, mScene.mTerrain, mScene.mQuadTree, Actor.ModelMatrix.Translation, 4.0f, 15.0f, mScene, mSilverback);
                    lumberjack.Hut = this;
                    var axe = new Actor(mAxeMesh);
                    lumberjack.Actor.mAnimator.AttachActor("Bone.016", axe);
                    mLumberjacks.Add(lumberjack);
                    lumberjack.StayInRange();
                    mScene.Add(lumberjack);
                    mScene.Add(axe);
                }
                
                for (var i = 0; i < (HP - mNumLumberjacks); i++)
                {
                    var killer = new Lumberjack.DoubleAxeKiller(mDoubleAxeKillerMesh, mScene.mTerrain, mScene.mQuadTree, Actor.ModelMatrix.Translation, 4.0f, 15.0f);
                    killer.Hut = this;
                    var leftAxe = new Actor(mAxeMesh);
                    var rightAxe = new Actor(mAxeMesh);
                    mDoubleAxeKillers.Add(killer);
                    killer.Actor.mAnimator.AttachActor("Bone.017", leftAxe);
                    killer.Actor.mAnimator.AttachActor("Bone.016", rightAxe);
                    killer.StayInRange();
                    mScene.Add(killer);
                    mScene.Add(leftAxe);
                    mScene.Add(rightAxe);
                }
                
                mSpawned = true;

            }
            else
            {

                // Instead of calculating the range to the silverback, we should do a query on the quad tree
                // to determine if there are any apes in the range. We should also do this for spawning.
                mDistanceSilverbackHut = Vector3.Distance(mSilverback.Actor.ModelMatrix.Translation, Actor.ModelMatrix.Translation);

                // Set the target to one of the lumberjacks and let them return of the lumberjack is out of range
                if (mDistanceSilverbackHut <= InnerRange)
                {
                    
                    foreach (var lumberjack in mLumberjacks)
                    {
                        lumberjack.SetTarget(mSilverback.Actor.ModelMatrix.Translation);
                    }
                    
                    foreach (var doubleAxeKiller in mDoubleAxeKillers)
                    {
                        doubleAxeKiller.SetTarget(mSilverback.Actor.ModelMatrix.Translation); 
                    }
                    

                    mWasInInnerRange = true;

                }
                else
                {

                    // We just want to tell them once to stay in range
                    if (mWasInInnerRange)
                    {
                        foreach (var lumberjack in mLumberjacks)
                        {
                            lumberjack.StayInRange();
                        }
                        foreach (var killer in mDoubleAxeKillers)
                        {
                            killer.StayInRange();
                        }

                        mWasInInnerRange = false;

                    }

                }

            }

        }


        public Hut(Mesh hut, Mesh lumberjack, Mesh doubleAxeKiller, Mesh axeMesh, Silverback silverback, Scene scene, int hp)
        {
            // HP of the hut are equal to the number of the lumberjacks that were spawned at this hut.
            // Each time one of the spawned lumberjacks dies, the hut loses 1 HP.
            // The number of lumberjacks that a hut can spawn should be dynamic based on the level (different levels could allow the player
            // to have a different number of apes to summon) 
            HP = hp;
            mSpawned = false;
            mSilverback = silverback;
            mLumberjackMesh = lumberjack;
            mDoubleAxeKillerMesh = doubleAxeKiller;
            mAxeMesh = axeMesh;
            mScene = scene;
            Actor = new Actor(hut, this);

            mLumberjacks = new List<Lumberjack.Lumberjack>();
            mDoubleAxeKillers = new List<DoubleAxeKiller>();

        }

    }

}
