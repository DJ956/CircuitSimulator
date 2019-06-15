using System;

namespace CircuitSimulator.command
{
    public class ExitCommand : ICommand
    {
        public ExitCommand() { }

        public void Execute()
        {
            Environment.Exit(1);
        }

        public string GetCommandType()
        {
            return "ex";
        }
    }
}
