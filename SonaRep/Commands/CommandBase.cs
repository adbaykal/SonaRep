using McMaster.Extensions.CommandLineUtils;

namespace SonaRep.Commands;

public abstract class CommandBase
{
    protected IConsole? _console;
    protected abstract Task<int> OnExecute(CommandLineApplication app);

}