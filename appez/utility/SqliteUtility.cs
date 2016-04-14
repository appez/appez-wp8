using Community.CsharpSqlite;
using appez.constants;
using appez.exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLiteClient;
using System;
using System.IO;
using System.Text;

namespace appez.utility
{
    /// <summary>
    /// Utility class that helps execute SQlite queries and
    /// perform operations for databaseService
    /// </summary>
    public class SqliteUtility : IDisposable
    {
        #region variables
        private bool dbOperationCompletion = false;
        SQLiteConnection sqliteDb = null;
        private String appDbName = null;
        private bool _disposed;
        public int QueryExceptionType { get; set; }
        #endregion

        public SqliteUtility()
        {
            _disposed = false;
        }

        public SqliteUtility(String dbName)
        {
            this.appDbName = dbName;
            _disposed = false;
        }

        /// <summary>
        /// Convert sqlite status code to framework status code.
        /// </summary>
        /// <param name="rowsAffected">Sqlite status code</param>
        /// <returns>Framework status code</returns>
        public bool ResultString(int rowsAffected)
        {
            if (rowsAffected >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// create new sqlite connection.
        /// </summary>
        /// <param name="dbName">DataBase name</param>
        /// <returns>Status code that show operation was successful or not</returns>
        public bool OpenDatabase()
        {

            if (sqliteDb == null)
            {
                String dataBaseName = this.appDbName;
                try
                {
                    sqliteDb = new SQLiteConnection(dataBaseName);
                    sqliteDb.Open();
                }
                catch (SQLiteException)
                {
                    throw new MobiletException(ExceptionTypes.DB_OPERATION_ERROR);
                }
            }
            if (sqliteDb._open)
            {
                return dbOperationCompletion = true;
            }
            else
            {
                return dbOperationCompletion = false;
            }

        }

        /// <summary>
        /// Close sqlite connection.    
        /// </summary>
        /// <returns></returns>
        public bool CloseDatabase()
        {

            if ((sqliteDb != null) && (sqliteDb._open))
            {
                sqliteDb.Dispose();
            }

            if ((sqliteDb != null) && (!sqliteDb._open))
            {
                dbOperationCompletion = true;
            }
            else
            {
                dbOperationCompletion = false;
            }

            return dbOperationCompletion;

        }

        /// <summary>
        /// Method for update, write, create, insert operation.
        /// </summary>
        /// <param name="queryString">query</param>
        /// <returns>Status code that show operation was successful or not</returns>
        public bool ExecuteDbQuery(String queryString)
        {
            if (sqliteDb != null)
            {

                try
                {

                    if (!queryString.ToLower().Contains("drop"))
                    {
                        sqliteDb.BeginTransaction();
                        SQLiteCommand cmd = sqliteDb.CreateCommand(queryString);
                        dbOperationCompletion = ResultString(cmd.ExecuteNonQuery());
                        sqliteDb.CommitTransaction();
                    }
                    else
                    {
                        SQLiteCommand cmd = sqliteDb.CreateCommand(queryString);
                        dbOperationCompletion = ResultString(cmd.ExecuteNonQuery());
                    }
                    return dbOperationCompletion;
                }
                catch (SQLiteException)
                {
                    if (sqliteDb.TransactionOpened)
                        sqliteDb.RollbackTransaction();
                    dbOperationCompletion = false;
                    throw new MobiletException(ExceptionTypes.DB_OPERATION_ERROR);

                }
            }
            return dbOperationCompletion = false;
        }

        /// <summary>
        /// Method to be used when result data to be sent.
        /// </summary>
        /// <param name="queryString">query string</param>
        /// <returns>result data as string</returns>
        public string ExecuteReadTableQuery(String queryString)
        {
            if (sqliteDb != null)
            {
                Sqlite3.Vdbe stmt = new Sqlite3.Vdbe();

                int resultCode = Sqlite3.sqlite3_prepare_v2(sqliteDb._db, queryString, queryString.Length, ref stmt, 0);

                if (resultCode == Sqlite3.SQLITE_OK)
                {
                    return PrepareQueryData(stmt);
                }
                else
                {
                    if (resultCode == Sqlite3.SQLITE_ERROR)
                    {
                        this.QueryExceptionType = ExceptionTypes.DB_TABLE_NOT_EXIST_ERROR;
                    }
                    else
                    {
                        this.QueryExceptionType = ExceptionTypes.DB_QUERY_EXEC_ERROR;
                    }
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Create result string.
        /// </summary>
        /// <param name="stmt">Statement</param>
        /// <returns>string</returns>
        public string PrepareQueryData(Sqlite3.Vdbe stmt)
        {
            String queryResponse = null;

            try
            {
                StringBuilder sb = new StringBuilder();
                StringWriter readQueryResponse = new StringWriter(sb);
                using (JsonWriter jsonWriter = new JsonTextWriter(readQueryResponse))
                {
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName(CommMessageConstants.MMI_RESPONSE_PROP_APP_DB);
                    jsonWriter.WriteValue(this.appDbName);
                    jsonWriter.WritePropertyName(CommMessageConstants.MMI_RESPONSE_PROP_DB_RECORDS);
                    jsonWriter.WriteStartArray();

                    if (!sqliteDb._open)
                    {
                        sqliteDb.Open();
                    }

                    // looping through all rows
                    int columnCount = 0;
                    while (Sqlite3.sqlite3_step(stmt) == Sqlite3.SQLITE_ROW)
                    {
                        columnCount = Sqlite3.sqlite3_column_count(stmt);
                        jsonWriter.WriteStartObject();
                        if (columnCount > 0)
                        {
                            for (int columnNumber = 0; columnNumber < columnCount; columnNumber++)
                            {
                                jsonWriter.WritePropertyName(Sqlite3.sqlite3_column_name(stmt, columnNumber));

                                var column_type = Sqlite3.sqlite3_column_type(stmt, columnNumber);

                                switch (column_type)
                                {
                                    case Sqlite3.SQLITE_INTEGER:
                                        jsonWriter.WriteValue(Sqlite3.sqlite3_column_int(stmt, columnNumber));
                                        break;
                                    case Sqlite3.SQLITE_FLOAT:
                                        jsonWriter.WriteValue(Sqlite3.sqlite3_column_double(stmt, columnNumber));
                                        break;
                                    case Sqlite3.SQLITE_TEXT:
                                        jsonWriter.WriteValue(Sqlite3.sqlite3_column_text(stmt, columnNumber));
                                        break;
                                    case Sqlite3.SQLITE_BLOB:
                                        jsonWriter.WriteValue(Sqlite3.sqlite3_column_blob(stmt, columnNumber));
                                        break;
                                    default:
                                        break;
                                }


                            }

                        }
                        jsonWriter.WriteEndObject();

                    }
                    jsonWriter.WriteEndArray();
                    jsonWriter.WriteEndObject();
                }
                queryResponse = readQueryResponse.ToString();
            }
            catch (Exception)
            {
                if (sqliteDb.TransactionOpened)
                    sqliteDb.RollbackTransaction();
                queryResponse = null;
            }

            Sqlite3.sqlite3_finalize(ref stmt);
            return queryResponse;
        }


        public void Dispose()
        {
            Dispose(true);

            // Call SupressFinalize in case a subclass implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (sqliteDb != null)
                    {
                        sqliteDb.Dispose();

                    }
                }

                sqliteDb = null;
                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }

    }
}
