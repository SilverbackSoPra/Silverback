using System;
using System.Globalization;
using System.IO;
using LevelEditor.Engine;
using LevelEditor.Engine.Loader;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LevelEditor.Engine.Mesh;
using Microsoft.Xna.Framework;
using LevelEditor.Objects.Ape;
using LevelEditor.Objects.Lumberjack;
using LevelEditor.Objects.Ape.SubApe;
using LevelEditor.Screen;
using Microsoft.Xna.Framework.Audio;
using LevelEditor.Sound;
using LevelEditor.Objects;
using System.Collections.Generic;
using LevelEditor.Collision;

namespace LevelEditor
{
    /// <inheritdoc />
    internal sealed class Level : Scene
    {

        public string mLevelTitle;
        public string mLevelStory;
        
        public readonly ModelLoader mModelLoader;
        
        private ActorBatch mLumberjacks;
        private ActorBatch mDoubleAxeKillers;

        public readonly Silverback mSilverback;
        public ActorBatch mBaboonBatch;
        public ActorBatch mCapuchinBatch;
        public ActorBatch mChimpanezzeBatch;
        public ActorBatch mGibbonBatch;
        public ActorBatch mOrangutanBatch;
        public ActorBatch mSilverbackBatch;

        public Mesh mSilverbackMesh;
        public Mesh mBaboonMesh;
        public Mesh mCapuchinMesh;
        public Mesh mOrangUtanMesh;
        public Mesh mGibbonMesh;
        public Mesh mChimpanezzeMesh;
        public Mesh mHutMesh;
        public Mesh mLumberjackMesh;
        public Mesh mDoubleAxeKillerMesh;
        public Mesh mAxeMesh;

        public List<Hut> mHuts;

        private readonly ContentManager mContentManager;
        private readonly GraphicsDevice mDevice;
        private readonly bool mEditMode;

        private List<Actor> mRemovableActors;
        

