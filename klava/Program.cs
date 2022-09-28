using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace klava
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            bool argument = false;
            Mutex mutex = new Mutex(false, "klaVa-tHemAnwhoSold-COME-ThewOrld42");
            try
            {
                if (mutex.WaitOne(0, false))
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    foreach (string arg in args)
                    {
                        if (arg == "-admin")
                        {
                            argument = true;
                            Application.Run(new Form1()); //34423
                            break;
                        }
                    }
                    if (argument == false)
                    {
                        Application.Run(new Form2());

                    }
                }
                else
                {
                    MessageBox.Show("Это приложение уже запущено", "klava");
                }
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.Close();
                    mutex = null;
                }
            }
            
        }
    }
}
