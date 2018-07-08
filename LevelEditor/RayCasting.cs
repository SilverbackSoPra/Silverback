using LevelEditor.Engine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using LevelEditor.Engine.Mesh;
using Microsoft.Xna.Framework.Input;

namespace LevelEditor
{

    /// <summary>
    /// 
    /// </summary>
    internal static class RayCasting
    {
        
        /// <summary>
        /// 
        /// </summary>
        public struct RayIntersection
        {

            public readonly Actor mActor;
            public Vector3 mLocation;
            public readonly float mDistance;
            public readonly bool mIntersected;
            public readonly bool mIsTerrainIntersection;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="actor"></param>
            /// <param name="location"></param>
            /// <param name="distance"></param>
            /// <param name="intersected"></param>
            /// <param name="isTerrainIntersection"></param>
            public RayIntersection(Actor actor, Vector3 location, float distance, bool intersected, bool isTerrainIntersection)
            {
                mActor = actor;
                mLocation = location;
                mDistance = distance;
                mIntersected = intersected;
                mIsTerrainIntersection = isTerrainIntersection;
            }

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="camera"></param>
        /// <param name="scene"></param>
        /// <param name="terrain"></param>
        /// <returns></returns>
        public static RayIntersection CalculateMouseRayIntersection(Viewport viewport, Camera camera, Scene scene, Terrain terrain)
        {

            var mouseState = Mouse.GetState();
            var mousePosition = new Vector2(mouseState.X, mouseState.Y);

            var ray = CalculateRay(viewport, camera, mousePosition);

            var rayTerrainIntersection = CalculateRayTerrainIntersect(ray, camera, terrain);
            var rayActorIntersection = CalculateRayActorIntersect(ray, camera, scene, terrain);

            if (rayTerrainIntersection.mDistance < rayActorIntersection.mDistance)
            {
                return rayTerrainIntersection;
            }
            else
            {
                return rayActorIntersection;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="camera"></param>
        /// <param name="terrain"></param>
        /// <returns></returns>
        public static RayIntersection CalculateMouseRayTerrainIntersection(Viewport viewport, Camera camera, Terrain terrain)
        {

            var mouseState = Mouse.GetState();
            var mousePosition = new Vector2(mouseState.X, mouseState.Y);

            var ray = CalculateRay(viewport, camera, mousePosition);

            return CalculateRayTerrainIntersect(ray, camera, terrain);   

        }

        private static RayIntersection CalculateRayTerrainIntersect(Ray ray, Camera camera, Terrain terrain)
        {

            const float stepLength = .25f;
            var distance = stepLength;

            Vector3 position = ray.Position;
            Vector3 nextPosition = ray.Position + ray.Direction * distance;

            while (distance < camera.mFarPlane)
            {

                nextPosition = ray.Position + ray.Direction * distance;

                if (!IsUnderground(position, terrain) && IsUnderground(nextPosition, terrain))
                {
                    return BinarySearch(ray, terrain, distance - stepLength, distance, 0, 10);
                }

                distance += stepLength;
                position = nextPosition;

            }

            return new RayIntersection(terrain.Actor, new Vector3(0.0f, 0.0f, 0.0f), camera.mFarPlane, false, true);

        }

        private static RayIntersection BinarySearch(Ray ray, Terrain terrain, float start, float finish, int count, int maxRecursion)
        {

            var half = start + (finish - start) / 2.0f;

            if (count >= maxRecursion)
            {
                var position = ray.Position + ray.Direction * half;
                position.Y = terrain.GetHeight(position);
                return new RayIntersection(terrain.Actor, position, half, true, true);
            }

            if (IntersectionInRange(ray, terrain, start, half))
            {
                return BinarySearch(ray, terrain, start, half, count + 1, maxRecursion);
            }
            else
            {
                return BinarySearch(ray, terrain, half, finish, count + 1, maxRecursion);
            }

        }

        private static bool IntersectionInRange(Ray ray, Terrain terrain, float start, float finish)
        {
            Vector3 startPosition = ray.Position + ray.Direction * start;
            Vector3 finishPosition = ray.Position + ray.Direction * finish;
            if (!IsUnderground(startPosition, terrain) && IsUnderground(finishPosition, terrain))
            {
                return true;
            }

            return false;

        }

        private static bool IsUnderground(Vector3 position, Terrain terrain)
        {
            var height = terrain.GetHeight(position);
            return (height > position.Y);
        }

        private static RayIntersection CalculateRayActorIntersect(Ray ray, Camera camera, Scene scene, Terrain terrain)
        {

            Actor nearestActor = null;

            float minDistance = camera.mFarPlane;

            foreach (var actorBatch in scene.mActorBatches)
            {

                if (actorBatch.mMesh != terrain.Actor.mMesh)
                {

                    foreach (var actor in actorBatch.mActors)
                    {

                        if (actor.mRender)
                        {

                            float? distance = IntersectDistance(actor, ray);

                            if (distance != null)
                            {

                                if (minDistance > distance.Value)
                                {
                                    nearestActor = actor;
                                    minDistance = distance.Value;
                                }

                            }

                        }

                    }

                }

            }

            if (nearestActor == null)
            {
                return new RayIntersection(null, Vector3.Zero, camera.mFarPlane, false, false);
            }

            return new RayIntersection(nearestActor, nearestActor.ModelMatrix.Translation, minDistance, true, false);

        }

        private static Ray CalculateRay(Viewport viewport, Camera camera, Vector2 mouseLocation)
        {

            var nearPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                mouseLocation.Y, 0.0f),
                camera.mProjectionMatrix,
                camera.mViewMatrix,
                Matrix.Identity);

            var farPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                mouseLocation.Y, 1.0f),
                camera.mProjectionMatrix,
                camera.mViewMatrix,
                Matrix.Identity);

            var direction = farPoint - nearPoint;

            direction.Normalize();

            return new Ray(nearPoint, direction);

        }

        private static float? IntersectDistance(Actor actor, Ray ray)
        {

            var boundingSphere = new BoundingSphere(actor.ModelMatrix.Translation + actor.mMesh.mMeshData.mBoundingSphere.Center, actor.mMesh.mMeshData.mBoundingSphere.Radius);

            return ray.Intersects(boundingSphere);
        }

    }

}
