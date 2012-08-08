namespace Axiom.Framework.Configuration
{
    /// <summary>
    /// </summary>
    public interface IConfigurationDialog
    {
        /// <summary>
        ///   Gets the selected rendersystem (and all the values of options set for it)
        /// </summary>
        Axiom.Graphics.RenderSystem RenderSystem { get; }

        /// <summary>
        ///   Display the dialog of configuration options
        /// </summary>
        /// <returns> A <see cref="DialogResult" /> </returns>
        DialogResult Show();
    }
}