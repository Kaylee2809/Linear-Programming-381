using Linear_Programming_381.Forms;
using System;
using System.Windows.Forms;

namespace Linear_Programming_381
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            Application.Run(new MainForm());
        }
    }
}