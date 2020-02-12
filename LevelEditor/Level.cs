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
using LevelEditor.Objects;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using LevelEditor.Collision;
using LevelEditor.Screen;
using LevelEditor.Sound;
using LevelEditor.Objects.Lumberjack;
using LevelEditor.Objects.Ape.SubApe;

namespace LevelEditor
{
    /// <inheritdoc />
    [Serializable()]
    public sealed class Level : Scene, ISerializable, IDisposable
    {

        public string mLevelTitle;
        public string mLevelStory;

        public string mLevelFilename;
        public string mNextLevelFilename;

        public int mSpawnablePrimatesCount = -1;
        public Vector3 mInitialSilverbackLocation;
        
        public readonly ModelLoader mModelLoader;
        
        [XmlIgnore]
        private ActorBatch mLumberjacks;
        [XmlIgnore]
        private ActorBatch mDoubleAxeKillers;

        [XmlIgnore]
        public Silverback mSilverback;
        [XmlIgnore]
        public ActorBatch mBaboonBatch;
        [XmlIgnore]
        public ActorBatch mCapuchinBatch;
        [XmlIgnore]
        public ActorBatch mChimpanezzeBatch;
        [XmlIgnore]
        public ActorBatch mGibbonBatch;
        [XmlIgnore]
        public ActorBatch mOrangutanBatch;
        [XmlIgnore]
        public ActorBatch mSilverbackBatch;

        [XmlIgnore]
        public Mesh mSilverbackMesh;
        [XmlIgnore]
        public Mesh mBaboonMesh;
        [XmlIgnore]
        public Mesh mCapuchinMesh;
        [XmlIgnore]
        public Mesh mOrangUtanMesh;
        [XmlIgnore]
        public Mesh mGibbonMesh;
        [XmlIgnore]
        public Mesh mChimpanezzeMesh;
        [XmlIgnore]
        public Mesh mHutMesh;
        [XmlIgnore]
        public Mesh mLumberjackMesh;
        [XmlIgnore]
        public Mesh mDoubleAxeKillerMesh;
        [XmlIgnore]
        public Mesh mAxeMesh;

        [XmlIgnore]
        public List<Hut> mHuts;

        [XmlIgnore]
        private AudioSource mGlobalSoundSource;
        [XmlIgnore]
        private AudioBuffer mGlobalSoundSourceBuffer;

        private readonly ContentManager mContentManager;
        private readonly SoundManager mSoundManager;
        private readonly GraphicsDevice mDevice;
        private readonly bool mEditMode;
        

