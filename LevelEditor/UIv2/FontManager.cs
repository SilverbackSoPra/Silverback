using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelEditor.Screen;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.UIv2
{
    public static class FontManager
    {
        public enum FontType
        {
            Heading,
            Subheading,
            Story,
            Default
        }

        private static SpriteFont mBigHeadingFont = ScreenManager.ContentManager.Load<SpriteFont>("Heading");
        private static SpriteFont mSmallHeadingFont = ScreenManager.ContentManager.Load<SpriteFont>("HeadingSmall");

        private static SpriteFont mBigSubeadingFont = ScreenManager.ContentManager.Load<SpriteFont>("SubHeaderFont");
        private static SpriteFont mSmallSubheadingFont = ScreenManager.ContentManager.Load<SpriteFont>("SubHeaderFontSmall");

        private static SpriteFont mBigStoryFont = ScreenManager.ContentManager.Load<SpriteFont>("StoryFont");
        private static SpriteFont mSmallStoryFont = ScreenManager.ContentManager.Load<SpriteFont>("StoryFontSmall");

        private static SpriteFont mBigFont = ScreenManager.ContentManager.Load<SpriteFont>("Font");
        private static SpriteFont mSmallFont = ScreenManager.ContentManager.Load<SpriteFont>("FontSmall");

        public static SpriteFont HeadingFont(int width, int height)
        {
            if (width < 1600 || height < 900)
            {
                return mSmallHeadingFont;
            }
            return mBigHeadingFont;
        }

        public static SpriteFont SubheadingFont(int width, int height)
        {
            if (width < 1600 || height < 900)
            {
                return mSmallSubheadingFont;
            }
            return mBigSubeadingFont;
        }

        public static SpriteFont StoryFont(int width, int height)
        {
            if (width < 1600 || height < 900)
            {
                return mSmallStoryFont;
            }
            return mBigStoryFont;
        }

        public static SpriteFont Font(int width, int height)
        {
            if (width < 1600 || height < 900)
            {
                return mSmallFont;
            }
            return mBigFont;
        }

        public static SpriteFont Get(FontType ft, int width, int height)
        {
            switch (ft)
            {
                case FontType.Heading:
                    return HeadingFont(width, height);
                case FontType.Subheading:
                    return SubheadingFont(width, height);
                case FontType.Story:
                    return StoryFont(width, height);
                default:
                    return Font(width, height);
            }
        }
    }
}
