using AudioMgr;
using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MelonLoader;

namespace SillyMod
{
    public class Utility
    {

        public static ClipManager clipManagerBundle;
        public static Shot soundSource;

        public static bool IsScenePlayable()
        {
            return !(string.IsNullOrEmpty(GameManager.m_ActiveScene) || GameManager.m_ActiveScene.Contains("MainMenu") || GameManager.m_ActiveScene == "Boot" || GameManager.m_ActiveScene == "Empty");
        }

        public static bool IsScenePlayable(string scene)
        {
            return !(string.IsNullOrEmpty(scene) || scene.Contains("MainMenu") || scene == "Boot" || scene == "Empty");
        }
        public static bool IsMainMenu(string scene)
        {
            return !string.IsNullOrEmpty(scene) && scene.Contains("MainMenu");
        }

        public static bool IsBootScreen(string scene)
        {
            return !string.IsNullOrEmpty(scene) && scene.Contains("Boot");
        }

        public static GameObject GetGameObjectUnderCrosshair()
        {
            GameObject go = null;
            PlayerManager pm = GameManager.GetPlayerManagerComponent();

            float maxPickupRange = GameManager.GetGlobalParameters().m_MaxPickupRange;
            float maxRange = pm.ComputeModifiedPickupRange(maxPickupRange);
            if (pm.GetControlMode() == PlayerControlMode.InFPCinematic)
            {
                maxRange = 50f;
            }

            go = GameManager.GetPlayerManagerComponent().GetInteractiveObjectUnderCrosshairs(maxRange);

            return go;

        }

        public static float AverageVector(Vector3 v)
        {
            return (v.x + v.y + v.z)/3f;
        }



        public static void PrepareAudio()
        {
            clipManagerBundle = new ClipManager();

            clipManagerBundle.LoadAllClipsFromBundle(SillyClass.sillyBundle);

            soundSource = AudioMaster.CreatePlayerShot(AudioMaster.SourceType.SFX);

        }

        public static void PlayAudioClip(string clipName) => soundSource.PlayOneshot(clipManagerBundle.GetClip(clipName));

        public static void LoadEmbeddedAssetBundle()
        {
            
            MemoryStream memoryStream;
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SillyMod.Resources.sillysounds");
            memoryStream = new MemoryStream((int)stream.Length);
            stream.CopyTo(memoryStream);

            SillyClass.sillyBundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());
            

            //SillyClass.sillyBundle = AssetBundle.LoadFromFile(Application.dataPath + "/../Mods/sillysounds.unity3d");

            
        }


    }
}
