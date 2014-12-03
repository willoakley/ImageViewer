using System;
using System.Windows.Forms;

namespace ImageViewer
{
    static class Program
    {
        [STAThread]
        static void Main(string[] arguments)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = new MainForm(arguments);
            if (!mainForm.ImageLoaded)
            {
                return;
            }

            Application.Run(mainForm);
        }
    }
}
