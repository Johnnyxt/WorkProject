using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using NPOI.HSSF.UserModel;
using HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment;

namespace JW8307A
{
    internal class ExcelHelper
    {
        public static DataTable Excel2DataTable(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("StartDateTime", typeof(string));
            dt.Columns.Add("EndDateTime", typeof(string));
            dt.Columns.Add("ItemCode", typeof(string));
            dt.Columns.Add("WorkOrder", typeof(string));
            dt.Columns.Add("SerialNumber", typeof(string));
            dt.Columns.Add("AteName", typeof(string));
            dt.Columns.Add("OpName", typeof(string));
            dt.Columns.Add("Outcome", typeof(string));
            dt.Columns.Add("ItemTestname", typeof(string));
            dt.Columns.Add("ItemTestOutcome", typeof(string));
            dt.Columns.Add("ItemTestStartDateTime", typeof(string));
            dt.Columns.Add("ItemTestEndDateTime", typeof(string));
            for (int i = 1; i <= 12; i++)
            {
                dt.Columns.Add(string.Concat("SubItemTestname", i), typeof(string));
                dt.Columns.Add(string.Concat("SubItemTestOutcome", i), typeof(string));
                dt.Columns.Add(string.Concat("SubItemTestDescription", i), typeof(string));
                dt.Columns.Add(string.Concat("SubItemTestcValue", i), typeof(string));
            }
            dt.Columns.Add("ItemName", typeof(string));
            dt.Columns.Add("OperationSequence", typeof(int));
            dt.Columns.Add("SiteCode", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Product", typeof(string));
            dt.Columns.Add("ProductLine", typeof(string));
            dt.Columns.Add("SystemOperator", typeof(string));
            dt.Columns.Add("AteVersion", typeof(string));
            dt.Columns.Add("cDefinitionname", typeof(string));
            dt.Columns.Add("cDefinitionversion", typeof(string));
            dt.Columns.Add("OpNumber", typeof(string));
            dt.Columns.Add("OpTeam", typeof(string));
            dt.Columns.Add("OpDepartment", typeof(string));
            dt.Columns.Add("Remark", typeof(string));

            try
            {
                var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                IWorkbook workbook = new HSSFWorkbook(fs);
                var sheet = workbook.GetSheetAt(0);
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数
                    var startRow = sheet.FirstRowNum + 1;
                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = dt.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        dt.Rows.Add(dataRow);
                    }
                }
                workbook.Close();
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        //public static System.Data.DataTable ExcelToDataTable(string fileName)
        //{
        //    System.Data.DataTable dt = new System.Data.DataTable();
        //    dt.Columns.Add("StartDateTime", typeof(string));
        //    dt.Columns.Add("EndDateTime", typeof(string));
        //    dt.Columns.Add("ItemCode", typeof(string));
        //    dt.Columns.Add("WorkOrder", typeof(string));
        //    dt.Columns.Add("SerialNumber", typeof(string));
        //    dt.Columns.Add("AteName", typeof(string));
        //    dt.Columns.Add("OpName", typeof(string));
        //    dt.Columns.Add("Outcome", typeof(string));
        //    dt.Columns.Add("ItemTestname", typeof(string));
        //    dt.Columns.Add("ItemTestOutcome", typeof(string));
        //    dt.Columns.Add("ItemTestStartDateTime", typeof(string));
        //    dt.Columns.Add("ItemTestEndDateTime", typeof(string));
        //    for (int i = 1; i <= 12; i++)
        //    {
        //        dt.Columns.Add(string.Concat("SubItemTestname", i), typeof(string));
        //        dt.Columns.Add(string.Concat("SubItemTestOutcome", i), typeof(string));
        //        dt.Columns.Add(string.Concat("SubItemTestDescription", i), typeof(string));
        //        dt.Columns.Add(string.Concat("SubItemTestcValue", i), typeof(string));
        //    }
        //    dt.Columns.Add("ItemName", typeof(string));
        //    dt.Columns.Add("OperationSequence", typeof(int));
        //    dt.Columns.Add("SiteCode", typeof(string));
        //    dt.Columns.Add("Description", typeof(string));
        //    dt.Columns.Add("Product", typeof(string));
        //    dt.Columns.Add("ProductLine", typeof(string));
        //    dt.Columns.Add("SystemOperator", typeof(string));
        //    dt.Columns.Add("AteVersion", typeof(string));
        //    dt.Columns.Add("cDefinitionname", typeof(string));
        //    dt.Columns.Add("cDefinitionversion", typeof(string));

        //    dt.Columns.Add("OpNumber", typeof(string));
        //    dt.Columns.Add("OpTeam", typeof(string));
        //    dt.Columns.Add("OpDepartment", typeof(string));
        //    dt.Columns.Add("Remark", typeof(string));
        //    string conStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1'";
        //    OleDbConnection myConn = new OleDbConnection(conStr);
        //    string strCom = " SELECT * FROM [Sheet0$]";
        //    myConn.Open();
        //    OleDbDataAdapter myCommand = new OleDbDataAdapter(strCom, myConn);
        //    // dt = new System.Data.DataTable();
        //    myCommand.Fill(dt);
        //    myConn.Close();
        //    return dt;
        //}

        //public static void DataTabletoExcel(System.Data.DataTable dtSource, string strFileName)
        //{
        //    if (dtSource == null)

        //        return;

        //    System.Data.DataTable sourceDataTable = ExcelToDataTable(strFileName);
        //    if (sourceDataTable.Rows.Count > 0)
        //    {
        //        sourceDataTable.Merge(dtSource);
        //        dtSource = sourceDataTable;
        //    }
        //    int rowNum = dtSource.Rows.Count;
        //    int columnNum = dtSource.Columns.Count;
        //    int rowIndex = 1;
        //    int columnIndex = 0;
        //    Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
        //    xlApp.DefaultFilePath = "";
        //    xlApp.DisplayAlerts = true;
        //    xlApp.SheetsInNewWorkbook = 1;
        //    Workbook xlBook = xlApp.Workbooks.Add(true);
        //    //将DataTable的列名导入Excel表第一行
        //    foreach (DataColumn dc in dtSource.Columns)
        //    {
        //        columnIndex++;
        //        xlApp.Cells.NumberFormat = "@"; //  如果数据中存在数字类型 可以让它变文本格式显示
        //        xlApp.Cells[rowIndex, columnIndex] = dc.ColumnName;
        //    }
        //    //将DataTable中的数据导入Excel中
        //    for (int i = 0; i < rowNum; i++)
        //    {
        //        rowIndex++;
        //        columnIndex = 0;
        //        for (int j = 0; j < columnNum; j++)
        //        {
        //            columnIndex++;

        //            xlApp.Cells[rowIndex, columnIndex] = dtSource.Rows[i][j].ToString();
        //        }
        //    }
        //    xlBook.SaveCopyAs(strFileName);
        //    xlBook.Close(false);
        //}

        public static void DataTabletoExcel(DataTable dtSource, string strFileName)
        {
            if (dtSource == null)

                return;

            DataTable sourceDataTable = Excel2DataTable(strFileName);
            if (sourceDataTable != null)
            {
                if (sourceDataTable.Rows.Count > 0)
                {
                    sourceDataTable.Merge(dtSource);
                    dtSource = sourceDataTable;
                }
            }

            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }

            IWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet0");

            //填充表头
            IRow dataRow = sheet.CreateRow(0);
            ICellStyle headStyle = workbook.CreateCellStyle();

            headStyle.Alignment = HorizontalAlignment.Center;
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 11;
            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            headStyle.SetFont(font);
            foreach (DataColumn column in dtSource.Columns)
            {
                dataRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                dataRow.GetCell(column.Ordinal).CellStyle = headStyle;
                sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 3) * 256);
            }

            //填充内容
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                dataRow = sheet.CreateRow(i + 1);
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    dataRow.CreateCell(j).SetCellValue(dtSource.Rows[i][j].ToString());
                }
            }

            //保存
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs);
                }
            }
            workbook.Close();
        }
    }
}