        public Level(ContentManager contentManager, GraphicsDevice device, bool editMode) : base()
        {

            mContentManager = contentManager;
            mDevice = device;
            mEditMode = editMode;

            var currentDirectoryPath = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory("../../../../Content");

            mModelLoader = new ModelLoader(device);
            mQuadTree = new QuadTree<Actor>(new Rectangle(-128, -128, 256, 256), 4, 7);

            mHutMesh = mModelLoader.LoadMesh("Mesh/spawningcabin_scaled 0.01.fbx");

            if (!mEditMode)
            {

                // Load silverback
                mSilverback = new Silverback(new Vector3(0.0f), new Vector2(0.0f))
                {
                    Actor = new Actor(mModelLoader.LoadMesh("Mesh/gorilla_idle.fbx"))
                };
                mSilverback.Actor.mAnimator.SetStandardAnimation("idle");
                mModelLoader.LoadAnimation(mSilverback.Actor.mMesh, "Mesh/gorilla_walking.fbx");
                mModelLoader.LoadAnimation(mSilverback.Actor.mMesh, "Mesh/gorilla_smash.fbx");
                Add(mSilverback);
                mQuadTree.Insert(mSilverback.Actor, mSilverback.Actor.mBoundingRectangle.GetAxisAlignedRectangle(1));

                // Load all the other meshes
                // Load Lumberjack mesh
                mLumberjackMesh = mModelLoader.LoadMesh("Mesh/lumberjack_idle.fbx");
                mModelLoader.LoadAnimation(mLumberjackMesh, "Mesh/lumberjack_walk.fbx");
                mModelLoader.LoadAnimation(mLumberjackMesh, "Mesh/lumberjack_hit.fbx");
                mModelLoader.LoadAnimation(mLumberjackMesh, "Mesh/lumberjack_run.fbx");

                mLumberjacks = new ActorBatch(mLumberjackMesh, true);
                mActorBatches.Add(mLumberjacks);

                // Load DoubleAxeKiller mesh
                mDoubleAxeKillerMesh = mModelLoader.LoadMesh("Mesh/lumberjack_distance_idle.fbx");
                mModelLoader.LoadAnimation(mDoubleAxeKillerMesh, "Mesh/lumberjack_distance_walk.fbx");
                mModelLoader.LoadAnimation(mDoubleAxeKillerMesh, "Mesh/lumberjack_distance_hit.fbx");
                mModelLoader.LoadAnimation(mDoubleAxeKillerMesh, "Mesh/lumberjack_distance_run.fbx");

                mDoubleAxeKillers = new ActorBatch(mDoubleAxeKillerMesh, true);
                mActorBatches.Add(mDoubleAxeKillers);

                // Load Baboon Mesh
                mBaboonMesh = mModelLoader.LoadMesh("Mesh/gibbon_idle.fbx");
                mModelLoader.LoadAnimation(mBaboonMesh, "Mesh/gibbon_walk.fbx");
                mModelLoader.LoadAnimation(mBaboonMesh, "Mesh/gibbon_singing.fbx");

                mBaboonBatch = new ActorBatch(mBaboonMesh, true);
                mActorBatches.Add(mBaboonBatch);

                // Load Capuchin Mesh
                mCapuchinMesh = mModelLoader.LoadMesh("Mesh/kapuziner_idle.fbx");
                mModelLoader.LoadAnimation(mCapuchinMesh, "Mesh/kapuziner_walk.fbx");
                mModelLoader.LoadAnimation(mCapuchinMesh, "Mesh/kapuziner_heal.fbx");

                mCapuchinBatch = new ActorBatch(mCapuchinMesh, true);
                mActorBatches.Add(mCapuchinBatch);

                // Load Chimpanezze Mesh
                mChimpanezzeMesh = mModelLoader.LoadMesh("Mesh/chimp_idle.fbx");
                mModelLoader.LoadAnimation(mChimpanezzeMesh, "Mesh/chimp_walk.fbx");
                mModelLoader.LoadAnimation(mChimpanezzeMesh, "Mesh/chimp_attack.fbx");

                mChimpanezzeBatch = new ActorBatch(mChimpanezzeMesh, true);
                mActorBatches.Add(mChimpanezzeBatch);

                // Load Gibbon Mesh
                mGibbonMesh = mModelLoader.LoadMesh("Mesh/Gibbon_idle.fbx");
                mModelLoader.LoadAnimation(mGibbonMesh, "Mesh/Gibbon_walk.fbx");
                mModelLoader.LoadAnimation(mGibbonMesh, "Mesh/Gibbon_singing.fbx");

                mGibbonBatch = new ActorBatch(mGibbonMesh, true);
                mActorBatches.Add(mGibbonBatch);


                // Load Orang Utan Mesh
                mOrangUtanMesh = mModelLoader.LoadMesh("Mesh/orangutan_idle.fbx");
                mModelLoader.LoadAnimation(mOrangUtanMesh, "Mesh/orangutan_walk.fbx");
                mModelLoader.LoadAnimation(mOrangUtanMesh, "Mesh/orangutan_throw.fbx");

                mOrangutanBatch = new ActorBatch(mOrangUtanMesh, true);
                mActorBatches.Add(mOrangutanBatch);

                mAxeMesh = mModelLoader.LoadMesh("Mesh/axe.fbx");

                mRemovableActors = new List<Actor>();
                mHuts = new List<Hut>();
                
            }

            mPostProcessing.mBloom.Activated = Options.Bloom;
            mPostProcessing.mBloom.mPasses = Options.BloomPasses;

            mPostProcessing.mFxaa.Activated = Options.Fxaa;

            mSky.Light.mShadow.mActivated = Options.Shadows;
            mSky.Light.mShadow.mDistance = Options.ShadowDistance;
            mSky.Light.mShadow.mNumSamples = Options.ShadowSamples;

            mFog.mDistance = Options.ViewingDistance - 10.0f;

            Directory.SetCurrentDirectory(currentDirectoryPath);

        }

