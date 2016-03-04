using System;
using System.Data;
using System.Data.SqlClient;

namespace CodeSnippetLibrary.Data
{
    public class SqlExecutor : IDisposable
    {
        public int ExecuteNonQuery(SqlCommand cmd, string connectionString)
        {
            return (int)CommandExecute(cmd, connectionString, CommandExecuteType.NonQuery);
        }

        public SqlDataReader ExecuteReader(SqlCommand cmd, string connectionString)
        {
            return (SqlDataReader)CommandExecute(cmd, connectionString, CommandExecuteType.DataReader);
        }

        public TValue ExecuteScalar<TValue>(SqlCommand cmd, string connectionString)
        {
            object result = CommandExecute(cmd, connectionString, CommandExecuteType.Scalar);
            if (result == DBNull.Value)
            {
                result = null;
            }

            //先尝试强制类型转换，如果遇到值类型失败了，则尝试用Convert转换，如果再失败则正常抛出异常
            try
            {
                return (TValue)result;
            }
            catch
            {
                return (TValue)Convert.ChangeType(result, typeof(TValue));
            }
        }

        #region Close & Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Close();
                }
                _isDisposed = true;
            }
        }

        public void Close()
        {
            if (_currentSqlDataReader != null)
            {
                _currentSqlDataReader.Close();
                _currentSqlDataReader.Dispose();
                _currentSqlDataReader = null;
            }

            if (_currentSqlCommand != null)
            {
                _currentSqlCommand.Dispose();
                _currentSqlCommand = null;
            }

            if (_currentSqlConnection != null)
            {
                _currentSqlConnection.Close();
                _currentSqlConnection.Dispose();
                _currentSqlConnection = null;
            }
        }

        #endregion

        #region Private

        private object CommandExecute(SqlCommand cmd, string cs, CommandExecuteType type)
        {
            object result;
            cmd.CommandTimeout = 3600;
            _currentSqlCommand = cmd;
            _currentSqlConnection = SetSqlConnection(cmd, cs);

            _currentSqlConnection.Open();

            switch (type)
            {
                case CommandExecuteType.DataReader:
                    _currentSqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    result = _currentSqlDataReader;
                    break;
                case CommandExecuteType.NonQuery:
                    result = cmd.ExecuteNonQuery();
                    break;
                case CommandExecuteType.Scalar:
                    result = cmd.ExecuteScalar();
                    break;
                default:
                    result = null;
                    break;
            }

            if (type != CommandExecuteType.DataReader)
            {
                _currentSqlCommand.Dispose();
                _currentSqlCommand = null;

                _currentSqlConnection.Close();
                _currentSqlConnection.Dispose();
                _currentSqlConnection = null;
            }

            return result;
        }

        private static SqlConnection SetSqlConnection(SqlCommand cmd, string cs)
        {
            SqlConnection connection = new SqlConnection(cs);
            cmd.Connection = connection;
            return connection;
        }

        private bool _isDisposed;
        private SqlCommand _currentSqlCommand;
        private SqlDataReader _currentSqlDataReader;
        private SqlConnection _currentSqlConnection;

        private enum CommandExecuteType
        {
            NonQuery = 1,
            Scalar = 2,
            DataReader = 3
        }

        #endregion
    }
}
