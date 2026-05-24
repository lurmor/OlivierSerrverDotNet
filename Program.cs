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
            UnitColector.getInstance().LoadFomFile("Items.json");
            TcpConector tcpConector = new TcpConector(8739);
            NTP ntp = new NTP(1230);
            while (true)
            {
                tcpConector.Update();
                ntp.Update();

                if (input != null)
                {
                    ParseInput(input);
                    input = null;
                }

            }
        }
        static void ParseInput(string input)
        {
            var splitedInput = input.Split(' ');
            if (splitedInput.Length == 3)
            {
                // 1234567890 DT192.168.1.100
                UnitColector colector = UnitColector.getInstance();


                var unitFrom = UnitColector.getInstance().GetUnit(uint.Parse(splitedInput[1]));
                var unitTo = UnitColector.getInstance().GetUnit(uint.Parse(splitedInput[2]));
                if (unitFrom != null && unitTo != null)
                {
                    if (splitedInput[0][0] == 'C') colector.UnitConect(unitFrom, unitTo);
                    if (splitedInput[0][0] == 'D') colector.UnitDisConect(unitFrom, unitTo);
                }

                // if (unit != null && unit.tcpStream != null)
                // {
                //     TcpConector.SendMessage(unit.tcpStream, splitedInput[1]);
                // }
            }
            if (splitedInput.Length == 1)
            {
                if (splitedInput[0] == "Save")
                {
                    UnitColector.getInstance().SaveToFile("Items.json");
                }
                if (splitedInput[0] == "JsonUnits")
                {
                    Console.WriteLine(UnitColector.getInstance().ToJsonUnit());
                }
                if (splitedInput[0] == "JsonTransfers")
                {
                    Console.WriteLine(UnitColector.getInstance().ToJsonTrans());
                }
            }
        }
    }
}