        /// <summary>
        /// Load a file, read relevant infos from it and build level
        /// </summary>
        public void Load(string filename)
        {

            /* File is build up like this line by line(for now)
             * 1. Light information x,y,z
             * 1. Heightmap path
             * 2. Heightmap texture path
             * 3. M Mesh1 (including relative path to mesh? and other relevant informations)
             * 4. A Actor1 using Mesh1 is represented by following information: x,z,rotation,(scale?)
             * 5. A Actor2 using Mesh1 is represented by following information: x,z,rotation,(scale?)
             * 6. ......
             * n. Mesh2 (including relative path to mesh? and other relevant informations)
             * k. EOF (end of file, stream reader stops reading)
             */
            try
            {
                using (var sr = new StreamReader(filename))
                {

                    var directoryPath = Path.GetDirectoryName(filename);
                    var currentDirectoryPath = Directory.GetCurrentDirectory();

                    Directory.SetCurrentDirectory(directoryPath);

                    // Load/build terrain
                    mTerrain = new Terrain(mContentManager, mDevice, sr.ReadLine(), sr.ReadLine());
                    var mCollisionRectangles = new List<CollisionRectangle>();

                    string line;

                    Mesh mesh = null;
                    // Load the rest (objects and actors and light)
                    while ((line = sr.ReadLine()) != null)
                    {

                        var lineSub = line.Substring(2);

                        if (line.StartsWith("M "))
                        {
                            // Read the path to the mesh
                            mesh = mModelLoader.LoadMesh(lineSub);
                            mesh.Path = lineSub;

                        }
                        else if (line.StartsWith("A "))
                        {

                            var strings = lineSub.Split(' ');
                            var actorLocation = strings[0].Split(',');
                            var actorRotation = strings[1].Split(',');
                            var actorOffset = strings[2];

                            var x = StringToFloat(actorLocation[0]);
                            var z = StringToFloat(actorLocation[1]);

                            var qX = StringToFloat(actorRotation[0]);
                            var qY = StringToFloat(actorRotation[1]);
                            var qZ = StringToFloat(actorRotation[2]);
                            var qW = StringToFloat(actorRotation[3]);

                            var offset = StringToFloat(actorOffset);

                            var vector = new Vector3(x, 0.0f, z);
                            vector.Y = mTerrain.GetHeight(vector) - offset;

                            var quaternion = new Quaternion(qX, qY, qZ, qW);

                            var matrix = Matrix.CreateFromQuaternion(quaternion) * Matrix.CreateTranslation(vector);
                            var actor = new Actor(mesh) { ModelMatrix = matrix };

                            // We dont want to allow actors which hover above the ground to be in the collision caluc
                            if (-offset < mesh.mMeshData.mBoundingSphere.Radius)
                            {
                                mQuadTree.Insert(actor, actor.mBoundingRectangle.GetAxisAlignedRectangle(1));
                                mCollisionRectangles.Add(actor.mBoundingRectangle);
                            }
                            
                            Add(actor);

                        }
                        else if (line.StartsWith("L "))
                        {

                            var strings = lineSub.Split(' ');

                            var time = strings[0];
                            var lightColor = strings[1].Split(',');
                            var lightAmbient = strings[2];

                            var light = mSky.Light;

                            mSky.mTime = StringToFloat(time);

                            light.mColor.X = StringToFloat(lightColor[0]);
                            light.mColor.Y = StringToFloat(lightColor[1]);
                            light.mColor.Z = StringToFloat(lightColor[2]);

                            light.mAmbient = StringToFloat(lightAmbient);

                        }
                        else if (line.StartsWith("B "))
                        {

                            var strings = lineSub.Split(' ');

                            var threshold = strings[0];
                            var power = strings[1];
                            var intensity = strings[2];

                            mPostProcessing.mBloom.mThreshold = StringToFloat(threshold);
                            mPostProcessing.mBloom.mPower = StringToFloat(power);
                            mPostProcessing.mBloom.mIntensity = StringToFloat(intensity);

                        }
                        else if (line.StartsWith("F "))
                        {

                            var strings = lineSub.Split(' ');

                            var lumaThreshold = strings[0];
                            var lumaThresholdMin = strings[1];

                            mPostProcessing.mFxaa.mLumaThreshold = StringToFloat(lumaThreshold);
                            mPostProcessing.mFxaa.mLumaThresholdMin = StringToFloat(lumaThresholdMin);

                        }
                        else if (line.StartsWith("T "))
                        {

                            mLevelTitle = lineSub;

                        }
                        else if (line.StartsWith("S "))
                        {

                            var story = lineSub.Replace('|', '\\');
                            mLevelStory = story;                            

                        }
                        else if (line.StartsWith("H "))
                        {

                            var strings = lineSub.Split(' ');

                            var hutPosition = strings[0].Split(',');
                            var hutHealthPoints = strings[1];

                            var x = StringToFloat(hutPosition[0]);
                            var z = StringToFloat(hutPosition[1]);

                            var hp = Convert.ToInt32(hutHealthPoints);

                            var position = new Vector3(x, 0.0f, z);
                            position.Y = mTerrain.GetHeight(position);

                            var matrix = Matrix.CreateTranslation(position);

                            if (!mEditMode) {
                                var hut = new Hut(mHutMesh, mLumberjackMesh, mDoubleAxeKillerMesh, mAxeMesh, mSilverback, this, hp);
                                hut.Actor.ModelMatrix = matrix;
                                mQuadTree.Insert(hut.Actor, hut.Actor.mBoundingRectangle.GetAxisAlignedRectangle(1));
                                mHuts.Add(hut);
                                Add(hut);
                            }
                            else
                            {
                                Add(new Actor(mHutMesh) { ModelMatrix = matrix });
                            }

                        }

                    }

                    sr.Close();

                    Add(mTerrain);
                    mVisibilityGraph = new Pathfinding.VisibilityGraph(mCollisionRectangles, false);
                    // mBruteVisibilityGraph = new Pathfinding.VisibilityGraph(mCollisionRectangles, true);

                    Directory.SetCurrentDirectory(currentDirectoryPath);                    

                }

            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("This file could not be read:");
                Console.WriteLine(e.Message);
                throw;
            }
            
        }

