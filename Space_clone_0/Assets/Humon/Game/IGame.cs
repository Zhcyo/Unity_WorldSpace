using System;

    public interface IGame : IDependency
    {
        void EnterCheckpoint(int number, int subObjective);

        void EnterPassZone();

        void Fall(HumanBase human, bool drown = false, bool fallAchievement = true);

      
    }
