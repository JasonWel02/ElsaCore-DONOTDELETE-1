//using Elsa;
using Elsa.Extensions;
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Attributes;
using Elsa.Workflows.Core.Models;
using Microsoft.Office.Interop.Excel;
using LIMM.Custom.Activities.Managers;
using System.Diagnostics;

namespace LIMM.Custom.Activities
{
    public class OpenExcel : CodeActivity
    {
        //File Name 
        //Model Name

        [Input] public Input<string> FileDir { get; set; } = default!;

        protected override void Execute(ActivityExecutionContext context)
        {

            //ExcelManager excelManager = new ExcelManager();

            var path = FileDir.Get(context);

            ExcelManager.InitializeExcel();

            ExcelManager.OpenWorkbook(path);

        }
    }
    }
