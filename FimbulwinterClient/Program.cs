using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace FimbulwinterClient
{
    internal class Program
    {
        private static void Main()
        {
            using (Ragnarok ro = new Ragnarok())
            {
                ro.VSync = VSyncMode.Off;
                ro.Run(0, 0);
            }
        }
    }
}