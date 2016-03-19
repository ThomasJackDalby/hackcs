using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core.Tools
{
    public static class ConsoleMenu
    {
        public static void ShowMenu(List<MenuOption> options)
        {
            MenuOption help = new MenuOption("?", (o) =>
            {
                int max = options.Max(x => x.Command.Length);
                foreach (MenuOption option in options) Console.WriteLine(String.Format("{0,-"+max+"} : {1}", option.Command, option.Description));
            }) { Description = "Display all available options" };
            options.Add(help);

            while(true)
            {
                string rawInput = Console.ReadLine();
                string[] userInput = rawInput.Split(' ');

                string command = userInput[0];
                string[] args = userInput.Skip(1).ToArray();

                foreach (MenuOption option in options)
                {
                    if (option.Command == command)
                    {
                        try
                        {
                            option.Method(args);
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("#Fail");
                            Console.WriteLine(e.Message);
                        }
                        return;
                    }
                }
                Console.WriteLine("Unknown command");
            }
        }
    }
}
