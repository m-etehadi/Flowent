using Flowent;
using Flowent.Samples.Basic.Commands;
using Flowent.Samples.Basic.Contexts;
using System.Reflection.Metadata.Ecma335;




var context = new SomeContext()
{
    ContextVar1 = 1,
    ContextVar2 = "2",
};



var command = new FlowBuilder<TestCommand1>()
   .Init.By(cmd => cmd.IntProp = 2, p => p.Status = "3")
   .Validate
       .IfIsNot(cmd => cmd.Status == "3").Throw(cmd => new Exception("Status must be '3'"))
       .If(cmd => cmd.Status == "1").Throw(cmd => new Exception("Status can not be '1'"))
   .EndValidate
   .On
       .Exception<ArgumentNullException>((cmd, ex) => Task.Run(() => Console.WriteLine("Value can not be null")))
       .ExecutedAsync(p =>
                   p.Status = "Done",
                   p => Console.WriteLine("Complted!"),
                   p => context.ContextVar2 = p.Output)
   .EndOn
   .Then()
       .Do<TestCommand2>(new FlowBuilder<TestCommand2>())
   .EndThen
   .If(cmd => cmd.IntProp == 11)
       .Do<TestCommand2>(new FlowBuilder<TestCommand2>()
                                .Init.By(p => p.Status = "Init2"))
   .ElseIf(p => true)
       .Do<TestCommand2>(new FlowBuilder<TestCommand2>()
                                .Init.By(() => new TestCommand2
                                {
                                    IntProp = 12,
                                    Status = "Else condition",
                                }))
   .EndIf;



command.Run().Wait();

Console.ReadLine();