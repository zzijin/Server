using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Server
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;

            Console.BufferWidth = 200;
            IOCManager iocManager = new IOCManager();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ServerMainForm(iocManager.BuildServiceProvider()));

        }
    }
}
