using LevelEditor.Engine;
using LevelEditor.Engine.Mesh;
using LevelEditor.Objects.Ape;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using LevelEditor.Engine.Loader;
using LevelEditor.Objects.Lumberjack;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;

namespace LevelEditor.Objects
{
    [Serializable()]
    public class Hut : IEscape, ICollide, IActor, IGameObject, ISerializable, ILoader, ISaver
    {

        private static Random mRandom = new Random();

        public static int mAmount = 0;
        public int mId;

        public float mInnerRange = 100.0f;
        public float mOuterRange = 110.0f;

        public int mNumLumberjacks;
        public bool mSpawned;
        [XmlIgnore]
        public Scene mScene;
        [XmlIgnore]
        public Mesh mLumberjackMesh;
        [XmlIgnore]
        public Mesh mDoubleAxeKillerMesh;
        [XmlIgnore]
        public Mesh mAxeMesh;
        [XmlIgnore]
        public Silverback mSilverback;
        public float mDistanceSilverbackHut;
        public bool mWasInInnerRange = false;

        [XmlIgnore]
        public List<Lumberjack.Lumberjack> mLumberjacks;
        [XmlIgnore]
        public List<DoubleAxeKiller> mDoubleAxeKillers;

        public bool mTechDemo;

        public int HP { get; set; }
        public bool IsAlive { get; set; }

        [XmlIgnore]
        public Actor Actor { get; set; }

        public string Name { get; set; }

        public bool HasSoundEmitter { get; set; }
        public string Save()
        {
            return "";
        }

        public bool Load(string str)
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

            //To satisfy the condition, that you have to loose after some time
            mOuterRange += 0.001f * gameTime.ElapsedGameTime.Milliseconds / 16.0f;
            mInnerRange += 0.001f * gameTime.ElapsedGameTime.Milliseconds / 16.0f;

            var inRange = mScene.mQuadTree.QueryRectangle(Actor.mBoundingRectangle.GetAxisAlignedRectangle((int)Math.Floor(mOuterRange)));

            mDistanceSilverbackHut = Vector3.Distance(mSilverback.Actor.ModelMatrix.Translation, Actor.ModelMatrix.Translation);

