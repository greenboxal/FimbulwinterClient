using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Graphics;
using FimbulvetrEngine.IO;
using FimbulvetrEngine.Plugin;

namespace FimbulvetrEngine
{
    public class Vetr
    {
        public static Vetr Instance { get; private set; }

        public XDocument Configuration { get; private set; }
        public XElement ConfiguartionRoot { get; private set; }

        public Vetr()
        {
            if (Instance != null)
                throw new Exception("Only one instance of Vetr is allowed, use the Instance property.");

            InstanceSingletonClasses();

            Instance = this;
        }

        private void InstanceSingletonClasses()
        {
            new PluginManager();
            new FileSystemManager();
            new ContentManager();
            new TextureManager();
        }

        public bool ReadConfiguration(string filename = "vetr.xml", string section = "Vetr")
        {
            Configuration = XDocument.Load(filename);

            ConfiguartionRoot = Configuration.Element(section);
            if (ConfiguartionRoot != null)
            {
                ReadPluginsConfig(ConfiguartionRoot);
                ReadFileSystemConfig(ConfiguartionRoot);
            }

            return true;
        }

        private void ReadFileSystemConfig(XElement vetr)
        {
            XElement fs = vetr.Element("FileSystem");

            if (fs != null)
            {
                XElement sources = fs.Element("Sources");

                if (sources != null)
                {
                    var sourcelist = from node in sources.Elements("Source") select new { Path = (string)node.Attribute("Path") ?? "", Type = (string)node.Attribute("Type") ?? "Folder", MD5Check = (string)node.Attribute("MD5Check") ?? "" };

                    foreach (var source in sourcelist)
                    {
                        FileSystemManager.Instance.RegisterFileSystem(source.Type, source.Path, source.MD5Check);
                    }
                }
            }
        }

        private void ReadPluginsConfig(XElement vetr)
        {
            XElement sources = vetr.Element("Plugins");

            if (sources != null)
            {
                var sourcelist = from node in sources.Elements("Plugin") select new { Path = (string)node.Attribute("Path") ?? "", Type = (string)node.Attribute("Type") ?? "Folder", MD5Check = (string)node.Attribute("MD5Check") ?? "" };

                foreach (var source in sourcelist)
                {
                    // TODO: Implement MD5Check
                    // TODO: Log invalid plugin?
                    PluginManager.Instance.RegisterPlugin(source.Path);
                }
            }
        }

        public void Run()
        {
            
        }
    }
}
