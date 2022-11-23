using System;

namespace ImAgent.Helpers
{
    public static class Helper
    {
        public static void PrintConsoleMessage(MessageType type, params string[] text)
        {
            Console.ResetColor();

            switch(type)
            {
                case MessageType.Error: Console.ForegroundColor = ConsoleColor.Red; break;
                case MessageType.Info: Console.ForegroundColor = ConsoleColor.Blue; break;
                case MessageType.Success: Console.ForegroundColor = ConsoleColor.Green; break;
                case MessageType.Warning: Console.ForegroundColor = ConsoleColor.Yellow; break;
            }

            foreach (var item in text)
            {
                Console.WriteLine(item); 
            }

            Console.ResetColor();
        }
    }
}
