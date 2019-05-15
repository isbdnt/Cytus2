using UnityEngine;
using System.Collections;

namespace Cytus2
{
    public static class Main
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void TestOneWayLove()
        {
            Time.fixedDeltaTime = 1f / 60f;
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60;
            PlayerLoop.NewInstance();
            ResourceManager.NewInstance();
            GridView.NewInstance();

            PlayerLoop.instance.StartCoroutine(InitializeGridView());
        }

        static IEnumerator InitializeGridView()
        {
            yield return new WaitForFixedUpdate();

            GridView.instance.Initialize(ResourceManager.instance.songDataMap[1], 0);
        }
    }
}