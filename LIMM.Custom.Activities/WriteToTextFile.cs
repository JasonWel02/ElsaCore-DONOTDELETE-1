using Elsa.Workflows.Core.Attributes;
using Elsa.Workflows.Core.Models;
using Elsa.Workflows.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Permissions;
using Elsa.Extensions;

namespace LIMM.Custom.Activities
{
    public class WriteToTextFile : CodeActivity
    {
        [Input] public Input<string> Message { get; set; } = default!;
        [Input] public Input<string> FileDir { get; set; } = default!;

        protected override void Execute(ActivityExecutionContext context)
        {
            var message = Message.Get(context);

            var path = FileDir.Get(context);

            File.WriteAllText(path, message);

        }
    }
}
