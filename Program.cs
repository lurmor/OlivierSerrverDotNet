using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OlivierSerrverDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpConector tcpConector = new TcpConector(8739);
            while (true)
            {
                tcpConector.Update();

            }
        }
    }
}