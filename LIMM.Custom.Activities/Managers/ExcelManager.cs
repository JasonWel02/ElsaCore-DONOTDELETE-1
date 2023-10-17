//using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using VBA = Microsoft.Vbe.Interop;
using Newtonsoft.Json.Linq;
using Microsoft.Office.Interop.Excel;
using System.Collections;
using Microsoft.Vbe.Interop;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

namespace LIMM.Custom.Activities.Managers
{

    public static class ExcelManager
    {
        public static Excel.Application excelApp;
        public static Excel.Workbook workbook;
        public static string filepath;

        public static void InitializeExcel()
        {
            excelApp = new Excel.Application();
        }

        public static void OpenWorkbook(string filePath)
        {
            if (excelApp != null)
            {
                workbook = excelApp.Workbooks.Open(filePath);


            }
        }

        public static void LoadInputs(string xmlfilename)
        {
            var pythonpath = "C:\\Python37\\python.exe";
            var scriptpath = "C:\\Users\\jmw\\Desktop\\ResourcesForWorkflow\\ParsingValuesInputs.py";
            var xmlpath = "C:\\Users\\jmw\\Desktop\\ResourcesForWorkflow\\" + xmlfilename;
            var filepath2 = "C:\\Users\\jmw\\Desktop\\ResourcesForWorkflow\\InputPathVar.txt";
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(pythonpath)
            {
                Arguments = scriptpath + " " + xmlpath,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            //var output2 = "parmsFromXML; xmlFileName=C:\\Users\\jmw\\Desktop\\ResourcesForWorkflow\\analysis_1_10063_10070.xml  initRowIndex=-1\r\nC:\\Users\\jmw\\Desktop\\ResourcesForWorkflow\\ParsingValues.py";

            //171
            p.Start();

            string output = p.StandardOutput.ReadToEnd();

            Console.WriteLine(output);
            string output2 = output[113..];

            p.WaitForExit();

            Console.WriteLine(output2);

            Dictionary<string, string> dict = new Dictionary<string, string>();

            List<string> numbers = new List<string>();

            JObject jsonObject = JObject.Parse(output2);


            Names wbNames = workbook.Names;
            foreach (Name n in wbNames)
            {

                foreach (KeyValuePair<string, JToken?> n2 in jsonObject)
                {
                    if (n.Name == n2.Key)
                    {

                        var parsedvalue = n.Value.Replace("=", "");
                        var parsedvalue2 = parsedvalue.Replace("!", "/");
                        var parsedvalue3 = parsedvalue2.Replace("$", "");
                        var parsedtheparsed = parsedvalue3.Split('/');


                        for (int i = 0; i < parsedtheparsed.Count() - 1; i++)
                        {
                            string currentKey = parsedtheparsed[i];
                            string nextValue = parsedtheparsed[i + 1];
                            dict[currentKey] = nextValue;
                        }
                        foreach (KeyValuePair<string, string> v in dict)
                        {
                            Console.WriteLine(v.Key + " = " + v.Value);
                            Excel.Worksheet sheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[v.Key];
                            Excel.Range cell = sheet.get_Range(n.Name);
                            cell.Value = n2.Value["Value"];

                        }
                    }
                }
            }


            File.WriteAllText(filepath2, output);

            var xmlfilename2 = xmlfilename.Replace("xml", "xlsm");

            filepath = xmlfilename2;


            //Console.ReadLine();
            p.Close();
        }


        public static void LoadOutputs(string xmlfilename)
          {
            var pythonpath = "C:\\Python37\\python.exe";
            var scriptpath = "C:\\Users\\jmw\\Desktop\\ResourcesForWorkflow\\ParsingValuesOutputs.py";
            var xmlpath = "C:\\Users\\jmw\\Desktop\\ResourcesForWorkflow\\" + xmlfilename;
            var filepath = "C:\\Users\\jmw\\Desktop\\ResourcesForWorkflow\\InputPathVar.txt";
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(pythonpath)
            {
                Arguments = scriptpath + " " + xmlpath,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            //171
            p.Start();

            string output = p.StandardOutput.ReadToEnd();

            string output2 = output[113..];

            p.WaitForExit();

            Console.WriteLine(output2);


            //var parsedjson = JsonConvert.DeserializeObject(output2);

            var jsonObject = JObject.Parse(output2);
            //jsonObject["AVG_NUM"]["Value"] = 1;
            //foreach(KeyValuePair<string, Newtonsoft.Json.Linq.JToken?> v in jsonObject)
            //{
            //    Console.WriteLine(v.Key+ " "+ v.Value);
            //}

            ////JObject modifiedobject = JObject(jsonObject`)


            Names wbNames = workbook.Names;

            Dictionary<string, string> dict = new Dictionary<string, string>();

            Dictionary<string, string> transformedict = new Dictionary<string, string>();


            List<string> listforoutputinfo = new List<string>();


            foreach (Name wbName in wbNames)
            {
                Console.WriteLine(wbName.Name);



                foreach (KeyValuePair<string, JToken?> kv in jsonObject)
                {
                    if (wbName.Name == kv.Key)
                    {
                        Console.WriteLine("Names " + wbName.Name + " " + kv.Key);
                        Console.WriteLine(kv.Value);
                        Console.WriteLine(wbName.Value);


                        var parsedvalue = wbName.Value.Replace("=", "");
                        var parsedvalue2 = parsedvalue.Replace("!", "/");
                        var parsedvalue3 = parsedvalue2.Replace("$", "");
                        var parsedtheparsed = parsedvalue3.Split('/');

                        for (int i = 0; i < parsedtheparsed.Count() - 1; i++)
                        {
                            string currentKey = parsedtheparsed[i];
                            string nextValue = parsedtheparsed[i + 1];
                            dict[currentKey] = nextValue;
                        }

                        foreach (KeyValuePair<string, string> v in dict)
                        {
                            Console.WriteLine(v.Key + " = " + v.Value);
                            Excel.Worksheet sheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[v.Key];
                            Excel.Range cell = sheet.get_Range(wbName.Name);
                            var cellvalue = cell.Value;
                            Console.WriteLine(cellvalue);





                            jsonObject[kv.Key]["Value"] = cellvalue.ToString();

                        }


                    }

                }
            }

            File.WriteAllText(filepath, output);

            foreach (KeyValuePair<string, JToken?> v in jsonObject)
            {
                Console.WriteLine(v.Key + " " + v.Value);
            }
            p.Close();
        }


        public static void RunMacro()
        {
            Console.WriteLine(filepath);
            Console.WriteLine("Hello!!!!");

            List<string> listOfMacros = new List<string>();

            var project = workbook.VBProject;
            var projectName = project.Name;
            var procedureType = Microsoft.Vbe.Interop.vbext_ProcKind.vbext_pk_Proc;

            foreach (var component in project.VBComponents)
            {
                VBA.VBComponent vbComponent = component as VBA.VBComponent;
                if (vbComponent != null)
                {
                    string componentName = vbComponent.Name;
                    var componentCode = vbComponent.CodeModule;
                    int componentCodeLines = componentCode.CountOfLines;

                    int line = 1;
                    while (line < componentCodeLines)
                    {
                        string procedureName = componentCode.get_ProcOfLine(line, out procedureType);
                        if (procedureName != string.Empty)
                        {
                            int procedureLines = componentCode.get_ProcCountLines(procedureName, procedureType);
                            int procedureStartLine = componentCode.get_ProcStartLine(procedureName, procedureType);
                            var allCodeLines = componentCode.get_Lines(procedureStartLine, procedureLines);

                            Regex regex = new Regex("Macro\r\n' (.*?)\r\n'\r\n\r\n'");
                            var v = regex.Match(allCodeLines);
                            string comments = v.Groups[1].ToString();

                            //if (comments.IsEmpty()) { comments = "No comment is written for this Macro"; }

                            line += procedureLines - 1;
                            listOfMacros.Add(procedureName);
                        }
                        line++;
                    }
                }

            }
            //Console.WriteLine("FILEPATH = " + filepath);
            foreach (var letters in listOfMacros)
            {

                excelApp.GetType().InvokeMember("Run", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, excelApp, new object[] { filepath + "!Module1." + letters });
                Console.WriteLine("LETTERS " + letters.ToString());
            }
            //File.WriteAllText(filepath, output);



        }


        public static void GetChart()
        {
            List<Chart> allCharts = new List<Chart>();

            foreach (Worksheet worksheet in workbook.Sheets)
            {
                ChartObjects chartObjects = worksheet.ChartObjects() as ChartObjects;

                if (chartObjects != null)
                {
                    foreach (ChartObject chartObj in chartObjects)
                    {
                        allCharts.Add(chartObj.Chart);
                    }
                }
            }

            for (int i = 0; i < allCharts.Count; i++)
            {
                // Generate a unique file name for each chart
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string outputPath = "C:\\LIMMV3\\queue\\outgoing\\analysis_1_10063_10070\\Chart.png";

                // Refresh the chart data (if needed)
                allCharts[i].Refresh();

                // Save the chart as an image
                allCharts[i].Export(outputPath, "PNG");

                Console.WriteLine($"Saved chart {i + 1} to '{outputPath}'");
            }
            //foreach (Worksheet worksheet in workbook.Worksheets)
            //{
            //    ChartObject chartObject = null
            //    foreach(ChartObject chartObj in worksheet.ChartObjects())
            //}

        }

        public static void CloseWorkbook()
        {
            if (workbook != null)
            {
                workbook.Save();
                workbook.Close();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                workbook = null;
            }
        }

        public static void CleanupExcel()
        {
            if (workbook != null)
            {
                CloseWorkbook();
            }

            if (excelApp != null)
            {
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                excelApp = null;
            }
        }
    }
}

