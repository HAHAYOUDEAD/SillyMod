using Il2Cpp;
using ModSettings;
using UnityEngine;

namespace SillyMod
{
    internal static class Settings
    {
        public static void OnLoad()
        {
            options = new SillySettings();
            options.AddToModSettings("Silly Settings");
        }

        public static SillySettings options;
    }

    internal class SillySettings : JsonModSettings
    {
        [Name("Enable physics")]
        [Description("Enables physics after moving a gear item\n\nItems WILL fly out of the map, use at your own risk")]
        public bool enablePhysics = false;

        [Name("Enable collisions")]
        [Description("Enable collision of gear items with player")]
        public bool enableCollisions = false;

        [Name("Danger mode")]
        [Description("If something falls on your head - you'll get hurt (it's very much not precice, you can get damaged by pushing something yourself)")]
        public bool dangerMode = false;

        [Name("Player scale")]
        [Description("Change your player height to extreme values")]
        [Choice(new string[]
        {
            "Fall into woodcracks",
            "5%",
            "10%",
            "25%",
            "50%",
            "75%",
            "Default height",
            "200%",
            "500%",
            "10000%",
            "Touch the moon"
        })]
        public int playerScale = 6;


        [Name("Silly button")]
        [Description("Does something silly to object under crosshair")]
        public KeyCode scaleKey = KeyCode.O;

        [Name("Silly multiplyer")]
        [Description("Multiply Silliness")]
        [Slider(1f, 20f)]
        public int scaleMult = 5;

        [Name("Silly wackiness")]
        [Description("Yeah, that's an option")]
        [Slider(1f, 10f)]
        public int scaleRandom = 1;

        public static bool settingsChanged;

        protected override void OnConfirm()
        {
            

            Physics.IgnoreLayerCollision(14, 17, !Settings.options.enableCollisions);
            Physics.IgnoreLayerCollision(17, 17, !Settings.options.enableCollisions);



            float scale = 1f;

            switch (Settings.options.playerScale)
            {
                case 0:
                    scale = 0.01f;
                    break;
                case 1:
                    scale = 0.05f;
                    break;
                case 2:
                    scale = 0.1f;
                    break;
                case 3:
                    scale = 0.25f;
                    break;
                case 4:
                    scale = 0.5f;
                    break;
                case 5:
                    scale = 0.75f;
                    break;
                case 6:
                    scale = 1f;
                    break;
                case 7:
                    scale = 2f;
                    break;
                case 8:
                    scale = 5f;
                    break;
                case 9:
                    scale = 10f;
                    break;
                case 10:
                    scale = 100f;
                    break;
            }

            SillyClass.worldView.localScale = Vector3.one * scale;

            base.OnConfirm();
        }
    }
}
