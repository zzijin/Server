using Server.StatisticsModule.Interface;

namespace Server.StatisticsModule
{
    class StatisticsManager: IStatisticsManager
    {
        public StatisticsManager()
        {
            InitStatisticsConfiguration();


        }

        private void InitStatisticsConfiguration()
        {
            LoadStatisticsConfiguration();
        }

        private void LoadStatisticsConfiguration()
        {

        }
    }
}
