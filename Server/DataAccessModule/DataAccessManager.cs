using Server.DataAccessModule.Interface;

namespace Server.DataAccessModule
{
    class DataAccessManager:IDataAccessManager
    {
        public DataAccessManager()
        {
            InitSQLConfiguration();


        }

        private void InitSQLConfiguration()
        {
            LoadSQLConfiguration();
        }

        private void LoadSQLConfiguration()
        {

        }
    }
}
