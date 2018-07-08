using System.Collections.Generic;

namespace LevelEditor.Engine.Mesh
{

    /// <summary>
    /// 
    /// </summary>
    internal sealed class ActorBatch
    {
        public readonly List<Actor> mActors;

        public readonly Mesh mMesh;

        public readonly bool mLock;

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
    }
}