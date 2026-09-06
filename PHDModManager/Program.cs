using System;
using System.Windows.Forms;

namespace PHDModManager
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());  // <-- CAMBIO (antes: new PHDModManager() o similar)
        }
    }
}