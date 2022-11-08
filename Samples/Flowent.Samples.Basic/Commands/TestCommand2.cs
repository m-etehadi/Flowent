using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowent.Command;

namespace Flowent.Samples.Basic.Commands
{
    public class TestCommand2 : ICommand
    {
        public int IntProp { get; set; }
        public string Status { get; set; }


        public Task Execute()
        {
            Console.WriteLine("Here is Test command 2");
            return Task.CompletedTask;
        }

        public void Init() { }
        public void OnExecuted() { }

        public void OnExecuting() { }

        //public void OnValidated() { }

        //public void OnValidating() { }

    }
}
