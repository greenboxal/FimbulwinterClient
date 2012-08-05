using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Core;
using Axiom.Graphics;
using FimbulwinterClient.Core.Content;
using Axiom.Framework;

namespace FimbulwinterClient.Core
{
    public class SharedInformation
    {
        static SharedInformation()
        {

        }

        public static Game Game { get; set; }
    }
}
