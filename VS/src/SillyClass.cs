using Il2Cpp;
using UnityEngine;
using HarmonyLib;
using MelonLoader;
using AudioMgr;
using System.Collections;
using UnityEngine.Analytics;


[assembly: MelonOptionalDependencies("AudioManager")]


namespace SillyMod
{
    public class SillyClass : MelonMod
    {
        public static AssetBundle sillyBundle;

        public static Dictionary<int, Vector3> rememberScale = new Dictionary<int, Vector3>();


        public static bool audioEnabled = false;

        public static string modsPath;

        public static bool coroutineRunning;

        public static GameObject? lastWoo = null;

        public static bool allowMod;

        public static Transform? worldView = null;

        public static GameObject? playerCollider = null;

        public static LineRenderer line;



        public override void OnInitializeMelon()
        {
            Utility.LoadEmbeddedAssetBundle();
            //sillyBundle = AssetBundle.LoadFromFile(Application.dataPath + "/../Mods/sillysounds.unity3d");

            modsPath = Path.GetFullPath(typeof(MelonMod).Assembly.Location + "/../../../Mods/");

            if (File.Exists(modsPath + "AudioManager.dll")) audioEnabled = true;

            Settings.OnLoad();
        }

        public override void OnSceneWasInitialized(int level, string name)
        {

            if (Utility.IsBootScreen(name))
            {
                if (audioEnabled) Utility.PrepareAudio();

                //if (audioEnabled) PatchMaster.AddReplacePatch("PLAY_CROWCAWSDISTANT", Utility.clipManagerBundle, "woo", AudioMaster.SourceType.SFX);
            }
            

            

            if (Utility.IsScenePlayable())
            {
                worldView = GameManager.GetTopLevelCharacterFpsPlayer().transform.Find("WorldView");
                playerCollider = worldView.Find("TriggerCollideCapsule").gameObject;

                if (!worldView.GetComponent<SillyCollisionDetection>()) worldView.gameObject.AddComponent<SillyCollisionDetection>();
                if (!playerCollider.GetComponent<SillyCollisionDetection>()) playerCollider.gameObject.AddComponent<SillyCollisionDetection>();




                // DEBUG

                UnityEngine.Object.Destroy(line);
                line = (new GameObject("testLine")).AddComponent<LineRenderer>();
                line.material = new Material(Shader.Find("Sprites/Default"));
                line.widthMultiplier = 0.01f;
                Gradient gradient = new Gradient();
                gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.blue, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 1f) }
                );
                line.colorGradient = gradient;

                //DEBUG






            }

            allowMod = Utility.IsScenePlayable();
        }








        [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.ExitMeshPlacement))]
        private static class DisablePhysicsCheck
        {
            internal static void Prefix(PlayerManager __instance)
            {
                if (Settings.options.enablePhysics)
                {
                    if (__instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<Rigidbody>())
                    {
                        __instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                        __instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
                        __instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                        __instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<Rigidbody>().solverIterations = 30;
                    }
                    else
                    {
                        if (__instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<MeshCollider>())
                        {
                            __instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<MeshCollider>().convex = true;
                        }
                        __instance.m_ObjectToPlaceGearItem.gameObject.AddComponent<Rigidbody>();
                        __instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
                        __instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                        __instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<Rigidbody>().solverIterations = 30;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(GameManager), nameof(GameManager.Start))]
        private static class GMStart
        {
            internal static void Prefix(PlayerManager __instance)
            {
                Physics.IgnoreLayerCollision(14, 17, !Settings.options.enableCollisions);
                Physics.IgnoreLayerCollision(17, 17, !Settings.options.enableCollisions);

                
            }
        }



        public static IEnumerator GradualScale(Transform t, Vector3 targetScale)
        {
            coroutineRunning = true;
            float f = 0f;
            Vector3 initialScale = t.localScale;

            while (t.localScale != targetScale || f < 1f)
            {
                f += Time.deltaTime * 5;

                t.localScale = Vector3.Lerp(initialScale, targetScale, f);
                yield return new WaitForEndOfFrame();
            }

            coroutineRunning = false;
            yield return null;
        }




        public override void OnUpdate()
        {
            /*
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.L))
            {
                foreach (GameObject animal in GameObject.FindGameObjectsWithTag("BaseAi"))
                {
                    if (animal.name.Contains("WILDLIFE_Wolf"))
                    {
                        GameObject mesh = animal.transform.Find("Wolf_Rig").gameObject;
                        if (mesh != null)
                        {
                            mesh.transform.eulerAngles = new Vector3(285, 0f, 0f);
                            mesh.transform.localPosition= new Vector3(0f, -0.6f, 0f);

                        }


                    }
                }
            }
            */

            bool flag = allowMod && !uConsole.m_On && !InterfaceManager.IsOverlayActiveCached() && !coroutineRunning;

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.scaleKey) && flag)
            {
                GameObject go = Utility.GetGameObjectUnderCrosshair();

                if (!go)
                {
                    // restore last selected object, in case player is stuck inside
                    if (lastWoo && rememberScale.ContainsKey(lastWoo.GetInstanceID()))
                    {
                        MelonCoroutines.Start(GradualScale(lastWoo.transform, rememberScale[lastWoo.GetInstanceID()]));
                        rememberScale.Remove(lastWoo.GetInstanceID());
                        return;
                    }
                    return;
                }

                if (rememberScale.ContainsKey(go.GetInstanceID()))
                {
                    MelonCoroutines.Start(GradualScale(go.transform, rememberScale[go.GetInstanceID()]));
                    rememberScale.Remove(go.GetInstanceID());
                    return;
                }
                else
                {
                    System.Random r = new System.Random();

                    Vector3 currentScale = go.transform.localScale;
                    float x = go.transform.localScale.x * Settings.options.scaleMult * r.Next(1, Settings.options.scaleRandom);
                    float y = go.transform.localScale.y * Settings.options.scaleMult * r.Next(1, Settings.options.scaleRandom); 
                    float z = go.transform.localScale.z * Settings.options.scaleMult * r.Next(1, Settings.options.scaleRandom);

                    MelonCoroutines.Start(GradualScale(go.transform, new Vector3(x, y, z)));

                    rememberScale.Add(go.GetInstanceID(), currentScale);
                    lastWoo = go;

                    if (audioEnabled) Utility.PlayAudioClip("woo");

                }

            }
        }


    }
}