        public Level(ContentManager contentManager, SoundManager soundManager, GraphicsDevice device, bool editMode) : base()
        {

            mContentManager = contentManager;
            mSoundManager = soundManager;
            mDevice = device;
            mEditMode = editMode;

            var currentDirectoryPath = Directory.GetCurrentDirectory();

            if (!currentDirectoryPath.EndsWith("Content") && !currentDirectoryPath.EndsWith("Content/") &&
                !currentDirectoryPath.EndsWith("Content\\"))
            {
                Directory.SetCurrentDirectory("../../../../Content");
            }
            
            mModelLoader = new ModelLoader(device);
            mQuadTree = new QuadTree<Actor>(new Rectangle(-128, -128, 256, 256), 4, 7);
            // Load silverback
            mSilverback = new Silverback(new Vector3(0.0f), new Vector2(0.0f), mModelLoader.LoadMesh("Mesh/gorilla_idle.fbx"));
            mSilverback.mQuadTree = mQuadTree;

            mHutMesh = mModelLoader.LoadMesh("Mesh/spawningcabin_scaled 0.015.fbx");
            mHuts = new List<Hut>();
            Hut.mAmount = 0;

            mGlobalSoundSourceBuffer = new AudioBuffer("Audio/ForestSoundMono.wav");
            mGlobalSoundSource = new AudioSource(mGlobalSoundSourceBuffer);
            mGlobalSoundSource.Relative = true;

            mSoundManager.AddSound(mGlobalSoundSource);

            mInitialSilverbackLocation = new Vector3(0.0f);

            if (!mEditMode)
            {

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
                
            }

            mPostProcessing.mBloom.Activated = Options.Bloom;
            mPostProcessing.mBloom.mPasses = Options.BloomPasses;

            mPostProcessing.mFxaa.Activated = Options.Fxaa;

            mSky.Light.mShadow.mActivated = Options.Shadows;
            mSky.Light.mShadow.mDistance = Options.ShadowDistance;
            mSky.Light.mShadow.mNumSamples = Options.ShadowSamples;
            mSky.Light.mShadow.mBias = Options.ShadowBias;

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
            var currentDirectoryPath = Directory.GetCurrentDirectory();

            if (currentDirectoryPath.EndsWith("Content") || currentDirectoryPath.EndsWith("Content/") ||
                currentDirectoryPath.EndsWith("Content\\"))
            {
                Directory.SetCurrentDirectory("..\\bin\\Windows\\x86\\Debug\\");
            }
            mLevelFilename = filename;
            try
            {
                using (var sr = new StreamReader(filename))
                {

                    var directoryPath = Path.GetDirectoryName(filename);

                    Directory.SetCurrentDirectory(directoryPath);

                    // Load/build terrain
                    mTerrain = new Terrain(mContentManager, mDevice, sr.ReadLine(), sr.ReadLine());
                    mTerrain.mGrass.mActivated = Options.GraphicsQuality > 0 ? true : false;
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
                            if (-offset < mesh.mMeshData.mBoundingSphere.Radius && actor.mBoundingRectangle.mCollidable)
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

                            var story = lineSub.Replace('|', '\n');
                            mLevelStory = story;                            

                        }
                        else if (line.StartsWith("H "))
                        {

                            var strings = lineSub.Split(' ');
                            var hutLocation = strings[0].Split(',');
                            var hutRotation = strings[1].Split(',');
                            var hutHp = strings[2];

                            var x = StringToFloat(hutLocation[0]);
                            var z = StringToFloat(hutLocation[1]);

                            var qX = StringToFloat(hutRotation[0]);
                            var qY = StringToFloat(hutRotation[1]);
                            var qZ = StringToFloat(hutRotation[2]);
                            var qW = StringToFloat(hutRotation[3]);

                            var hp = Convert.ToInt32(hutHp);

                            var vector = new Vector3(x, 0.0f, z);
                            vector.Y = mTerrain.GetHeight(vector);

                            var quaternion = new Quaternion(qX, qY, qZ, qW);

                            var matrix = Matrix.CreateFromQuaternion(quaternion) * Matrix.CreateTranslation(vector);

                            var hut = new Hut(mHutMesh, mLumberjackMesh, mDoubleAxeKillerMesh, mAxeMesh, mSilverback, this, hp, 15.0f, 30.0f, false);
                            hut.Actor.ModelMatrix = matrix;
                           
                            mHuts.Add(hut);
                            if (!mEditMode)
                            {
                                mQuadTree.Insert(hut.Actor, hut.Actor.mBoundingRectangle.GetAxisAlignedRectangle(1));
                                Add(hut);
                            }
                            else
                            {
                                hut.Actor.IActor = null;
                                Add(hut.Actor);
                            }

                            mCollisionRectangles.Add(hut.Actor.mBoundingRectangle);

                        }
                        else if (line.StartsWith("W "))
                        {

                            var strings = lineSub.Split(' ');

                            var fogColor = strings[0].Split(',');
                            var fogDistance = strings[1];

                            mFog.mColor.X = StringToFloat(fogColor[0]);
                            mFog.mColor.Y = StringToFloat(fogColor[1]);
                            mFog.mColor.Z = StringToFloat(fogColor[2]);

                            mFog.mDistance = StringToFloat(fogDistance);

                        }
                        else if (line.StartsWith("N "))
                        {

                            mNextLevelFilename = lineSub;

                        }
                        else if (line.StartsWith("P "))
                        {
                            if (mSpawnablePrimatesCount == -1)
                            {
                                mSpawnablePrimatesCount = Convert.ToInt32(lineSub);
                            }

                        }
                        else if (line.StartsWith("I "))
                        {

                            var strings = lineSub.Split(',');

                            var x = StringToFloat(strings[0]);
                            var z = StringToFloat(strings[1]);

                            var location = new Vector3(x, 0.0f, z);
                            location.Y = mTerrain.GetHeight(location);

                            mInitialSilverbackLocation = location;

                            mSilverback.Actor.ModelMatrix = Matrix.CreateTranslation(location);

                        }

                    }

                    sr.Close();

                    Add(mTerrain);
                    mVisibilityGraph = new Pathfinding.VisibilityGraph(mCollisionRectangles, new Rectangle(-128, -128, 256, 256), 1.0f, false);

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

                    // Save the filename of the next level
                    if (mNextLevelFilename != null)
                    {
                        sw.WriteLine("N " + mNextLevelFilename);
                    }

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

                    if (mLevelTitle != null)
                    {
                        var title = mLevelTitle.Replace('\n', ' ');
                        sw.WriteLine("T " + title);
                    }

                    // Write level text
                    if (mLevelStory != null)
                    {
                        var text = mLevelStory.Replace('\n', '|');
                        sw.WriteLine("S " + text);
                    }

                    // Write fog properties
                    r = FloatToString(mFog.mColor.X);
                    g = FloatToString(mFog.mColor.Y);
                    b = FloatToString(mFog.mColor.Z);

                    var fogDistance = FloatToString(mFog.mDistance);

                    sw.WriteLine("W " + r + "," + g + "," + b + " " + fogDistance);

                    // Initial location of the silverback
                    var x = FloatToString(mInitialSilverbackLocation.X);
                    var z = FloatToString(mInitialSilverbackLocation.Z);

                    sw.WriteLine("I " + x + "," + z);

                    // Save the huts
                    foreach (var hut in mHuts)
                    {

                        x = FloatToString(hut.Actor.ModelMatrix.Translation.X);
                        z = FloatToString(hut.Actor.ModelMatrix.Translation.Z);

                        var hp = hut.HP.ToString();

                        var qX = FloatToString(hut.Actor.ModelMatrix.Rotation.X);
                        var qY = FloatToString(hut.Actor.ModelMatrix.Rotation.Y);
                        var qZ = FloatToString(hut.Actor.ModelMatrix.Rotation.Z);
                        var qW = FloatToString(hut.Actor.ModelMatrix.Rotation.W);                

                        sw.WriteLine("H " + x + "," + z + " " + qX + "," + qY + "," + qZ + "," + qW + " " + hp);

                    }

                    // Number of primates which can be spawned
                    sw.WriteLine("P " + mSpawnablePrimatesCount.ToString());

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

                                x = FloatToString(actor.ModelMatrix.Translation.X);
                                z = FloatToString(actor.ModelMatrix.Translation.Z);

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
                mTerrain.Actor.mRender = true;
            }

