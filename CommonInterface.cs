using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyElevator
{
	interface CommonInterface
	{
	}

    interface IStop
    {
        void StayPut(int floor);
    }

    public interface ILogger
    {
        void Register(string error);
    }

    public interface IMessage
    {
        void ShowMessage(string message);
    }
}
