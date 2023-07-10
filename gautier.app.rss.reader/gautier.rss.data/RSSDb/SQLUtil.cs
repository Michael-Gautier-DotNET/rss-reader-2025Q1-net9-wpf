using System.Text;

namespace gautier.rss.data.RSSDb
{
    internal class SQLUtil
    {
        internal static StringBuilder CreateSQLInsertCMDText(string tableName, string[] columnNames)
        {
            StringBuilder ColumnNameSB = new();
            ColumnNameSB.AppendLine($"INSERT INTO {tableName} (");

            StringBuilder ColumnValuesSB = new();

            for (int ColI = 0; ColI < columnNames.Length; ColI++)
            {
                var ColumnName = columnNames[ColI];
                string ParamName = $"@{ColumnName}";

                string Sep = ",";

                if (ColI + 1 == columnNames.Length)
                {
                    Sep = string.Empty;
                }

                ColumnNameSB.AppendLine($"{ColumnName}{Sep}");
                ColumnValuesSB.AppendLine($"{ParamName}{Sep}");
            }

            ColumnNameSB.AppendLine(") VALUES (");
            ColumnValuesSB.AppendLine(");");

            StringBuilder CommandText = new();

            CommandText.Append($"{ColumnNameSB}{ColumnValuesSB};");

            return CommandText;
        }

        internal static StringBuilder CreateSQLUpdateCMDText(string tableName, string[] columnNames)
        {
            StringBuilder ColumnNameSB = new();
            ColumnNameSB.AppendLine($"UPDATE {tableName} SET ");

            for (int ColI = 0; ColI < columnNames.Length; ColI++)
            {
                var ColumnName = columnNames[ColI];
                string ParamName = $"@{ColumnName}";

                string Sep = ",";

                if (ColI + 1 == columnNames.Length)
                {
                    Sep = string.Empty;
                }

                ColumnNameSB.AppendLine($"{ColumnName} = {ParamName}{Sep}");
            }

            ColumnNameSB.AppendLine(" WHERE ");

            StringBuilder CommandText = new();

            CommandText.Append($"{ColumnNameSB}");

            return CommandText;
        }

    }
}
