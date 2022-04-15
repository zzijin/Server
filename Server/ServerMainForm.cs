using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Server.StatisticsModule;

namespace Server
{
    public partial class ServerMainForm : Form
    {
        ServerManager serverManager;
        int outputMode = 0;

        public ServerMainForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            serverManager = new ServerManager(serviceProvider);
        }

        private void Button_Start_Click(object sender, EventArgs e)
        {
            serverManager.StartServer();
            Button_StartServer.Enabled = false;

            Task.Run(async () =>
            {
                while (true)
                {
                    switch (outputMode)
                    {
                        case 0: serverManager.OutputConnPoolInfoToConsole(); break;
                        case 1: serverManager.OutputUsedConnInfoToConsole(); break;

                        default: serverManager.OutputConnPoolInfoToConsole(); break;
                    }
                    await Task.Delay(2000);
                }

            });
        }

        private void check_onlyActivity_CheckedChanged(object sender, EventArgs e)
        {
            if(check_onlyActivity.Checked)
            {
                outputMode = 1;
            }
            else
            {
                outputMode = 0;
            }
        }
    }
}
