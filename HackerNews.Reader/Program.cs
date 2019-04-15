using System;

namespace HackerNews.Reader
{
    class Program
    {
        static void Main(string[] args)
        {
            var scraper = new Reader(args[0], args[2]);
            scraper.Run();
            scraper.ShowExceptions();

            Console.ReadLine();
        }
    }
}