        public void Save(string filename)
        {
            // Save in a textfile; first row contains HeightMapPath; second TexturePath; then write for all objects on map
            // x, z coordinate and a rotation value; probably changed later
            try
            {
                using (StreamWriter sw = new StreamWriter(filename))
                {

                    sw.WriteLine("Terrain/" + Path.GetFileName(mTerrain.mHeightMapPath));
                    sw.WriteLine("Terrain/" + Path.GetFileName(mTerrain.mTexturePath));

                    var light = mSky.Light;
                    var bloom = mPostProcessing.mBloom;
                    var fxaa = mPostProcessing.mFxaa;

                    // Save the light
                    var time = FloatToString(mSky.mTime);

                    var r = FloatToString(light.mColor.X);
                    var g = FloatToString(light.mColor.Y);
                    var b = FloatToString(light.mColor.Z);

                    var ambient = FloatToString(light.mAmbient);

                    sw.WriteLine("L " + time + " " + r + "," + g + "," + b + " " + ambient);

                    // Save the bloom
                    var threshold = FloatToString(bloom.mThreshold);
                    var power = FloatToString(bloom.mPower);
                    var intensity = FloatToString(bloom.mIntensity);

                    sw.WriteLine("B " + threshold + " " + power + " " + intensity);

                    // Save the FXAA
                    var lumaThreshold = FloatToString(fxaa.mLumaThreshold);
                    var lumaThresholdMin = FloatToString(fxaa.mLumaThresholdMin);

                    sw.WriteLine("F " + lumaThreshold + " " + lumaThresholdMin);

                    // Write level text
                    if (mLevelStory != null)
                    {
                        var text = mLevelStory.Replace('\\', '|');
                        sw.WriteLine("S " + text);
                    }

                    // Iterate over all actor batches. An actor batch holds all instances of a mesh.
                    foreach (var actorbatch in mActorBatches)
                    {

                        if (actorbatch.mMesh == mTerrain.Actor.mMesh || actorbatch.mMesh == mHutMesh)
                        {
                            continue;
                        }

                        if (actorbatch.mActors.Count > 0)
                        {

                            var path = Path.GetFileName(actorbatch.mMesh.Path);
                            sw.WriteLine("M " + "Mesh/" +  path);

                            foreach (var actor in actorbatch.mActors)
                            {

                                var x = FloatToString(actor.ModelMatrix.Translation.X);
                                var z = FloatToString(actor.ModelMatrix.Translation.Z);

                                var qX = FloatToString(actor.ModelMatrix.Rotation.X);
                                var qY = FloatToString(actor.ModelMatrix.Rotation.Y);
                                var qZ = FloatToString(actor.ModelMatrix.Rotation.Z);
                                var qW = FloatToString(actor.ModelMatrix.Rotation.W);

                                var offset = mTerrain.GetHeight(actor.ModelMatrix.Translation) - actor.ModelMatrix.Translation.Y;

                                sw.WriteLine("A " + x + "," + z + " " + qX + "," + qY + "," + qZ + "," + qW + " " + FloatToString(offset));

                            }

                        }
                    }

                }
            }
            catch (Exception)
            {
                Console.WriteLine("This file could not be saved. Possible reasons are: This file is protected, " +
                                  "the pathname is too long, or there is no space available.");
                throw;
            }
        }

