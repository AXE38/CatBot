using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Args;

namespace CatBot.Commands
{
    interface ICommand
    {
        public String Command { get; }

        public void Execute(object sender, MessageEventArgs e) => throw new NotImplementedException();
    }
}
