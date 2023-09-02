using System.Collections;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;


[assembly: MelonInfo(typeof(Melon_HoneyCome.Uncensored), "HoneyCome.Uncensored", "1.0.0", "Shiina")]
[assembly: MelonGame("", "")]
namespace Melon_HoneyCome
{
    public class Uncensored : MelonMod
    {
        private bool HSceneLoaded = false;
        private bool replaced = false;
        private List<GameObject> allChar = new();
        
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            LoggerInstance.Msg($"Scene {sceneName} Loaded at {buildIndex}");
            
            if (sceneName == "HScene")
            {
                LoggerInstance.Msg("H Scene Loaded");
                HSceneLoaded = true;
            }
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            LoggerInstance.Msg($"Scene {sceneName} Unloaded at {buildIndex}");
            
            if (sceneName == "HScene")
            {
                replaced = false;
                allChar.Clear();
                HSceneLoaded = false;
                // MelonCoroutines.Stop("ReplaceMaterial"); // TODO: error: routine is null
            }   
        }
        
        public override void OnUpdate()
        {
            allChar.Clear();
            FindAllChar();
            
            if (HSceneLoaded && !replaced && allChar.Count >= 2)
            {
                replaced = true;
                MelonCoroutines.Start(ReplaceCorotine());
            }
        }

        IEnumerator ReplaceCorotine()
        {
            yield return new WaitForSeconds(5f);
            ClearCloth();
            Replace();
        }

        void FindAllChar()
        {
            var Common = GameObject.Find("Common");
            if (Common == null) return ;
            var Stashing = Common.transform.Find("________Stashing");
            
            // In special case, entering HScenes from the lobby will result in characters being generated under "Common"
            // instead of "Stashing"
            for (int index = 0; index < Common.transform.childCount; index++)
            {
                var obj = Common.transform.GetChild(index).gameObject;
                if(obj.name.Contains("cha"))
                {
                    allChar.Add(obj);
                }
                
            }
            for (int index = 0; index < Stashing.childCount; index++)
            {
                var obj = Stashing.transform.GetChild(index).gameObject;
                if(obj.name.Contains("cha") && obj.name!="chaF_00")
                {
                    allChar.Add(obj);
                }
            }
        }
        void ReplaceFemale(GameObject parent, Material material)
        {
            GameObject mnpa = SearchForTargetByName(parent, "mnpa");
            GameObject mnpb = SearchForTargetByName(parent, "mnpb");
            
            SetMaterial(mnpa, material);
            SetMaterial(mnpb, material);
            LoggerInstance.Msg("Replace FeMale");
        }

        void ReplaceMale(GameObject parent, Material material)
        {
            GameObject o_dan_f = SearchForTargetByName(parent, "o_dan_f");
            GameObject o_dankon = SearchForTargetByName(parent, "o_dankon");
            GameObject o_gomu = SearchForTargetByName(parent, "o_gomu");
            
            SetMaterial(o_dan_f, material);
            SetMaterial(o_dankon, material);
            SetMaterial(o_gomu, material);
            LoggerInstance.Msg("Replace Male");
        }

        void SetMaterial(GameObject obj, Material mat)
        {
            var skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer.material.name.Contains("mnpb")
                && skinnedMeshRenderer.material.name != mat.name)
            {
                LoggerInstance.Msg($"obj.name: {obj.name}, smrMaterial: {skinnedMeshRenderer.material.name} -> {mat.name}");
                skinnedMeshRenderer.material = mat;
            }
            
        }

        void ReplaceMaterial(GameObject obj)
        {
            GameObject body = SearchForTargetByName(obj, "o_body");
            Material bodyMaterial =new(body.GetComponent<SkinnedMeshRenderer>().material);
            if (obj.name.Contains("chaM"))
            {
                ReplaceMale(obj, bodyMaterial);
            }
            else if (obj.name.Contains("chaF"))
            {
                ReplaceFemale(obj, bodyMaterial);
            }
            
            LoggerInstance.Msg($"{obj.name} replace material success!");
        }

        void Replace()
        {
            foreach (var obj in allChar)
            {
                ReplaceMaterial(obj.gameObject);
            }
        }

        void ClearCloth()
        {
            GameObject UI = GameObject.Find("UI");
            GameObject ClothAllBt = SearchForTargetByName(UI, "ClothAllBt");
            GameObject button = ClothAllBt.transform.GetChild(0).gameObject;
            GameObject button1 = ClothAllBt.transform.GetChild(1).gameObject;
            if (button.active)
            {
                Button buttonComponent = ClothAllBt.transform.GetChild(0).GetComponent<Button>();
                buttonComponent.onClick.Invoke();
            }
            if (button1.active)
            {
                Button button1Component = ClothAllBt.transform.GetChild(1).GetComponent<Button>();
                button1Component.onClick.Invoke();
            }
            
        }
        
        GameObject SearchForTargetByName(GameObject parent, string targetName)
        {
            if (parent.name == targetName)
            {
                return parent;
            }

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject child = parent.transform.GetChild(i).gameObject;

                GameObject found = SearchForTargetByName(child, targetName);

                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }
}