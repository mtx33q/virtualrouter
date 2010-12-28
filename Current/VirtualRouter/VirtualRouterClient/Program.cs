using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualRouterClient
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            SingleInstanceManager manager = new SingleInstanceManager();
            manager.Run(args);
        }
    }
}
