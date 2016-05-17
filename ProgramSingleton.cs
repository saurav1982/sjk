using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ElevatorStateDesignPattern
{
    /// <summary>

  /// The 'Singleton' class

  /// </summary>

    class SingletonConsoleLogger : IMessage
    {

        private static SingletonConsoleLogger _instanceLogger;
        // Constructor is 'protected'
        protected SingletonConsoleLogger()
        {

        }
        public static SingletonConsoleLogger Instance()
        {
            // Uses lazy initialization.
            // Note: this is not thread safe.
            if (_instanceLogger == null)
            {
                _instanceLogger = new SingletonConsoleLogger();
            }
            return _instanceLogger;
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
    class SingletonConsoleMessage
    {

        private static SingletonConsoleMessage _instance;
        private static IMessage iLog = null;
        // Constructor is 'protected'
        protected SingletonConsoleMessage()
        {

        }
        public static SingletonConsoleMessage Instance(IMessage i)
        {
            // Uses lazy initialization.
            // Note: this is not thread safe.
            if (_instance == null)
            {
                _instance = new SingletonConsoleMessage();
                iLog = i;
            }
            return _instance;
        }
       
        public void RegisterErr(string message)
        {
            iLog.ShowMessage(message);
        }
    }
    class Program
    {
        private const string QUIT = "q";
        static void Main(string[] args)
        {
            Start:

            SingletonConsoleLogger cm = SingletonConsoleLogger.Instance(); //Can use Singleton and Lazy loading 
            SingletonConsoleMessage me = SingletonConsoleMessage.Instance(cm); //Can use Singleton and Lazy loading 
            me.RegisterErr("Welcome to Saurav's Basic elevator!!");
            me.RegisterErr("How tall is the building that this elevator will be in?");

            
            int floor; string floorInput; ElevatorConcrete eleConcrete;
            floorInput = Console.ReadLine();

            if (Int32.TryParse(floorInput, out floor))
            {
                eleConcrete = new ElevatorConcrete("Saurav Kundu", floor);
                Elevator.topfloor = floor;
                Elevator.floorReady = new bool[floor + 1];
            }
            else
            {
                me.RegisterErr("That' doesn't make sense...");
                Console.Beep();
                Thread.Sleep(2000);
                Console.Clear();
                goto Start;
            }
            
            //eleConcrete.FloorPress(1);
            //eleConcrete.FloorPress(floor);

            string input = string.Empty;

            while (input != QUIT)
            {
                me.RegisterErr("Please press which floor you would like to go to");
                input = Console.ReadLine();

                if (Int32.TryParse(input, out floor))
                    eleConcrete.FloorPress(floor);
                else if (input == QUIT)
                    me.RegisterErr("Goodbye!!"); 
                else
                    me.RegisterErr("You have pressed an incorrect floor, Please try again"); 
            }
        }
    }
    abstract class Elevator
    {
        protected ElevatorConcrete account;
        protected int floor;
        public static bool[] floorReady;
        public static int CurrentFloor = 1;
        public static int topfloor;
        protected int floorToGo;
        public static string Status = ElevatorStatus.STOPPED.ToString();
        //public static ElevatorStatus Status = ElevatorStatus.STOPPED;
        public ElevatorConcrete Account
        {
            get { return account; }
            set { account = value; }
        }
        public int Floor
        {
            get { return floor; }
            set { floor = value; }
        }
        public enum ElevatorStatus
        {
            UP,
            STOPPED,
            DOWN
        }
        public abstract void Handle(int floor);
        public abstract void Stop(int floor);
    }
    class ElevatorUpwards : Elevator
    {
        //Constructor
        public ElevatorUpwards(Elevator state)
            : this(state.Floor, state.Account)
        {

        }
        public ElevatorUpwards(int floor, ElevatorConcrete account)
        {
            this.account = account;       
            
        }
        public ElevatorUpwards(int floor)
        {
            Elevator.CurrentFloor = floor;
            //this.ElevatorBase = ElevatorBase.
        }

        public override void Handle(int floor)
        {
            for (int i = CurrentFloor; i <= topfloor; i++)
            {
                if (floorReady[i])
                {
                    //Console.WriteLine("Elevator stooped at: {0}", i);
                    Stop(i);
                    return;
                }
                else
                {
                    Console.WriteLine("Floor: {0}", i);
                    continue;
                }
            }

            Status = ElevatorStatus.STOPPED.ToString();
            Console.WriteLine("Waiting..");

        }
        public override void Stop(int floor)
        {
            Status = ElevatorStatus.STOPPED.ToString();
            CurrentFloor = floor;
            floorReady[floor] = false;
            Console.WriteLine("Stopped at floor {0}", floor);
        }
        private void StateChangeCheck(int floor)
        {
            if (Elevator.CurrentFloor > floor)
            {
                account.State = new ElevatorDownwards(floor);
            }

        }
    }
    class ElevatorDownwards : Elevator
    {
        //Constructor
        public ElevatorDownwards(Elevator state)
            : this(state.Floor, state.Account)
        {

        }
        public ElevatorDownwards(int floor, ElevatorConcrete account)
        {
            this.account = account;  
            
        }
        
        public ElevatorDownwards(int floor)
        {
            Elevator.CurrentFloor = floor;            
        }
        public override void Handle(int floor)
        {
            for (int i = CurrentFloor; i >= 1; i--)
            {
                if (floorReady[i])
                {
                    Console.WriteLine("Elevator stooped at: {0}", i);
                    break;
                }
                else
                {
                    Console.WriteLine("Floor: {0}", i);
                    continue;
                }
            }

            Status = ElevatorStatus.STOPPED.ToString();
            Console.WriteLine("Waiting..");

        }
        public override void Stop(int floor)
        {
            Status = ElevatorStatus.STOPPED.ToString();
            CurrentFloor = floor;
            floorReady[floor] = false;
            Console.WriteLine("Stopped at floor {0}", floor);
        }
        private void StateChangeCheck(int floor)
        {
            if (Elevator.CurrentFloor < floor)
            {
                account.State = new ElevatorUpwards(floor);
            }

        }
    }
    class ElevatorController : Elevator
    {
        //Constructor
        public ElevatorController(Elevator state)
            : this(state.Floor, state.Account)
        {

        }
        public ElevatorController(int floor, ElevatorConcrete account)
        {
            this.account = account;  
            
        }
        
        public override void Handle(int floor)
        {
            if(Elevator.CurrentFloor < floor)
                account.State = new ElevatorUpwards(this);
            else
                account.State = new ElevatorDownwards(this);
        }
        public override void Stop(int floor)
        {

        }
    }

    public interface IFloorPressed
    {
        void FloorPress(int floor);
    }

    public interface IMessage
    {
        void ShowMessage(string message);
    }

    class ConsoleLogger : IMessage
    {
        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class ConsoleMessage
    {
        private IMessage iLog = null;

        public ConsoleMessage(IMessage i)
        {
            this.iLog = i;
        }

        public void RegisterErr(string message)
        {
            iLog.ShowMessage(message);
        }
    }

    class ElevatorConcrete : IFloorPressed
    {
        private Elevator _state;
        private string _owner;
        private int maxFloor;

        // Constructor
        public ElevatorConcrete(string owner, int maxFloor)
        {
            // New accounts are 'Silver' by default
            this._owner = owner;
            this._state = new ElevatorUpwards(1, this);
            this.maxFloor = maxFloor;

        }

        // Properties
        public int Floor
        {
            get { return _state.Floor; }
        }
        public Elevator State
        {
            get { return _state; }
            set { _state = value; }

        }

        public void FloorPress(int floor)
        {
            ElevatorController ec = new ElevatorController(floor,this);

            if (floor > maxFloor)
            {
                Console.WriteLine("We only have {0} floors", maxFloor);
                return;
            }

            Elevator.floorReady[floor] = true;

            ec.Handle(floor);
            if (Elevator.CurrentFloor > floor)
            {
                //this._state = new ElevatorDownwards(1, this);
                _state.Handle(floor);
            }
            else if (Elevator.CurrentFloor == floor)
            {
                Console.WriteLine("That's our current floor"); 
            }
            else
            {
                _state.Handle(floor);
            }

            

        }

    }
}
