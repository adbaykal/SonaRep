using McMaster.Extensions.CommandLineUtils;

namespace SonaRep.Commands;

public class CommandBase
{
    protected IConsole _console;
    protected virtual Task<int> OnExecute(CommandLineApplication app)
    {
        throw new NotImplementedException();
    }
}