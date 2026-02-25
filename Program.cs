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

            string? input = null;

            Task.Run(() =>
            {
                while (true)
                {
                    input = Console.ReadLine();
                }
            });

            TcpConector tcpConector = new TcpConector(8739);
            NTP ntp = new NTP(1230);
            while (true)
            {
                tcpConector.Update();
                ntp.Update();


                // if (Console.In.Peek() != -1)
                // {
                //var key = Console.ReadKey(true);
                //if (key.Key == ConsoleKey.Enter)
                //input = Console.ReadLine();
                if (input != null)
                {
                    var splitedInput = input.Split(' ');
                    if (splitedInput.Length == 2)
                    {

                        var unit = UnitColector.getInstance().GetUnit(uint.Parse(splitedInput[0]));
                        if (unit != null && unit.tcpStream != null)
                        {
                            TcpConector.SendMessage(unit.tcpStream, splitedInput[1]);
                        }
                    }
                    input = null;
                }
                //input += key.KeyChar;
                // }
            }
        }
    }
}