using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CatBot.Commands
{
    class CommandHandler
    {
        static private Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

        private CommandHandler() { }

        public static Boolean AddCommand(ICommand command)
        {
            return commands.TryAdd(command.Command, command);
        }

        public static Boolean TryGetCommand(String commandStr, out ICommand command)
        {
            return commands.TryGetValue(commandStr, out command);
        }
    }
}
