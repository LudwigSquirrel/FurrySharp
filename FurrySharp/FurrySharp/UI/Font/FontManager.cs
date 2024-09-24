using FurrySharp.Dialogue;
using FurrySharp.Registry;
using Microsoft.Xna.Framework;

namespace FurrySharp.UI.Font
{
    public static class FontManager
    {
        public static string LanguageString
        {
            get
            {
                switch (GlobalState.CurrentLanguage)
                {
                    default:
                        return ENString;
                }
            }
        }

        //Language string stuff
        private const string ENString = "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ’1234567890.:,;\'\"(!?)+-*/=$[]_<>%^";

        public static SpriteFont InitFont(bool forceEnglish = false)
        {
            string lString;
            string fileName;

            int width;
            int height = forceEnglish ? 8 : GameConstants.FontLineHeight;

            Language lang = forceEnglish ? Language.EN : GlobalState.CurrentLanguage;

            switch (lang)
            {
                default:
                    lString = ENString;
                    fileName = "font-white-apple-7x8";
                    width = 7;
                    break;
            }

            return new SpriteFont(height, width, fileName, lString);
        }
        
        public static SpriteFont InitMockFont(Language lang = Language.EN)
        {
            return new SpriteFont(8, 8, "font-white-apple-7x8", ENString);
        }

        public static int GetCharacterWidth(bool forceEnglish = false)
        {

            Language lang = forceEnglish ? Language.EN : GlobalState.CurrentLanguage;

            return lang switch
            {
                _ => 7,
            };
        }
    }
}
