using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml.Serialization;
using LevelEditor.Engine;
using LevelEditor.Engine.Mesh;
using LevelEditor.Objects;
using LevelEditor.Objects.Ape;
using LevelEditor.Objects.Ape.SubApe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LevelEditor.UIv2;
using LevelEditor.Sound;

namespace LevelEditor.Screen
{
    sealed class PauseScreen : IScreen
    {
        [XmlIgnore]
        private OptionsMenu mOptionsScreen;


        [XmlIgnore]
        private List<UIv2.Menu> mMenuList;
        private SpriteBatch mSpriteBatch;
        private Texture2D mBackgroundImageOptions;

        [XmlIgnore]
        private SoundEffect mClickSound;

        private GraphicsDevice Device { get; set; }

        [XmlIgnore]
        private HudScreen mHudScreen;

        private GraphicsDevice mGraphicsDevice;

        private int mScreenWidth;
        private int mScreenHeight;

        private Texture2D texture;
        
        // private SpriteFont heading1;

        public ScreenManager ScreenManager { get; set; }
        public SoundManager SoundManager { get; set; }
        public bool IsVisible { get; set; }

        public static string GetSavedGamesPath()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                throw new NotSupportedException();
            }

            IntPtr pathPtr = IntPtr.Zero;
            try
            {
                SHGetKnownFolderPath(ref sFolderDownloads, 0, IntPtr.Zero, out pathPtr);
                return Marshal.PtrToStringUni(pathPtr);
            }
            finally
            {
                Marshal.FreeCoTaskMem(pathPtr);
            }
        }

        private static Guid sFolderDownloads = new Guid("4C5C32FF-BB9D-43b0-B5B4-2D72E54EAAA4");
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetKnownFolderPath(ref Guid id, int flags, IntPtr token, out IntPtr path);

        public PauseScreen(HudScreen screen)
        {
            mHudScreen = screen;
        }

        public void LoadContent(GraphicsDeviceManager deviceManager, ContentManager contentManager, int windowWidth, int windowHeight)
        {

            mScreenWidth = windowWidth;
            mScreenHeight = windowHeight;

            //Load SoundEffects
            mClickSound = contentManager.Load<SoundEffect>("Audio/click2");

            Device = deviceManager.GraphicsDevice;
            mSpriteBatch = new SpriteBatch(deviceManager.GraphicsDevice);

            mOptionsScreen = new OptionsMenu(true);

            // Unlock the framerate. Later we want to remove this piece of code
            mGraphicsDevice = deviceManager.GraphicsDevice;
            mSpriteBatch = new SpriteBatch(deviceManager.GraphicsDevice);

            // Load font
            var font = contentManager.Load<SpriteFont>("Font");
            var subHeaderFont = contentManager.Load<SpriteFont>("SubHeaderFont");
            IsVisible = true;

            // Load background image
            mBackgroundImageOptions = contentManager.Load<Texture2D>("forest");

            mMenuList = new List<UIv2.Menu>();

            var menu = new UIv2.Menu(mGraphicsDevice, 5, 5, 90, 90);
            menu.WithBackground(UIv2.Menu.CreateTexture2D(deviceManager.GraphicsDevice, 50, 30, pixel => new Color(0.0f, 0.0f, 0.0f, 0.2f)), 5, 5, 90, 90);
            mMenuList.Add(menu);

            Texture2D texture2D = UIv2.Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.Black);
            Texture2D texture2DSliderPoint = UIv2.Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.White);

            var heading = new UIv2.Components.Label(mGraphicsDevice, 35, 5, 30, 15, "Pause", subHeaderFont, Color.White);
            heading.FontType = FontManager.FontType.Subheading;
            heading.AddTo(menu);

            // Create new Resume Button

            var resumeButton = new UIv2.Components.Button(mGraphicsDevice, 40, 20, 20, 7, texture2D, "Resume", font, Color.White);
            resumeButton.AddTo(menu);
            resumeButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                mHudScreen.IsVisible = true;
                ScreenManager.Remove(this);
                IsVisible = false;
            });
            
            // Create new Save Button
            var saveButton = new UIv2.Components.Button(mGraphicsDevice, 40, 30, 20, 7, texture2D, "Save Game", font, Color.White);
            saveButton.AddTo(menu);
            saveButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);

                var serializer = new XmlSerializer(typeof(Camera));
                
                Console.WriteLine("########################################################");
                Console.WriteLine("Saving Camera to " + GetSavedGamesPath() + "\\Camera.xml");
                Console.WriteLine("########################################################");


                using (Stream stream = new FileStream(GetSavedGamesPath() + "\\Camera.xml", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    serializer.Serialize(stream, mHudScreen.mGameScreen.mCamera);
                    stream.Close();
                }

                serializer = new XmlSerializer(typeof(Level));
                
                Console.WriteLine("########################################################");
                Console.WriteLine("Saving Level to " + GetSavedGamesPath() + "\\Level.xml");
                Console.WriteLine("########################################################");



                using (Stream stream = new FileStream(GetSavedGamesPath() + "\\Level.xml", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    serializer.Serialize(stream, mHudScreen.mGameScreen.mLevel);
                    stream.Close();
                }

                serializer = new XmlSerializer(typeof(HudScreen));
                
                Console.WriteLine("########################################################");
                Console.WriteLine("Saving Level to " + GetSavedGamesPath() + "\\HudScreen.xml");
                Console.WriteLine("########################################################");



                using (Stream stream = new FileStream(GetSavedGamesPath() + "\\HudScreen.xml", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    serializer.Serialize(stream, mHudScreen);
                    stream.Close();
                }

                serializer = new XmlSerializer(typeof(Silverback));

                Console.WriteLine("########################################################");
                Console.WriteLine("Saving Level to " + GetSavedGamesPath() + "\\Silverback.xml");
                Console.WriteLine("########################################################");

                using (Stream stream = new FileStream(GetSavedGamesPath() + "\\Silverback.xml", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    serializer.Serialize(stream, mHudScreen.mGameScreen.mLevel.mSilverback);
                    stream.Close();
                }


                serializer = new XmlSerializer(typeof(List<Matrix>));
                
                Console.WriteLine("########################################################");
                Console.WriteLine("Saving Level to " + GetSavedGamesPath() + "\\CapuchinPositions.xml");
                Console.WriteLine("########################################################");

                using (Stream stream = new FileStream(GetSavedGamesPath() + "\\CapuchinPositions.xml", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var toSerialize = new List<Matrix>();
                    mHudScreen.mGameScreen.mLevel.mCapuchinBatch.mActors.ForEach(actor => toSerialize.Add(actor.mModelMatrix));
                    serializer.Serialize(stream, toSerialize);
                    stream.Close();
                }
                
                Console.WriteLine("########################################################");
                Console.WriteLine("Saving Level to " + GetSavedGamesPath() + "\\GibbonPositions.xml");
                Console.WriteLine("########################################################");

                using (Stream stream = new FileStream(GetSavedGamesPath() + "\\GibbonPositions.xml", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var toSerialize = new List<Matrix>();
                    mHudScreen.mGameScreen.mLevel.mGibbonBatch.mActors.ForEach(actor => toSerialize.Add(actor.mModelMatrix));
                    serializer.Serialize(stream, toSerialize);
                    stream.Close();
                }
                
                Console.WriteLine("########################################################");
                Console.WriteLine("Saving Level to " + GetSavedGamesPath() + "\\OrangPositions.xml");
                Console.WriteLine("########################################################");

                using (Stream stream = new FileStream(GetSavedGamesPath() + "\\OrangPositions.xml", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var toSerialize = new List<Matrix>();
                    mHudScreen.mGameScreen.mLevel.mOrangutanBatch.mActors.ForEach(actor => toSerialize.Add(actor.mModelMatrix));
                    serializer.Serialize(stream, toSerialize);
                    stream.Close();
                }
                
                Console.WriteLine("########################################################");
                Console.WriteLine("Saving Level to " + GetSavedGamesPath() + "\\ChimpPositions.xml");
                Console.WriteLine("########################################################");

                using (Stream stream = new FileStream(GetSavedGamesPath() + "\\ChimpPositions.xml", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var toSerialize = new List<Matrix>();
                    mHudScreen.mGameScreen.mLevel.mChimpanezzeBatch.mActors.ForEach(actor => toSerialize.Add(actor.mModelMatrix));
                    serializer.Serialize(stream, toSerialize);
                    stream.Close();
                }
                
                Console.WriteLine("########################################################");
                Console.WriteLine("Saving Level to " + GetSavedGamesPath() + "\\Huts.xml");
                Console.WriteLine("########################################################");

                serializer = new XmlSerializer(typeof(List<Hut>));

                using (Stream stream = new FileStream(GetSavedGamesPath() + "\\Huts.xml", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var toSerialize = new List<Hut>();
                    var hutActorBatch = mHudScreen.mGameScreen.mLevel.mActorBatches.Find(ele =>
                        ele.mMesh == mHudScreen.mGameScreen.mLevel.mHutMesh);
                    mHudScreen.mGameScreen.mLevel.mHuts.ForEach(hut => toSerialize.Add(hut));
                    serializer.Serialize(stream, toSerialize);
                    stream.Close();
                }


            });

            // Error label | Savegame
            var errLabel = new UIv2.Components.Label(mGraphicsDevice, 10, 90, 80, 4, "Could not load the savegame", font, Color.White);
            errLabel.FontType = FontManager.FontType.Default;
            errLabel.SetVisibility(false);
            errLabel.AddTo(menu);

            // Create new Load Game Button
            var loadButton = new UIv2.Components.Button(mGraphicsDevice, 40, 40, 20, 7, texture2D, "Load Checkpoint", font, Color.White);
            loadButton.AddTo(menu);
            loadButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                if (!File.Exists(PauseScreen.GetSavedGamesPath() + "\\Camera.xml") ||
                    !File.Exists(PauseScreen.GetSavedGamesPath() + "\\CapuchinPositions.xml") ||
                    !File.Exists(PauseScreen.GetSavedGamesPath() + "\\ChimpPositions.xml") ||
                    !File.Exists(PauseScreen.GetSavedGamesPath() + "\\GibbonPositions.xml") ||
                    !File.Exists(PauseScreen.GetSavedGamesPath() + "\\HudScreen.xml") ||
                    !File.Exists(PauseScreen.GetSavedGamesPath() + "\\Huts.xml") ||
                    !File.Exists(PauseScreen.GetSavedGamesPath() + "\\Level.xml") ||
                    !File.Exists(PauseScreen.GetSavedGamesPath() + "\\OrangPositions.xml") ||
                    !File.Exists(PauseScreen.GetSavedGamesPath() + "\\Silverback.xml"))
                {
                    errLabel.SetVisibility(true);
                    return;
                }
                errLabel.SetVisibility(false);
                ScreenManager.Remove(this);
                ScreenManager.Remove(mHudScreen.mGameScreen);
                ScreenManager.Remove(mHudScreen);

                var loadingScreen = new LoadingScreen(mHudScreen.mLevelPath);
                loadingScreen.LoadSaveGame();
                ScreenManager.Add(loadingScreen);
            });
            
            // Create new Options Button
            var optionsButton = new UIv2.Components.Button(mGraphicsDevice, 40, 50, 20, 7, texture2D, "Options", font, Color.White);
            optionsButton.AddTo(menu);
            optionsButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                ScreenManager.Add(mOptionsScreen);
                IsVisible = false;

            });

            // Create new Back Button
            var backButton = new UIv2.Components.Button(mGraphicsDevice, 40, 60, 20, 7, texture2D, "Main Menu", font, Color.White);
            backButton.AddTo(menu);
            backButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                ScreenManager.Remove(mHudScreen);
                ScreenManager.Remove(this);
                IsVisible = false;

            });

        }

        public void UnloadContent()
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime time)
        {
            // Iterate over all Menu elements
            foreach (var menu in mMenuList)
            {
                if (!menu.mIsVisible)
                {
                    continue;
                }
                menu.CheckRegisteredEvents();
            }
        }

        public void Render(GameTime time)
        {
            if (!IsVisible)
            {
                return;
            }
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackgroundImageOptions, new Rectangle(0, 0, mScreenWidth, mScreenHeight), Color.White);
            mSpriteBatch.End();
            foreach (var menu in mMenuList)
            {
                menu.Render(mSpriteBatch);
            }
        }

        public void ChangeWindowSize(int width, int height)
        {
            if (mMenuList == null)
            {
                return;
            }

            mScreenWidth = width;
            mScreenHeight = height;


            foreach (var menu in mMenuList)
            {
                menu.UpdateResolution(width, height);
            }
        }

        public void ChangeRenderingResolution(int width, int height)
        { }

        public static string ReinitialseSavegame(ref HudScreen hudScreen)
        {
            if (!File.Exists(PauseScreen.GetSavedGamesPath() + "\\Camera.xml") ||
                !File.Exists(PauseScreen.GetSavedGamesPath() + "\\CapuchinPositions.xml") ||
                !File.Exists(PauseScreen.GetSavedGamesPath() + "\\ChimpPositions.xml") ||
                !File.Exists(PauseScreen.GetSavedGamesPath() + "\\GibbonPositions.xml") ||
                !File.Exists(PauseScreen.GetSavedGamesPath() + "\\HudScreen.xml") ||
                !File.Exists(PauseScreen.GetSavedGamesPath() + "\\Huts.xml") ||
                !File.Exists(PauseScreen.GetSavedGamesPath() + "\\Level.xml") ||
                !File.Exists(PauseScreen.GetSavedGamesPath() + "\\OrangPositions.xml") ||
                !File.Exists(PauseScreen.GetSavedGamesPath() + "\\Silverback.xml"))
            {
                return "There are missing files";
            }

            var serializer = new XmlSerializer(typeof(Camera));

            using (var fs = File.OpenRead(GetSavedGamesPath() + "\\Camera.xml"))
            {
                Camera camera = null;
                try
                {
                    camera = (Camera)serializer.Deserialize(fs);
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
                
                hudScreen.mGameScreen.mCamera = camera;
                hudScreen.mGameScreen.mCameraHandler.mCamera = camera;
                fs.Close();
            }
            
            serializer = new XmlSerializer(typeof(Level), new XmlRootAttribute("Level"));

            using (var fs = File.OpenRead(GetSavedGamesPath() + "\\Level.xml"))
            {
                Level level = null;
                try
                {
                    level = (Level)serializer.Deserialize(fs);
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
                hudScreen.mGameScreen.mLevel.Dispose();
                hudScreen.mGameScreen.mLevel = level;
                hudScreen.mGameScreen.mLevel?.Load(level?.mLevelFilename);
                fs.Close();
            }

            if (hudScreen.mGameScreen.mLevel != null)
            {
                var oldSilverback = hudScreen.mGameScreen.mLevel.mSilverback;
            }

            serializer = new XmlSerializer(typeof(Silverback));

            using (var fs = File.OpenRead(PauseScreen.GetSavedGamesPath() + "\\Silverback.xml"))
            {
                Silverback silverback = null;
                try
                {
                    silverback = (Silverback)serializer.Deserialize(fs);
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }

                if (hudScreen.mGameScreen.mLevel != null)
                {
                    if (silverback != null)
                    {
                        hudScreen.mGameScreen.mLevel.mSilverback.HP = silverback.HP;
                    }
                }
                fs.Close();
            }
            
            serializer = new XmlSerializer(typeof(List<Matrix>));

            using (var fs = File.OpenRead(PauseScreen.GetSavedGamesPath() + "\\CapuchinPositions.xml"))
            {
                var positions = new List<Matrix>();
                try
                {
                    positions = (List<Matrix>)serializer.Deserialize(fs);


                    var rand = new Random();

                    foreach (var matrix in positions)
                    {
                        if (hudScreen.mGameScreen.mLevel != null)
                        {
                            var mcapuchin = new Capuchin(hudScreen.mGameScreen.mLevel.mCapuchinMesh,
                                hudScreen.mGameScreen.mLevel.mTerrain,
                                hudScreen.mGameScreen.mLevel.mSilverback,
                                hudScreen.mGameScreen.mLevel,
                                ref rand);
                            mcapuchin.Actor.ModelMatrix = matrix;
                            hudScreen.mGameScreen.mLevel.Add(mcapuchin);
                        }

                        LumberChoice.AddApe(3);
                    }

                }
                catch (Exception)
                {
                    // ignored
                }

                fs.Close();
            }

            using (var fs = File.OpenRead(PauseScreen.GetSavedGamesPath() + "\\GibbonPositions.xml"))
            {
                var positions = new List<Matrix>();
                try
                {
                    positions = (List<Matrix>)serializer.Deserialize(fs);


                    var rand = new Random();

                    foreach (var matrix in positions)
                    {
                        if (hudScreen.mGameScreen.mLevel != null)
                        {
                            var gibbon = new Gibbon(hudScreen.mGameScreen.mLevel.mGibbonMesh,
                                hudScreen.mGameScreen.mLevel.mTerrain,
                                hudScreen.mGameScreen.mLevel.mSilverback,
                                hudScreen.mGameScreen.mLevel,
                                ref rand);
                            gibbon.Actor.ModelMatrix = matrix;
                            hudScreen.mGameScreen.mLevel.Add(gibbon);
                        }

                        LumberChoice.AddApe(4);
                    }

                }
                catch (Exception)
                {
                    // ignored
                }

                fs.Close();
            }

            using (var fs = File.OpenRead(PauseScreen.GetSavedGamesPath() + "\\ChimpPositions.xml"))
            {
                var positions = new List<Matrix>();
                try
                {
                    positions = (List<Matrix>)serializer.Deserialize(fs);


                    var rand = new Random();

                    foreach (var matrix in positions)
                    {
                        if (hudScreen.mGameScreen.mLevel != null)
                        {
                            var chimp = new Chimpanezee(hudScreen.mGameScreen.mLevel.mChimpanezzeMesh,
                                hudScreen.mGameScreen.mLevel.mTerrain,
                                hudScreen.mGameScreen.mLevel.mSilverback,
                                hudScreen.mGameScreen.mLevel,
                                ref rand);
                            chimp.Actor.ModelMatrix = matrix;
                            hudScreen.mGameScreen.mLevel.Add(chimp);
                        }
                        LumberChoice.AddApe(1);
                    }

                }
                catch (Exception)
                {
                    // ignored
                }

                fs.Close();
            }

            using (var fs = File.OpenRead(PauseScreen.GetSavedGamesPath() + "\\OrangPositions.xml"))
            {
                var positions = new List<Matrix>();
                try
                {
                    positions = (List<Matrix>)serializer.Deserialize(fs);


                    var rand = new Random();

                    foreach (var matrix in positions)
                    {
                        if (hudScreen.mGameScreen.mLevel != null)
                        {
                            var orang = new OrangUtan(hudScreen.mGameScreen.mLevel.mOrangUtanMesh,
                                hudScreen.mGameScreen.mLevel.mTerrain,
                                hudScreen.mGameScreen.mLevel.mSilverback,
                                hudScreen.mGameScreen.mLevel,
                                ref rand);
                            orang.Actor.ModelMatrix = matrix;
                            hudScreen.mGameScreen.mLevel.Add(orang);
                        }

                        LumberChoice.AddApe(2);
                    }

                }
                catch (Exception)
                {
                    // ignored
                }

                fs.Close();
            }


            serializer = new XmlSerializer(typeof(List<Hut>));

            using (var fs = File.OpenRead(PauseScreen.GetSavedGamesPath() + "\\Huts.xml"))
            {
                try
                {
                    Hut.mAmount = 0;
                    var huts = (List<Hut>)serializer.Deserialize(fs) ?? new List<Hut>();
                    if (hudScreen.mGameScreen.mLevel != null)
                    {
                        // hudScreen.mGameScreen.mLevel.mHuts = hudScreen.mGameScreen.mLevel.mHuts.Except(huts).ToList();
                        if (hudScreen.mGameScreen.mLevel != null)
                        {
                            // Remove dead huts from ...mLevel.mHuts

                            var tmpHuts = new List<Hut>();

                            // Yeah this is bad
                            foreach (var h in huts)
                            {
                                foreach (var hut in hudScreen.mGameScreen.mLevel.mHuts)
                                {
                                    if (h.mId == hut.mId)
                                    {
                                        tmpHuts.Add(hut);
                                    }
                                }
                            }

                            hudScreen.mGameScreen.mLevel.mHuts.Clear();
                            hudScreen.mGameScreen.mLevel.mHuts.AddRange(tmpHuts);

                            var screen = hudScreen;
                            var hutActorBatch = hudScreen.mGameScreen.mLevel.mActorBatches.Find(ele =>
                                ele.mMesh == screen.mGameScreen.mLevel.mHutMesh);

                            var tmpActors = new List<Actor>();

                            foreach (var actor in hutActorBatch.mActors)
                            {
                                foreach (var hut in hudScreen.mGameScreen.mLevel.mHuts)
                                {
                                    if (hut.Actor == actor)
                                    {
                                        tmpActors.Add(actor);
                                    }
                                }
                            }

                            hutActorBatch.mActors.Clear();
                            hutActorBatch.mActors.AddRange(tmpActors);


                            var i = 0;
                            foreach (var hut in hudScreen.mGameScreen.mLevel.mHuts)
                            {
                                // var contains = false;
                                // foreach (var h in huts)
                                // {
                                //     if (h.mId == hut.mId)
                                //     {
                                //         contains = true;
                                //     }
                                // }
                                // 
                                // if (!contains)
                                // {
                                //     hudScreen.mGameScreen.mLevel.mHuts.Remove(hut);
                                //     continue;
                                // }

                                hut.mSilverback = hudScreen.mGameScreen.mLevel.mSilverback;
                                hut.mAxeMesh = hudScreen.mGameScreen.mLevel.mAxeMesh;
                                hut.mDoubleAxeKillerMesh = hudScreen.mGameScreen.mLevel.mDoubleAxeKillerMesh;
                                hut.mScene = hudScreen.mGameScreen.mLevel;
                                hut.mLumberjackMesh = hudScreen.mGameScreen.mLevel.mLumberjackMesh;
                                hut.HP = huts[i++].HP;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

                fs.Close();
            }
            Hut.mAmount = 0;

            return "";

        }
    }
}