using System;
using Serato.Net.Services;

namespace Serato.Net.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new SessionFileParser();
            Console.ReadKey(true);
        }

        private static void InstanceOnSessionFileChanged(object? sender, EventArgs e)
        {

        }
    }
}