            if (!mSpawned && mDistanceSilverbackHut <= mOuterRange)
            {
                if (!mTechDemo)
                {

                    mNumLumberjacks = LumberChoice.NumLumberjacks(HP);

                    // Spawning is not enough, we should give them a target
                    // Also I suggest that we use something like an inner and an outer range.
                    // If the distance to the siverback is less than the outer range, we spawn the lumberjacks
                    // If the distance to the silverback is less than the inner range, we set the targets of the lumberjacks
                    for (var i = 0; i < mNumLumberjacks; i++)
                    {
                        var lumberjack = new Lumberjack.Lumberjack(mLumberjackMesh, mScene.mTerrain, mScene.mQuadTree, Actor.ModelMatrix.Translation, 4.0f, mInnerRange, mScene, mSilverback, false);
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
                        var killer = new DoubleAxeKiller(mDoubleAxeKillerMesh, mScene.mTerrain, mScene.mQuadTree, Actor.ModelMatrix.Translation, 4.0f, mInnerRange, mScene, mSilverback);
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

                }
                else
                {
                    for (var i = 0; i < HP; i++)
                    {
                        var lumberjack = new Lumberjack.Lumberjack(mLumberjackMesh, mScene.mTerrain, mScene.mQuadTree, Actor.ModelMatrix.Translation, 4.0f, mInnerRange, mScene, mSilverback, true);
                        lumberjack.Hut = this;
                        mLumberjacks.Add(lumberjack);
                        lumberjack.StayInRange();
                        mScene.Add(lumberjack);
                    }
                }

                mSpawned = true;

            }
            else
            {

                // Instead of calculating the range to the silverback, we should do a query on the quad tree
                // to determine if there are any apes in the range. We should also do this for spawning.
                mDistanceSilverbackHut = Vector3.Distance(mSilverback.Actor.ModelMatrix.Translation, Actor.ModelMatrix.Translation);

                // Set the target to one of the lumberjacks and let them return of the lumberjack is out of range
                if (mDistanceSilverbackHut <= mInnerRange)
                {
                    
                    foreach (var lumberjack in mLumberjacks)
                    {
                        if (lumberjack.IsMoving)
                        {
                            lumberjack.SetTarget(mSilverback.Actor.ModelMatrix.Translation);
                        }
                    }
                    
                    foreach (var doubleAxeKiller in mDoubleAxeKillers)
                    {
                        if (doubleAxeKiller.IsMoving)
                        {
                            doubleAxeKiller.SetTarget(mSilverback.Actor.ModelMatrix.Translation); 
                        }
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


        public Hut(Mesh hut, Mesh lumberjack, Mesh doubleAxeKiller, Mesh axeMesh, Silverback silverback, Scene scene, int hp, float innerRange, float outerRange, bool techdemo)
        {
            mTechDemo = techdemo;
            mId = mAmount++;

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

            mInnerRange = innerRange;
            mOuterRange = outerRange;

            mLumberjacks = new List<Lumberjack.Lumberjack>();
            mDoubleAxeKillers = new List<DoubleAxeKiller>();

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("mNumLumberjacks", mNumLumberjacks);
            info.AddValue("mSpawned", mSpawned);
            info.AddValue("mScene", mScene);
            info.AddValue("mLumberjackMesh", mLumberjackMesh);
            info.AddValue("mDoubleAxeKillerMesh", mDoubleAxeKillerMesh);
            info.AddValue("mAxeMesh", mAxeMesh);
            info.AddValue("mSilverback", mSilverback);
            info.AddValue("mDistanceSilverbackHut", mDistanceSilverbackHut);
            info.AddValue("mWasInInnerRange", mWasInInnerRange);
            info.AddValue("mLumberjacks", mLumberjacks);
            info.AddValue("mDoubleAxeKillers", mDoubleAxeKillers);
            info.AddValue("HP", HP);
            info.AddValue("IsAlive", IsAlive);
            info.AddValue("Actor", Actor);
            info.AddValue("Name", Name);
            info.AddValue("HasSoundEmitter", HasSoundEmitter);
        }

        public Hut(SerializationInfo info, StreamingContext context)
        {
            mNumLumberjacks = (int)info.GetValue("mNumLumberjacks", typeof(int));
            mSpawned = (bool)info.GetValue("mSpawned", typeof(bool));
            mScene = (Scene)info.GetValue("mScene", typeof(Scene));
            mLumberjackMesh = (Mesh)info.GetValue("mLumberjackMesh", typeof(Mesh));
            mDoubleAxeKillerMesh = (Mesh)info.GetValue("mDoubleAxeKillerMesh", typeof(Mesh));
            mAxeMesh = (Mesh)info.GetValue("mAxeMesh", typeof(Mesh));
            mSilverback = (Silverback)info.GetValue("mSilverback", typeof(Silverback));
            mDistanceSilverbackHut = (float)info.GetValue("mDistanceSilverbackHut", typeof(float));
            mWasInInnerRange = (bool)info.GetValue("mWasInInnerRange", typeof(bool));
            mLumberjacks = (List<Lumberjack.Lumberjack>)info.GetValue("mLumberjacks", typeof(List<Lumberjack.Lumberjack>));
            mDoubleAxeKillers = (List<DoubleAxeKiller>)info.GetValue("mDoubleAxeKillers", typeof(List<DoubleAxeKiller>));
            HP = (int)info.GetValue("HP", typeof(int));
            IsAlive = (bool)info.GetValue("IsAlive", typeof(bool));
            Actor = (Actor)info.GetValue("Actor", typeof(Actor));
            Name = (string)info.GetValue("Name", typeof(string));
            HasSoundEmitter = (bool)info.GetValue("HasSoundEmitter", typeof(bool));
        }

        public Hut()
        {
            if (mLumberjacks == null)
            {
                mLumberjacks = new List<Lumberjack.Lumberjack>();
            }

            if (mDoubleAxeKillers == null)
            {
                mDoubleAxeKillers = new List<DoubleAxeKiller>();
            }
        }

        [OnDeserialized()]
        internal void OnSerializedMethod(StreamingContext context)
        {
            Actor.IActor = this;
        }
    }

}
