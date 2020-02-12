using System.Collections.Generic;
using LevelEditor.Engine.Helper;
using LevelEditor.Engine.Mesh;
using LevelEditor.Engine.Postprocessing;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using LevelEditor.Collision;
using LevelEditor.Pathfinding;

namespace LevelEditor.Engine
{
    /// <summary>
    /// The Scene class is used to represent a scene. This is useful
    /// if we want many actors in one place e.g for a level.
    /// </summary>
    [Serializable()]
    public class Scene : IDisposable, ISerializable
    {
        [XmlIgnore]
        public readonly List<ActorBatch> mActorBatches;
        [XmlIgnore]
        public QuadTree<Actor> mQuadTree;
        [XmlIgnore]
        public VisibilityGraph mVisibilityGraph;
        [XmlIgnore]
        public VisibilityGraph mBruteVisibilityGraph;

        public readonly PostProcessing mPostProcessing;
        public readonly Sky mSky;
        public readonly Fog mFog;

        public Terrain mTerrain;        

        protected bool mDisposed = false;

        /// <summary>
        /// Constructs a <see cref="Scene"/>.
        /// </summary>
        public Scene()
        {

            mActorBatches = new List<ActorBatch>();

            mPostProcessing = new PostProcessing();
            mSky = new Sky();
            mFog = new Fog();

        }

        /// <summary>
        /// Adds an actor to the scene.
        /// </summary>
        /// <param name="actor"> The actor which you want to add to the scene</param>
        public void Add(Actor actor)
        {

            // Search for the ActorBatch
            var actorBatch = mActorBatches.Find(ele => ele.mMesh == actor.mMesh);

            // If there is no ActorBatch which already uses the mesh of the Actor we
            // need to create a new ActorBatch and add it to mActorBatches
            if (actorBatch == null)
            {
                actorBatch = new ActorBatch(actor.mMesh, false);
                mActorBatches.Add(actorBatch);
            }

            actorBatch.Add(actor);

        }

        /// <summary>
        /// Adds an actor to the scene.
        /// </summary>
        /// <param name="interfaceActor">The object which you want to add to the scene</param>
        public void Add(IActor interfaceActor)
        {
            var actor = interfaceActor.Actor;

            // Search for the ActorBatch
            var actorBatch = mActorBatches.Find(ele => ele.mMesh == actor.mMesh);

            // If there is no ActorBatch which already uses the mesh of the Actor we
            // need to create a new ActorBatch and add it to mActorBatches
            if (actorBatch == null)
            {
                actorBatch = new ActorBatch(actor.mMesh, false);
                mActorBatches.Add(actorBatch);
            }

            actorBatch.Add(actor);

        }


        /// <summary>
        /// Removes an actor from the scene.
        /// </summary>
        /// <param name="actor">The actor which you want to remove from the scene.</param>
        /// <returns>A boolean whether the actor was found in the scene.
        /// If there is no ActorBatch existing the return value will be null</returns>
        public bool? Remove(Actor actor)
        {

            var actorBatch = mActorBatches.Find(ele => ele.mMesh == actor.mMesh);

            var boolean = actorBatch?.Remove(actor);

            var attached = actor.mAnimator.GetAttachedActors();

            foreach(var attachedActor in attached)
            {
                Remove(attachedActor);
            }

            if (actorBatch != null)
            {
                if (actorBatch.mActors.Count == 0 && !actorBatch.mLock)
                {

                    mActorBatches.Remove(actorBatch);

                }
            }

            return boolean;

        }

        /// <summary>
        /// Removes an actor from the scene.
        /// </summary>
        /// <param name="interfaceActor">The object which you want remove from the scene.</param>
        /// <returns>A boolean whether the object wasn't found in the scene.
        /// If there is no ActorBatch existing the return value will be null</returns>
        public bool? Remove(IActor interfaceActor)
        {
            var actor = interfaceActor.Actor;
            
            var actorBatch = mActorBatches.Find(ele => ele.mMesh == actor.mMesh);

            var boolean = actorBatch?.Remove(actor);

            var attached = actor.mAnimator.GetAttachedActors();

            foreach (var attachedActor in attached)
            {
                Remove(attachedActor);
            }

            if (actorBatch != null)
            {
                if (actorBatch.mActors.Count == 0 && !actorBatch.mLock)
                {

                    mActorBatches.Remove(actorBatch);

                }
            }

            return boolean;

        }

        /// <summary>
        /// Updates the scene, including the camera and the animations.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="gameTime">The current game time</param>
        public virtual void Update(Camera camera, GameTime gameTime)
        {

            camera.UpdateView();
            camera.UpdatePerspective();

            var visibleActors = FrustumCulling.CullActorsOutsideFrustum(this, camera);

            mSky.Update(camera);

            var visibleShadowCasters = FrustumCulling.CullShadowCastersOutsideFrustum(this);

            if (mTerrain != null)
            {
                var visibleGrassPatches = FrustumCulling.CullGrassPatchesOutsideFrustum(mTerrain.mGrass, camera);
            }

            foreach (var actorBatch in mActorBatches)
            {
                foreach (var actor in actorBatch.mActors)
                {
                    actor.QuadTree = false;
                }
            }

            var actorBatches = mActorBatches.ToArray();

            foreach (var actorBatch in actorBatches)
            {

                var actors = actorBatch.mActors.ToArray();

                foreach (var actor in actors)
                { 
                    actor.IActor?.Update(gameTime);
                    
                }


                if (!actorBatch.mMesh.mMeshData.mIsSkinned)
                {
                    continue;
                }

                foreach (var actor in actors)
                {
                    actor.mAnimator.Update(gameTime, actor.ModelMatrix, actor.mRender);
                }

            }

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {

            if (mDisposed)
            {
                return;
            }

            if (disposing)
            {
                
            }

            mDisposed = true;

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("mActorBatches", mActorBatches);
            info.AddValue("mQuadTree", mQuadTree);
            info.AddValue("mVisibilityGraph", mVisibilityGraph);
            info.AddValue("mBruteVisibilityGraph", mBruteVisibilityGraph);
            info.AddValue("mBruteVisibilityGraph", mBruteVisibilityGraph);
            info.AddValue("mPostProcessing", mPostProcessing);
            info.AddValue("mSky", mSky);
            info.AddValue("mFog", mFog);
            info.AddValue("mTerrain", mTerrain);
            info.AddValue("mDisposed", mDisposed);
        }

        public Scene(SerializationInfo info, StreamingContext context)
        {
            mActorBatches = (List<ActorBatch>)info.GetValue("mActorBatches", typeof(List<ActorBatch>));
            mQuadTree = (QuadTree<Actor>)info.GetValue("mQuadTree", typeof(QuadTree<Actor>));
            mVisibilityGraph = (VisibilityGraph)info.GetValue("mVisibilityGraph", typeof(VisibilityGraph));
            mBruteVisibilityGraph = (VisibilityGraph)info.GetValue("mBruteVisibilityGraph", typeof(VisibilityGraph));
            mPostProcessing = (PostProcessing)info.GetValue("mPostProcessing", typeof(PostProcessing));
            mSky = (Sky)info.GetValue("mSky", typeof(Sky));
            mFog = (Fog)info.GetValue("mFog", typeof(Fog));
            mTerrain = (Terrain)info.GetValue("mTerrain", typeof(Terrain));
            mDisposed = (bool)info.GetValue("mDisposed", typeof(bool));
        }
    }
}