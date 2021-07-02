using System;

namespace MiodenusAnimationConverter
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Start time: {DateTime.Now} {DateTime.Now.Millisecond} ms.");
            var mainController = new MainController();
            Console.WriteLine($"End time: {DateTime.Now} {DateTime.Now.Millisecond} ms.");
        }
    }
}