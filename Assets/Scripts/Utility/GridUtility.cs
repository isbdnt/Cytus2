namespace Cytus2
{
    public static class GridUtility
    {
        public static float StepToPositionY(int step)
        {
            float paceInRound = step % 32 / 2f;
            return paceInRound < 8 ? paceInRound : (16 - paceInRound);
        }

        public static int StepToDirection(int step)
        {
            float paceInRound = step % 32 / 2f;
            return paceInRound < 8 ? 1 : -1;
        }
    }
}