            if (!mEditMode)
            {

                var removableActors = new List<Actor>();

                //Remove Dead actors from scene
                foreach (var actorbatch in mActorBatches)
                {

                    foreach (var actor in actorbatch.mActors)
                    {
                        if (actor.IActor is IEscape)
                        {
                            var current = (IEscape)actor.IActor;
                            if (current.IsAlive && current.HP <= 0)
                            {
                                if (current is DoubleAxeKiller || current is Lumberjack)
                                {
                                    Statistic.EscapedLumberjacks++;
                                    if (Statistic.EscapedLumberjacks >= 42)
                                    {
                                        Achievements.LumberjacksNightmare = true;
                                    }
                                }
                                if (current is Capuchin || current is Chimpanezee || current is Gibbon || current is OrangUtan)
                                {
                                    Statistic.EscapedApes++;
                                }

                                current.IsAlive = false;
                                current.Escape();
                                removableActors.Add(actor);
                            }
                        }
                    }

                }

                if (mSilverback.IsAlive && mSilverback.HP <= 0)
                {
                    removableActors.Add(mSilverback.Actor);
                }

                foreach (var actor in removableActors)
                {
                    Remove(actor);
                    mQuadTree.Remove(actor, actor.mBoundingRectangle.GetAxisAlignedRectangle(1));
                }

                var removableHuts = new List<Hut>();

                foreach (var hut in mHuts)
                {
                    if (hut.HP <= 0)
                    {
                        removableHuts.Add(hut);
                    }
                }
                foreach (var hut in removableHuts)
                {                    
                    mHuts.Remove(hut);
                    Remove(hut);
                    mQuadTree.Remove(hut.Actor, hut.Actor.mBoundingRectangle.GetAxisAlignedRectangle(1));
                }

                mSilverback.Update(mTerrain, camera, handler, this, gameTime);

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

                mSoundManager.RemoveSound(mGlobalSoundSource);
                mGlobalSoundSource.Dispose();
                mGlobalSoundSourceBuffer.Dispose();

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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("mLevelTitle", mLevelTitle);
            info.AddValue("mLevelStory", mLevelStory);
            info.AddValue("mLumberjacks", mLumberjacks);
            info.AddValue("mDoubleAxeKillers", mDoubleAxeKillers);
            info.AddValue("mSilverback", mSilverback);
            info.AddValue("mBaboonBatch", mBaboonBatch);
            info.AddValue("mCapuchinBatch", mCapuchinBatch);
            info.AddValue("mChimpanezzeBatch", mChimpanezzeBatch);
            info.AddValue("mGibbonBatch", mGibbonBatch);
            info.AddValue("mOrangutanBatch", mOrangutanBatch);
            info.AddValue("mSilverbackBatch", mSilverbackBatch);
            info.AddValue("mSilverbackMesh", mSilverbackMesh);
            info.AddValue("mBaboonMesh", mBaboonMesh);
            info.AddValue("mCapuchinMesh", mCapuchinMesh);
            info.AddValue("mOrangUtanMesh", mOrangUtanMesh);
            info.AddValue("mGibbonMesh", mGibbonMesh);
            info.AddValue("mChimpanezzeMesh", mChimpanezzeMesh);
            info.AddValue("mHutMesh", mHutMesh);
            info.AddValue("mLumberjackMesh", mLumberjackMesh);
            info.AddValue("mDoubleAxeKillerMesh", mDoubleAxeKillerMesh);
            info.AddValue("mAxeMesh", mAxeMesh);
            info.AddValue("mHuts", mHuts);
            info.AddValue("mEditMode", mEditMode);
        }

