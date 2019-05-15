using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Cytus2
{
    public class ResourceManager
    {
        public static ResourceManager instance { get; private set; }

        public Dictionary<int, SongData> songDataMap { get; private set; } = new Dictionary<int, SongData>();

        public static ResourceManager NewInstance()
        {
            if (instance != null)
            {
                instance.songDataMap.Clear();
                GridView.prefab = null;
                NoteView.pool.Clear();
                ClickPieceView.pool.Clear();
                HoldPieceView.pool.Clear();
                SpecialHoldPieceView.pool.Clear();
                DragPieceView.pool.Clear();
                FlickPieceView.pool.Clear();
                BeatingResultView.pool.Clear();
                Resources.UnloadUnusedAssets();
            }
            instance = new ResourceManager();
            return instance;
        }

        private ResourceManager()
        {
            LoadSong("OneWayLove");
            LoadPrefabs();
        }

        private void LoadSong(string name)
        {
            var song = JsonConvert.DeserializeObject<SongData>(Resources.Load<TextAsset>($"Songs/{name}/Config").text);
            song.audioClip = Resources.Load<AudioClip>($"Songs/{name}/Soundtrack");
            songDataMap[song.id] = song;
        }

        private void LoadPrefabs()
        {
            GridView.prefab = Resources.Load<GameObject>("Prefabs/Grid");
            NoteView.pool.AddEntityPrefab(Resources.Load<GameObject>("Prefabs/Note"));
            ClickPieceView.pool.AddEntityPrefab(Resources.Load<GameObject>("Prefabs/ClickPiece"));
            HoldPieceView.pool.AddEntityPrefab(Resources.Load<GameObject>("Prefabs/HoldPiece"));
            SpecialHoldPieceView.pool.AddEntityPrefab(Resources.Load<GameObject>("Prefabs/SpecialHoldPiece"));
            DragPieceView.pool.AddEntityPrefab(Resources.Load<GameObject>("Prefabs/DragPiece"));
            FlickPieceView.pool.AddEntityPrefab(Resources.Load<GameObject>("Prefabs/FlickPiece"));
            BeatingResultView.pool.AddEntityPrefab("Good", Resources.Load<GameObject>("Prefabs/GoodBeating"));
            BeatingResultView.pool.AddEntityPrefab("Perfect", Resources.Load<GameObject>("Prefabs/PerfectBeating"));
            BeatingResultView.pool.AddEntityPrefab("Miss", Resources.Load<GameObject>("Prefabs/MissBeating"));
            BeatingResultView.pool.AddEntityPrefab("Bad", Resources.Load<GameObject>("Prefabs/BadBeating"));
        }
    }
}