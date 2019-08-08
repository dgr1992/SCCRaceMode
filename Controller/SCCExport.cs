using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Avalonia.Controls;
using SCCRaceMode.Models;

namespace SCCRaceMode.Controller
{
    public class SCCExport
    {
        public async System.Threading.Tasks.Task ExportToCSVAsync(LinkedList<Round> rounds, LinkedList<Driver> drivers, Window parent)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "CSV", Extensions = { "csv" } });
            dlg.InitialFileName = DateTime.Now.ToString("yyyyMMdd")+"_SCCHeatArrangement.csv";
            string result = await dlg.ShowAsync(parent);
            if (result != null)
            {
                string filePath = Path.Combine(result);
                string delimiter = ";";  
                    
                StringBuilder sb = new StringBuilder();  
                foreach (Round round in rounds)
                {
                    sb.AppendLine(string.Join(delimiter, round.Description));
                    sb.AppendLine();

                    //Row = grid
                    //Column = Heat
                    string[] output = new string[round.Heats.Length * 3];
                    for (int pos = 0; pos < round.HeatSize; pos++)
                    {
                        for (int heat = 0; heat < round.Heats.Length; heat++)
                        {
                            //Pos
                            output[heat * 3] = round.Heats[heat].StartingGrid[pos].PosNum.ToString();
                            //Name
                            output[heat * 3 + 1] = round.Heats[heat].StartingGrid[pos].TheDriver.Name;
                        }
                        sb.AppendLine(string.Join(delimiter, output));
                    }
                    
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine();
                }

                //Statistic
                sb.AppendLine(string.Join(delimiter, "Statistics"));
                sb.AppendLine();
                foreach (Driver driver in drivers)
                {
                    sb.AppendLine(string.Join(delimiter, new string[]{driver.Name,driver.AvgStartingPos.ToString()}));
                }


                File.WriteAllText(filePath, sb.ToString());
            }
        }
    }
}