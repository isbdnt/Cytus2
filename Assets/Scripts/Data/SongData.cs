namespace Cytus2
{
    public class SongData
    {
        public string name;
        public int bpm;//beats per minute
        public int noteValue;//note value of the beat
        public float timeOffset;
        public bool upsideDown;
        public ChartData[] charts;
    }
}