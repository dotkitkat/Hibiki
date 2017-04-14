using System;

namespace Hibiki
{
    internal class Program
    {
        private static void Main()
        {
            new HibikiBot().RunAndBlockAsync().GetAwaiter().GetResult();
        }
    }
}