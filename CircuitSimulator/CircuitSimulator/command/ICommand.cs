using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator.command
{
    public interface ICommand
    {
        void Execute();
        string GetCommandType();
    }
}