        public Level(SerializationInfo info, StreamingContext context)
        {
            mLevelTitle = (string)info.GetValue("mLevelTitle", typeof(string));
            mLevelStory = (string)info.GetValue("mLevelStory", typeof(string));
            mLumberjacks = (ActorBatch)info.GetValue("mLumberjacks", typeof(ActorBatch));
            mDoubleAxeKillers = (ActorBatch)info.GetValue("mDoubleAxeKillers", typeof(ActorBatch));
            mSilverback = (Silverback)info.GetValue("mSilverback", typeof(Silverback));
            mBaboonBatch = (ActorBatch)info.GetValue("mBaboonBatch", typeof(ActorBatch));
            mCapuchinBatch = (ActorBatch)info.GetValue("mCapuchinBatch", typeof(ActorBatch));
            mChimpanezzeBatch = (ActorBatch)info.GetValue("mChimpanezzeBatch", typeof(ActorBatch));
            mGibbonBatch = (ActorBatch)info.GetValue("mGibbonBatch", typeof(ActorBatch));
            mOrangutanBatch = (ActorBatch)info.GetValue("mOrangutanBatch", typeof(ActorBatch));
            mSilverbackBatch = (ActorBatch)info.GetValue("mSilverbackBatch", typeof(ActorBatch));
            mSilverbackMesh = (Mesh)info.GetValue("mSilverbackMesh", typeof(Mesh));
            mBaboonMesh = (Mesh)info.GetValue("mBaboonMesh", typeof(Mesh));
            mCapuchinMesh = (Mesh)info.GetValue("mCapuchinMesh", typeof(Mesh));
            mOrangUtanMesh = (Mesh)info.GetValue("mOrangUtanMesh", typeof(Mesh));
            mGibbonMesh = (Mesh)info.GetValue("mGibbonMesh", typeof(Mesh));
            mChimpanezzeMesh = (Mesh)info.GetValue("mChimpanezzeMesh", typeof(Mesh));
            mHutMesh = (Mesh)info.GetValue("mHutMesh", typeof(Mesh));
            mLumberjackMesh = (Mesh)info.GetValue("mLumberjackMesh", typeof(Mesh));
            mDoubleAxeKillerMesh = (Mesh)info.GetValue("mDoubleAxeKillerMesh", typeof(Mesh));
            mAxeMesh = (Mesh)info.GetValue("mAxeMesh", typeof(Mesh));
            mHuts = (List<Hut>)info.GetValue("mHuts", typeof(List<Hut>));
            mEditMode = (bool)info.GetValue("mEditMode", typeof(bool));
        }

