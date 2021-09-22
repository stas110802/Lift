using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Lift
{
    class Program
    {
        static void Main(string[] args)
        {
            var elevator = new Elevator(50, 650);
            elevator.WorkOfElevator();                                   
            
        }
    }
}
