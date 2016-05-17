using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MyElevator
{
    class Program 
    {
        private const string QUIT = "q";
        public static ILogger iMainLog;

        public static void Main(string[] args)
        {
        //where the magic happens...
        Start:
            Console.WriteLine("Welcome to David's awesome elevator!!");
            Console.WriteLine("How tall is the building that this elevator will be in?");

            int floor; string floorInput; Elevator elevator;

            floorInput = Console.ReadLine();

            if (Int32.TryParse(floorInput, out floor))
                elevator = new MyElevator(floor);
            else
            {
                
                Console.WriteLine("That' doesn't make sense...");
                Console.Beep();
                Thread.Sleep(2000);
                Console.Clear();
                goto Start;
            }
            string input = string.Empty;

            while (input != QUIT)
            {
                Console.WriteLine("Please press which floor you would like to go to");
                input = Console.ReadLine();

                if (Int32.TryParse(input, out floor))
                    elevator.FloorPress(floor);
                else if (input == QUIT)
                    Console.WriteLine("GoodBye!");
                else
                    Console.WriteLine("You have pressed an incorrect floor, Please try again");
            }
        }
    }

    public abstract class Elevator
    {
        //Defaults and Declarations
        //building has n floors

        protected bool[] floorReady;
        protected int CurrentFloor = 1;
        protected int topfloor;
        protected ElevatorStatus Status = ElevatorStatus.STOPPED;

        
        public virtual void Stop(int floor)
        {
            Status = ElevatorStatus.STOPPED;
            CurrentFloor = floor;
            floorReady[floor] = false;
            Console.WriteLine("Stopped at floor {0}", floor);
        }

        public virtual void Descend(int floor)
        {
            for (int i = CurrentFloor; i >= 1; i--)
            {
                if (floorReady[i])
                {
                    Stop(floor);
                    return;
                }
                else
                {
                    Console.WriteLine("Floor: {0}", i);
                    continue;
                }
            }

            Status = ElevatorStatus.STOPPED;
            Console.WriteLine("Waiting..");
        }

        public virtual void Ascend(int floor)
        {
            for (int i = CurrentFloor; i <= topfloor; i++)
            {
                if (floorReady[i])
                {
                    Stop(floor);
                    return;
                }
                else
                {
                    Console.WriteLine("Floor: {0}", i);
                    continue;
                }
            }

            Status = ElevatorStatus.STOPPED;
            Console.WriteLine("Waiting..");
        }
        
        public virtual void FloorPress(int floor){}

        public enum ElevatorStatus
        {
            UP,
            STOPPED,
            DOWN
        }
    }

    
    

    public class FileLogger : ILogger
    {
        public void Register(string error)
        {
            System.IO.File.WriteAllText(@"D:\Error.txt", error);
        }
    }
    public class DBLogger : ILogger
    {
        public void Register(string error)
        {
            //db code
        }
    }

    public class ConsoleMessage : IMessage
    {
        void ShowMessage(string message)
        {
            Console.WriteLine();
        }
    }

    public class RegisterError
    {
        private ILogger iLog = null;

        public RegisterError(ILogger i)
        {
            this.iLog = i;
        }

        public void RegisterErr(string message)
        {
            iLog.Register("Do not play..Its dangerous");
        }
    }
   
    public class MyElevator : Elevator,IStop
    {      
        
        public MyElevator(int NumberOfFloors)
        {
            floorReady = new bool[NumberOfFloors + 1];
            topfloor = NumberOfFloors;
        }
        public MyElevator() { }
        

        public void StayPut(int floor)
        {
            Console.WriteLine("That's our current floor");
        }

        public override void FloorPress(int floor)
        {
            if (floor > topfloor)
            {
                FileLogger fl = new FileLogger();
                RegisterError me = new RegisterError(fl);
                me.RegisterErr(string.Format("We only have {0} floors", topfloor));

                DBLogger dlog = new DBLogger();
                RegisterError me1 = new RegisterError(dlog);
                me1.RegisterErr(string.Format("We only have {0} floors", topfloor));
                Console.WriteLine("We only have {0} floors", topfloor);
                return;
            }
            
            floorReady[floor] = true;

            switch (Status)
            {

                case ElevatorStatus.DOWN:
                    Descend(floor);
                    break;

                case ElevatorStatus.STOPPED:
                    if (CurrentFloor < floor)
                        Ascend(floor);
                    else if (CurrentFloor == floor)
                        StayPut(floor);
                    else
                        Descend(floor);
                    break;

                case ElevatorStatus.UP:
                    Ascend(floor);
                    break;

                default:
                    break;
            }
        }
    }
}
