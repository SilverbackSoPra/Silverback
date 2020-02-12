using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace LevelEditor.Engine.Mesh
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public sealed class ActorBatch: ISerializable
    {
        public List<Actor> mActors;
        
        public Mesh mMesh;

        public bool mLock;

        /// <summary>
        /// Represents a batch of actors which are using the same mesh.
        /// </summary>
        /// <param name="mesh">The mesh the actors of the actor batch are using</param>
        /// <param name="lockBatch">If this variable is true the scene doesn't remove the batch from the scene if there aren't any actors in the batch</param>
        public ActorBatch(Mesh mesh, bool lockBatch)
        {
            mMesh = mesh;
            mLock = lockBatch;
            mActors = new List<Actor>();
        }

        public ActorBatch()
        {
            mActors = new List<Actor>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actor"></param>
        public void Add(Actor actor)
        {
            mActors.Add(actor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public bool Remove(Actor actor)
        {
            return mActors.Remove(actor);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("mActors", mActors);
            info.AddValue("mMesh", mMesh);
            info.AddValue("mLock", mLock);
        }

        public ActorBatch(SerializationInfo info, StreamingContext context)
        {
            mActors = (List<Actor>)info.GetValue("mActors", typeof(List<Actor>));
            mMesh = (Mesh)info.GetValue("mMesh", typeof(Mesh));
            mLock = (bool)info.GetValue("mLock", typeof(bool));
        }
    }
}