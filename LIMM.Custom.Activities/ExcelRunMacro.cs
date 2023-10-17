using Elsa.Extensions;
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Attributes;
using Elsa.Workflows.Core.Models;
using LIMM.Custom.Activities.Managers;
using System.Diagnostics;

namespace LIMM.Custom.Activities
{
    public class ExcelRunMacro : CodeActivity
    {


        protected override void Execute(ActivityExecutionContext context)
        {

            //ExcelManager excelManager = new ExcelManager();

            ExcelManager.RunMacro();

        }
    }
}