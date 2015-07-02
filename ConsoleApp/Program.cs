using ConsoleApp.Controller;
using ConsoleApp.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TableController tc = new TableController();
            tc.WriteToConsole();
        }
    }
}
