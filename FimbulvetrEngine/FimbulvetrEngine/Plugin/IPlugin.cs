namespace FimbulvetrEngine.Plugin
{
    public interface IPlugin
    {
        string Name { get; }
        bool Initialize();
        void Shutdown();
    }
}
