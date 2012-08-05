using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Ragnarok game = new Ragnarok())
            {
                game.Run();
            }
        }
    }
}
