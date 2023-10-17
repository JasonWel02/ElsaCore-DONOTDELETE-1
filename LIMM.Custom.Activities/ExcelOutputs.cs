using Elsa.Extensions;
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Attributes;
using Elsa.Workflows.Core.Models;
using LIMM.Custom.Activities.Managers;
using System.Diagnostics;

namespace LIMM.Custom.Activities
{
    public class ExcelOutputs : CodeActivity
    {

        [Input] public Input<string> XMLFileName { get; set; } = default!;

        protected override void Execute(ActivityExecutionContext context)
        {

            var xmlfile = XMLFileName.Get(context);

            //ExcelManager excelManager = new ExcelManager();

            ExcelManager.LoadOutputs(xmlfile);

        }
    }
}