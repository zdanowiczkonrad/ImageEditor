using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EdytorObrazow
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(@"sample.jpeg",2));
            //
            //
            // INFO:
            //
            // - pierwszy arg konstruktora to sciezka bezwzgledna do pliku
            // - drugi parametr
            // --- 1: tylko podglad zdjecia
            // --- 2: edycja zdjecia
            //
            // 

        }
    }
}
