using AICUP.Mark2;
using Aicup2020.Model;
using System.Collections.Generic;

namespace Aicup2020
{
    public class MyStrategy
    {
        HeadquartersModule headModule = null;

        public MyStrategy()
        {
            System.Console.Clear();

            Out.Print("███╗░░░███╗░█████╗░██████╗░██╗░░██╗  ██████╗░", System.ConsoleColor.DarkGreen);
            Out.Print("████╗░████║██╔══██╗██╔══██╗██║░██╔╝  ╚════██╗", System.ConsoleColor.DarkGreen);
            Out.Print("██╔████╔██║███████║██████╔╝█████═╝░  ░░███╔═╝", System.ConsoleColor.DarkGreen);
            Out.Print("██║╚██╔╝██║██╔══██║██╔══██╗██╔═██╗░  ██╔══╝░░", System.ConsoleColor.DarkGreen);
            Out.Print("██║░╚═╝░██║██║░░██║██║░░██║██║░╚██╗  ███████╗", System.ConsoleColor.DarkGreen);
            Out.Print("╚═╝░░░░░╚═╝╚═╝░░╚═╝╚═╝░░╚═╝╚═╝░░╚═╝  ╚══════╝", System.ConsoleColor.DarkGreen);
        }

        public Action GetAction(PlayerView playerView, DebugInterface debugInterface)
        {
            if (headModule == null) headModule = new HeadquartersModule(playerView);

            Action action = headModule.Execute(null, playerView, 0);

            return action;
        }

        public void DebugUpdate(PlayerView playerView, DebugInterface debugInterface)
        {
            debugInterface.Send(new DebugCommand.Clear());
            debugInterface.GetState();
        }
    }
}