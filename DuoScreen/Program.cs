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

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool onlyInstance = false;
            Mutex mutex = new Mutex(true, name, out onlyInstance);
            if (!onlyInstance)
                return;
//            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new DuoScreenContext());
            }
            catch (Exception e) { MessageBox.Show(e.Message, "Program Terminated Unexpectedly", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            GC.KeepAlive(mutex);
        }
    }
}
