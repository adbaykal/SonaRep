using System.ComponentModel;
using System.Data;
using System.Text;

namespace SonaRep.Helper;

public class CsvHelper : ICsvHelper
{
    public void SaveToCsv<T>(List<T> data, string path)
    {
        var lines = new List<string>();
        IEnumerable<PropertyDescriptor> props = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>();
        var header = string.Join(",", props.ToList().Select(x => x.Name));
        lines.Add(header);
        var valueLines = data.Select(row => string.Join(",", header.Split(',').Select(a => row.GetType().GetProperty(a).GetValue(row, null))));
        lines.AddRange(valueLines);
        File.WriteAllLines(path, lines.ToArray());
    }

    public void SaveToCsv(DataTable table, string path)
    {
        var stringBuilder = new StringBuilder(); 

        var columnNames = table.Columns.Cast<DataColumn>().
            Select(column => column.ColumnName);
        stringBuilder.AppendLine(string.Join(",", columnNames));

        foreach (DataRow row in table.Rows)
        {
            IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
            stringBuilder.AppendLine(string.Join(",", fields));
        }

        File.WriteAllText(path, stringBuilder.ToString());
    }
}