        public Level()
        {
            mContentManager = ScreenManager.ContentManager;
            mSoundManager = ScreenManager.SoundManager;
            mDevice = ScreenManager.GraphicsManager.GraphicsDevice;
            mEditMode = false;

            var currentDirectoryPath = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory("../../../../Content");

            mModelLoader = new ModelLoader(mDevice);
            mQuadTree = new QuadTree<Actor>(new Rectangle(-128, -128, 256, 256), 4, 7);
            // Load silverback
            mSilverback = new Silverback(new Vector3(0.0f), new Vector2(0.0f), mModelLoader.LoadMesh("Mesh/gorilla_idle.fbx"));
            mSilverback.mQuadTree = mQuadTree;

            mHutMesh = mModelLoader.LoadMesh("Mesh/spawningcabin_scaled 0.015.fbx");
            mHuts = new List<Hut>();
            Hut.mAmount = 0;

            mGlobalSoundSourceBuffer = new AudioBuffer("Audio/ForestSoundMono.wav");
            mGlobalSoundSource = new AudioSource(mGlobalSoundSourceBuffer);
            mGlobalSoundSource.Relative = true;

            mSoundManager.AddSound(mGlobalSoundSource);

            mInitialSilverbackLocation = new Vector3(0.0f);

            if (!mEditMode)
            {

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

            }

            mPostProcessing.mBloom.Activated = Options.Bloom;
            mPostProcessing.mBloom.mPasses = Options.BloomPasses;

            mPostProcessing.mFxaa.Activated = Options.Fxaa;

            mSky.Light.mShadow.mActivated = Options.Shadows;
            mSky.Light.mShadow.mDistance = Options.ShadowDistance;
            mSky.Light.mShadow.mNumSamples = Options.ShadowSamples;
            mSky.Light.mShadow.mBias = Options.ShadowBias;

            Directory.SetCurrentDirectory(currentDirectoryPath);
        }

        // public new void Dispose()
        // {
        //     mGlobalSoundSourceBuffer.Dispose();
        //     mGlobalSoundSource.Dispose();
        //     
        //     GC.SuppressFinalize(this);
        // }

        // ~Level()
        // {
        //     mGlobalSoundSourceBuffer = null;
        // }
    }
    
}
