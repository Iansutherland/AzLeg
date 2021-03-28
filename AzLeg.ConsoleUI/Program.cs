using AzLeg.ConsoleUI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AzLeg.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                InitialScraping.ScrapeAndDump();
            }
            catch( Exception exception)
            {
                Console.WriteLine("\tERROR: " + exception.Message);
            }
        }
    }
}
