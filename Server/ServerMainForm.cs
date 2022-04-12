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

namespace Server
{
    public partial class ServerMainForm : Form
    {
        ServerManager serverManager;
        public ServerMainForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            serverManager = new ServerManager(serviceProvider);
        }

        private void Button_Start_Click(object sender, EventArgs e)
        {
            serverManager.StartServer();
            Button_StartServer.Enabled = false;
        }
    }
}
