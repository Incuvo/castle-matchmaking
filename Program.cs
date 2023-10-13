using System;
using System.Collections.Generic;

namespace Castle_Feedbacker
{
    class Program
    {
        static void Main(string[] args)
        {
            MainModule.Instance.Init();

            while(Console.ReadKey().KeyChar != 'q')
            {

            }
        }
    }
}
