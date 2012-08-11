using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient
{
    internal class Program
    {
        private static void Main()
        {
            using (Ragnarok ro = new Ragnarok())
            {
                ro.Run();
            }
        }
    }
}