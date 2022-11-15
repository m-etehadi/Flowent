using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowent.Command;

public interface ICommandInitializer
{
    Task Initialize();
}