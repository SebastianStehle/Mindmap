
namespace RavenMind.Model
{
    public interface IDocumentCommands
    {
        void Apply(CommandBase command);
    }
}
