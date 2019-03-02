using UnityEngine;

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

            ResourceManager.NewInstance();
            GridView.NewInstance().Initialize(ResourceManager.instance.songDataMap[1], 0);
        }
    }
}