        public void ReloadTerrain()
        {

            mTerrain.Reload();

            foreach (var actorBatch in mActorBatches)
            {

                if (actorBatch.mMesh == mTerrain.Actor.mMesh)
                {
                    continue;
                }

                foreach (var actor in actorBatch.mActors)
                {

                    var translation = actor.ModelMatrix.Translation;
                    var rotation = actor.ModelMatrix.Rotation;
                    var scale = actor.ModelMatrix.Scale;

                    translation.Y = mTerrain.GetHeight(translation);

                    actor.ModelMatrix = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(translation);

                }

            }

        }

        public void Update(Camera camera, CameraHandler handler, GameTime gameTime)
        {

            var oldHandlerLocation = handler.mLocation;

            if (!mEditMode)
            {
                
                camera.mLocation.Y = mTerrain.GetHeight(camera.mLocation) + 1.5f;

                // We need to calculate the collision of the camera with the terrain
                var location = camera.mLocation - camera.mThirdPersonDistance * camera.Direction;

                var height = mTerrain.GetHeight(location);
                var relativeHeight = height - camera.mLocation.Y + 1.0f;
                var terrainAngle = Math.Sinh(relativeHeight / camera.mThirdPersonDistance);

                if (camera.mRotation.Y < terrainAngle)
                {
                    camera.mRotation.Y = (float) terrainAngle;
                    handler.mRotation.Y = (float) terrainAngle;
                }

                if (camera.mRotation.Y > Math.PI / 3.5f)
                {
                    camera.mRotation.Y = (float)Math.PI / 3.5f;
                    handler.mRotation.Y = (float)Math.PI / 3.5f;
                }

            }

            Update(camera, gameTime);

            if (mTerrain != null)
            {
                // We have to disable this every frame
                mTerrain.Actor.mCastShadow = false;
            }

            if (!mEditMode)
            {
                //Remove Dead actors from scene
                foreach (var actorbatch in mActorBatches)
                {
                    if (actorbatch.mActors.Count > 0)
                    {

                        foreach (var actor in actorbatch.mActors)
                        {
                            if (actor.IActor is IEscape)
                            {
                                var current = (IEscape)actor.IActor;
                                if (current.IsAlive && current.HP <= 0)
                                {
                                    current.IsAlive = false;
                                    mRemovableActors.Add(actor);
                                }
                            }
                        }
                        if (mRemovableActors.Count > 0)
                        {

                            foreach (var actor in mRemovableActors)
                            {
                                actorbatch.mActors.Remove(actor);
                                mQuadTree.Remove(actor, actor.mBoundingRectangle.GetAxisAlignedRectangle(1));
                                //var toDelete = (IEscape)actor.IActor;
                                //toDelete.Escape();
                            }
                        }
                    }
                }

                mSilverback.Update(mTerrain, camera, handler, this);

            }

        }

        public void RemoveActor(ActorBatch actorbatch, Actor actor)
        {
            actorbatch.mActors.Remove(actor);
        }

        protected override void Dispose(bool disposing)
        {

            if (mDisposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var actorBatch in mActorBatches)
                {
                    actorBatch.mMesh.Dispose();
                }
            }

            base.Dispose(disposing);

        }

        /// <summary>
        /// We want to convert the floating point number to a string and ignore the language settings
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string FloatToString(float value)
        {

            return value.ToString(CultureInfo.InvariantCulture);

        }

        /// <summary>
        /// We want to convert the string to a floating point number and ignore the language settings
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private float StringToFloat(string value)
        {

            return float.Parse(value, CultureInfo.InvariantCulture);

        }

    }
    
}
