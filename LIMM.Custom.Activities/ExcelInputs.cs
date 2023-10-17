using Elsa.Extensions;
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Attributes;
using Elsa.Workflows.Core.Models;
using LIMM.Custom.Activities.Managers;
using System.Diagnostics;

namespace LIMM.Custom.Activities
{
    public class ExcelInputs : CodeActivity
    {
        //File Name 
        //Model Name

        [Input] public Input<string> XMLFileName { get; set; } = default!;

        protected override void Execute(ActivityExecutionContext context)
        {

            //ExcelManager excelManager = new ExcelManager();

            var xml = XMLFileName.Get(context);

            ExcelManager.LoadInputs(xml);

        }
    }
}