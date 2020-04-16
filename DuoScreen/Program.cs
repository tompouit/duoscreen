using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DuoScreen
{
    static class Program
    {
        public const string name = "DuoScreen";
        public static DuoScreenContext appContext;

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Mutex mutex = new Mutex(true, name, out bool onlyInstance);
            if (!onlyInstance)
                return;
//            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                appContext = new DuoScreenContext();
                Application.Run(appContext);
            }
            catch (Exception e) { MessageBox.Show(e.Message, "Program Terminated Unexpectedly", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            GC.KeepAlive(mutex);
        }
    }
}
