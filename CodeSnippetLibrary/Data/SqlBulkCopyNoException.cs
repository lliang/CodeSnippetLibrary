using System;
using System.Data;
using System.Threading;
using System.Data.SqlClient;

namespace CodeSnippetLibrary.Data
{
    public class SqlBulkCopyNoException : IDisposable
    {
        private const int BatchSize = 1000; //批处理数据条数
        private const int BulkCopyTimeout = 3600;

        readonly SqlConnection _conn;
        readonly SqlBulkCopy _bulkCopy;
        private DataTable _importdata;

        public SqlBulkCopyNoException(string connString, string tableName)
        {
            _conn = new SqlConnection(connString);
            _bulkCopy = new SqlBulkCopy(_conn)
            {
                DestinationTableName = tableName,
                BatchSize = BatchSize,
                BulkCopyTimeout = BulkCopyTimeout
            };
        }

        public void Import(DataTable dataTable)
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return;
            }

            try
            {
                _importdata = dataTable;
                Open();
                _bulkCopy.WriteToServer(_importdata);
            }
            catch (Exception)
            {
                if (dataTable.Rows.Count == 1)
                {
                    return;
                }
                int middle = dataTable.Rows.Count / 2;
                DataTable table = dataTable.Clone();
                for (int i = 0; i < middle; i++)
                {
                    table.ImportRow(dataTable.Rows[i]);
                }
                Import(table);

                table.Clear();
                for (int i = middle; i < dataTable.Rows.Count; i++)
                {
                    table.ImportRow(dataTable.Rows[i]);
                }
                Import(table);
            }
        }

        public void Dispose()
        {
            Close();
        }

        private void Close()
        {
            _importdata = null;
            if (_conn != null && _conn.State != ConnectionState.Closed)
            {
                _conn.Close();
            }
            if (_bulkCopy != null)
            {
                _bulkCopy.Close();
            }
        }

        private void Open()
        {
            if (_conn.State != ConnectionState.Closed) return;
            try
            {
                _conn.Open();
            }
            catch (SqlException)
            {
                Thread.Sleep(600000);
                Open();
            }
            catch (Exception)
            {
                Thread.Sleep(600000);
                Open();
            }
        }
    }
}
