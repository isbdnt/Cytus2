using System;

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

        public static BeatingResultView SpawnBeatingResultView(BeatingResultType beatingResult)
        {
            switch (beatingResult)
            {
                case BeatingResultType.Good:
                    return BeatingResultView.pool.SpawnEntity("Good", GridView.instance.beatingResultContainer, false);

                case BeatingResultType.Perfect:
                    return BeatingResultView.pool.SpawnEntity("Perfect", GridView.instance.beatingResultContainer, false);

                case BeatingResultType.Miss:
                    return BeatingResultView.pool.SpawnEntity("Miss", GridView.instance.beatingResultContainer, false);

                case BeatingResultType.Bad:
                    return BeatingResultView.pool.SpawnEntity("Bad", GridView.instance.beatingResultContainer, false);

                default:
                    throw new Exception();
            }
        }
    }
}