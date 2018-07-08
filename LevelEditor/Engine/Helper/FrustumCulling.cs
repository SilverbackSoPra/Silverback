using System;
using Microsoft.Xna.Framework;

namespace LevelEditor.Engine.Helper
{

    /// <summary>
    /// Frustum culling reduces the number of actors rendered in the scene.
    /// </summary>
    class FrustumCulling
    {

        private static double sTang;

        private static Vector3 sX;
        private static Vector3 sY;
        private static Vector3 sZ;

        /// <summary>
        /// Disables all actors which are not visible to the camera
        /// </summary>
        /// <param name="scene">The scene where the actors should be disabled</param>
        /// <param name="camera">The camera from which the scene should be rendered</param>
        /// <returns></returns>
        public static int CullActorsOutsideFrustum(Scene scene, Camera camera)
        {

            var visibleActors = 0;

            CalculateCameraFrustum(camera);

            foreach (var actorBatch in scene.mActorBatches)
            {

                var boundingSphere = actorBatch.mMesh.mMeshData.mBoundingSphere;

                foreach (var actor in actorBatch.mActors)
                {

                    /*
                    // We don't need this right now, because we assume that the scale of all actors is 1.0f
                    var vec = new Vector3();
                    vec.X = new Vector3(actor.ModelMatrix.M11, actor.ModelMatrix.M21, actor.ModelMatrix.M31).Length();
                    vec.Y = new Vector3(actor.ModelMatrix.M12, actor.ModelMatrix.M22, actor.ModelMatrix.M32).Length();
                    vec.Z = new Vector3(actor.ModelMatrix.M13, actor.ModelMatrix.M23, actor.ModelMatrix.M33).Length();

                    var scale = MathExtension.Max(vec);
                    */

                    var translation = actor.ModelMatrix.Translation + boundingSphere.Center;
                    var distance = 0.0f;

                    actor.mRender = IsSphereVisible(translation, boundingSphere.Radius, 1.0f, camera, camera.mFarPlane, ref distance);

                    if (actor.mRender)
                    {
                        visibleActors++;
                    }

                }
            }

            return visibleActors;

        }

        /// <summary>
        /// Culls all shadow casters which aren't visible in x and y direction from the lights point of view
        /// </summary>
        /// <param name="scene">The scene where the shadow casters should be culled</param>
        /// <returns>The number of visible shadow casters</returns>
        public static int CullShadowCastersOutsideFrustum(Scene scene)
        {
            var visibleActors = 0;
            var light = scene.mSky.Light;

            CalculateLightFrustum(light);

            foreach (var actorBatch in scene.mActorBatches)
            {

                var boundingSphere = actorBatch.mMesh.mMeshData.mBoundingSphere;

                foreach (var actor in actorBatch.mActors)
                {

                    var translation = actor.ModelMatrix.Translation + boundingSphere.Center;

                    actor.mCastShadow = IsSphereVisible(translation, boundingSphere.Radius, 1.0f, light);

                    if (actor.mCastShadow)
                    {
                        visibleActors++;
                    }

                }
            }

            return visibleActors;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grass"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static int CullGrassPatchesOutsideFrustum(Grass grass, Camera camera)
        {

            var visiblePatches = 0;

            CalculateCameraFrustum(camera);

            foreach (var patch in grass.mPatches)
            {
                patch.mRender = IsSphereVisible(patch.mPosition, patch.mRadius, 2.0f, camera, camera.mFarPlane, ref patch.mDistance);

                if (patch.mRender)
                {
                    visiblePatches++;
                }
            }

            return visiblePatches;

        }

        private static bool IsSphereVisible(Vector3 point, float radius, float scale, Camera camera, float maxDistance, ref float distance)
        {

            var d = radius * scale;

            point = Vector3.Transform(point, camera.mViewMatrix);

            var z = -point.Z;
            distance = z;

            if (z - d > maxDistance || z + d < camera.mNearPlane)
            {
                return false;
            }

            var y = point.Y;
            var localHeight = z * sTang;

            if (y - d > localHeight || y + d < -localHeight)
            {
                return false;
            }

            var x = point.X;
            var localWidth = localHeight * camera.mAspectRatio;

            return !(x - d > localWidth) && !(x + d < -localWidth);

        }

        private static bool IsSphereVisible(Vector3 point, float radius, float scale, Light light)
        {

            var d = radius * scale;

            point = Vector3.Transform(point, light.mShadow.mViewMatrix);

            var min = light.mShadow.mMinProj;
            var max = light.mShadow.mMaxProj;

            var y = point.Y;

            if (y - d > max.Y || y + d < min.Y)
            {
                return false;
            }

            var x = point.X;

            if (x - d > max.X || x + d < min.X)
            {
                return false;
            }

            return true;

        }

        private static void CalculateCameraFrustum(Camera camera)
        {
            sTang = Math.Tan(camera.mFieldOfView * Math.PI / 360.0f);

            sZ = Vector3.Normalize(camera.Direction);
            sX = Vector3.Normalize(Vector3.Cross(camera.Up, sZ));
            sY = Vector3.Normalize(Vector3.Cross(sZ, sX));
        }

        private static void CalculateLightFrustum(Light light)
        {

            var direction = light.mShadow.mShadowCenter - light.mLocation;
            sZ = Vector3.Normalize(direction);
            sX = Vector3.Normalize(Vector3.Cross(Vector3.Up, sZ));
            sY = Vector3.Normalize(Vector3.Cross(sZ, sX));

        }

    }

}
