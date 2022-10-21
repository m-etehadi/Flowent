using Flowent;
using Flowent.Samples.Basic.Commands;
using Flowent.Samples.Basic.Contexts;
using System.Reflection.Metadata.Ecma335;




var context = new SomeContext()
{
    ContextVar1 = 1,
    ContextVar2 = "2",
};



var command = new CommandBuilder<TestCommand>()
   .Init.By(cmd => cmd.IntProp = 2, p => p.Status = "3")
   //.By(() => new TestCommand
   //{
   //    IntProp = 1,
   //    Status = "Initilization status"
   //})
   .Validate
       .IfIsNot(cmd => cmd.Status == "3").Throw(cmd => new Exception("Status must be '3'"))
       .If(cmd => cmd.Status == "1").Throw(cmd => new Exception("Status can not be '1'"))
   .EndValidate
   .On
       .Exception((cmd, ex) =>
       {
           Console.WriteLine(ex);
           throw ex;
       })
       .Exception<ArgumentNullException>((cmd, ex) => Task.Run(() => Console.WriteLine("Value can not be null")))
       .ExecutedAsync(p =>
                   p.Status = "Done",
                   p => Console.WriteLine("Complted!"),
                   p => context.ContextVar2 = p.Output)
   .EndOn
   .If(cmd => cmd.IntProp == 11)
       .Run<TestCommand2>(new CommandBuilder<TestCommand2>()
                                .Init.By(p => p.Status2 = "Init2"))
   .ElseIf(p => true)
       .Run<TestCommand2>(new CommandBuilder<TestCommand2>()
                                .Init.By(() => new TestCommand2
                                {
                                    IntProp2 = 12,
                                    Status2 = "Else condition",
                                }))
   .EndCondition;



command.Run();