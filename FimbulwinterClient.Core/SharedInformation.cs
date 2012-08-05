using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Core;
using Axiom.Graphics;
using FimbulwinterClient.Core.Content;

namespace FimbulwinterClient.Core
{
    public class SharedInformation
    {
        static SharedInformation()
        {

        }

        public static Root Engine { get; set; }
        public static RenderWindow Window { get; set; }
    }
}
