using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.Serialization;

namespace WalletConsoleTest
{
    public static class SqlExceptionHelper
    {
        public static SqlException CreateSqlException(int number, string message)
        {
            SqlErrorCollection collection = (SqlErrorCollection)FormatterServices.GetUninitializedObject(typeof(SqlErrorCollection));
            SqlError error = (SqlError)FormatterServices.GetUninitializedObject(typeof(SqlError));

            typeof(SqlError)
                .GetField("number", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(error, number);

            typeof(SqlError)
                .GetField("message", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(error, message);

            typeof(SqlErrorCollection)
                .GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(collection, new object[] { error });

            SqlException exception = (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

            typeof(SqlException)
                .GetField("_errors", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(exception, collection);

            return exception;
        }
    }

}
