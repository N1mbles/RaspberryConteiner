using System;
using System.Collections.Generic;
using System.IO;
using Ganss.Excel;

namespace Rasberry.Api.Excel
{
    public class ExcelHelper
    {
        public static void Save(string userName, List<TempData> temp)
        {
            //Name of excel file
            string dataToday = string.Concat(DateTime.Now.ToString("yyyy-MM-dd"), ".xlsx");
            ExcelMapper mapper = new ExcelMapper();

            string currentPath = Directory.GetCurrentDirectory();
            var pathWithFolder = Path.Combine(currentPath, "Statistic");

            if (!Directory.Exists(Path.Combine(currentPath, "Statistic")))
                Directory.CreateDirectory(Path.Combine(currentPath, "Statistic"));

            var newFile = Path.Combine(pathWithFolder, dataToday);  //path
            mapper.Save(newFile, temp, userName, true);

        }
    }
}
