using System;
namespace TestMarkdown
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
                Function.Menu(args[0]);
        }
    }
}
