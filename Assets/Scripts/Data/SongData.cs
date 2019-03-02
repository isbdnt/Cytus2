using UnityEngine;

namespace Cytus2
{
    public class SongData
    {
        public int id;
        public string name;
        public int bpm;//beats per minute
        public int beatUnit;//beat unit of time signature
        public float timeOffset;
        public bool upsideDown;
        public ChartData[] charts;
        public AudioClip audioClip;
    }
}