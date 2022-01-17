using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace DMS_Synchronization
{
    public enum DataType
    {
        StringField,
        NumericField,
        ImageField
    }
    public struct Field
    {
        public string Name;
        public object Value;
        public DataType FieldType;
        public SqlDbType image;
    }
    /// <summary>
    /// The DatatBase Layer for Data Processing
    /// </summary>
    public class SQLDBConnection
	{
		#region Declaring Varaiables & Objects
		public static string ConStr;
		
        public static string DataBaseName;
        public static string Server;

        public static string Timeout;
        public static string Trusted;
        public static string booling;
        public static string PersistSecurity;
        public static string ConnectionEncrypt;
        public static string ConnectionType;

		private static string User;
		private static string CurrentPassword;

        private static bool isConOpened = false;
		private static DataTable Dt;
		private static SqlDataAdapter Adapter;
		private static SqlConnection Connection;
		private static SqlCommand Cmd;

        private static string Criteria = string.Empty;
        #endregion

		#region Declaring Properities
		public static string ConnectionString
		{
			set
			{
				ConStr = value;
			}
			get
			{
				return ConStr;
			}
		}

		public static string DataSource
		{
			set
			{
				DataBaseName = value;
			}
			get
			{
				return DataBaseName;
			}
		}

		public static string ServerName
		{
			set
			{
				Server = value;
			}
			get
			{
				return Server;
			}
		}

		public static string UserID
		{
			set
			{
				User = value;
			}
			get
			{
				return User;
			}
		}

		public static string Password
		{
			set
			{
				CurrentPassword = value;
			}
			get
			{
				return  CurrentPassword;
			}
		}

        public static bool IsConOpened
        {
            get
            {
                return isConOpened;
            }
        }
		#endregion

		#region Declaring Methods
        /// <summary>
        ///  The procedure for inserting rows in the database
        /// </summary>
        /// <param name="Fields">The Parameter Fields</param>
        /// <param name="Query">The query to be executed</param>
        /// <returns></returns>
        public static int InsertData(Field[] Parameters, string Query)
        {
            return ExecuteQuery(Query, Parameters);
        }

		/// <summary>
		/// The procedure for inserting rows in the database
		/// </summary>
		/// <param name="Query">The query to be executed</param>
		/// <returns></returns>
		public static int InsertData(string Query)
		{
			return ExecuteQuery(Query);
		}


        /// <summary>
        /// The procedure for creating the insert query and execute it
        /// </summary>
        /// <param name="TableName">The name of the table</param>
        /// <param name="Fields">The fields to be updated</param>
        /// <returns></returns>
        public static void InsertData(string TableName, Field[] Parameters)
        {
            string Criteria = "Insert into " + TableName + " (";
            for (int i = 0; i < Parameters.Length; i++)
            {
                if (i == Parameters.Length - 1)
                    Criteria += Parameters[i].Name + ")";
                else
                    Criteria += Parameters[i].Name + ",";
            }
            Criteria += " Values (";
            for (int i = 0; i < Parameters.Length; i++)
            {
                Criteria += "@" + Parameters[i].Name;

                if (i == Parameters.Length - 1)
                    Criteria += ")";
                else
                    Criteria += ",";
            }

            SQLDBConnection.ExecuteQuery(Criteria, Parameters);
        }


		/// <summary>
		/// The procedure for updating rows in the database
		/// </summary>
		/// <param name="Query">The query to be executed</param>
		/// <returns></returns>
		public static int UpdateData(string Query)
		{
			return ExecuteQuery(Query);
		}


        /// <summary>
        /// The procedure for updating rows in the database
        /// </summary>
        /// <param name="Query">The query to be executed</param>
        /// <returns></returns>
        public static int UpdateData(Field[] Parameters, string Query)
        {
            return ExecuteQuery(Query, Parameters);
        }


        /// <summary>
        /// The procedure for creating the update query and execute it
        /// </summary>
        /// <param name="TableName">The name of the table</param>
        /// <param name="Fields">The fields to be updated</param>
        /// <param name="Conditions">The fields that are specialized for condition</param>
        /// <param name="AndCondition">True : Using And condition, False : Using Or condition</param>
        /// <returns></returns>
        public static void UpdateData(string TableName, Field[] Parameters, Field[] Conditions, bool AndCondition)
        {
            Criteria = "Update " + TableName + " Set ";
            for (int i = 0; i < Parameters.Length; i++)
            {
                Criteria += Parameters[i].Name + " = ";
                Criteria += "@" + Parameters[i].Name;

                if (i == Parameters.Length - 1)
                {
                    Criteria += " Where ";
                }
                else
                {
                    Criteria += ", ";
                }
            }


            for (int i = 0; i < Conditions.Length; i++)
            {
                Criteria += Conditions[i].Name + " = ";

                if (Conditions[i].FieldType == DataType.StringField)
                {
                    Criteria += "'" + Conditions[i].Value + "'";
                }
                else
                {
                    Criteria += Conditions[i].Value;
                }
                if (i != Conditions.Length - 1)
                {
                    if (AndCondition)
                    {
                        Criteria += " And ";
                    }
                    else
                    {
                        Criteria += " Or ";
                    }
                }
            }

            SQLDBConnection.ExecuteQuery(Criteria, Parameters);
        }

		/// <summary>
		/// The procedure for deleting rows in the database
		/// </summary>
		/// <param name="Query">The query to be executed</param>
		/// <returns></returns>
		public static int DeleteData(string Query)
		{
			return ExecuteQuery(Query);
		}


        /// <summary>
        /// The procedure for creating the delete query and execute it
        /// </summary>
        /// <param name="TableName">The name of the table</param>
        /// <param name="Conditions">The fields that are specialized for condition</param>
        /// <param name="AndCondition">True : Using And condition, False : Using Or condition</param>
        /// <returns></returns>
        public static int DeleteData(string TableName, Field[] Parameters, bool AndCondition)
        {
            Criteria = "Delete from " + TableName + " Where ";
            for (int i = 0; i < Parameters.Length; i++)
            {
                Criteria += Parameters[i].Name + " = ";

                if (Parameters[i].FieldType == DataType.StringField)
                {
                    Criteria += "'" + Parameters[i].Value + "'";
                }
                else
                {
                    Criteria += Parameters[i].Value;
                }
                if (i != Parameters.Length - 1)
                {
                    if (AndCondition)
                    {
                        Criteria += " And ";
                    }
                    else
                    {
                        Criteria += " Or ";
                    }
                }
            }
            return SQLDBConnection.ExecuteQuery(Criteria, Parameters);
        }


        public  void ConString(string con)
        {
            // Back office connection string
            ConStr = con;
            /*
            if (ConnectionType == "Cloud")
                ConStr = "Asynchronous Processing=true; Server=" + ServerName + ";Database=" + DataSource + ";User ID=" + User + ";Password=" + Password + " ;Encrypt=" + ConnectionEncrypt + ";TrustServerCertificate=" + Trusted + ";Connection Timeout=" + Timeout;
            else
                ConStr = "Asynchronous Processing=true; Persist Security Info=" + PersistSecurity + " ;Pooling=" + booling + " ;password= " + Password + ";User ID=" + User + ";Initial Catalog = " + DataSource + ";Trusted_Connection=" + Trusted + ";data source = " + ServerName + ";Connect Timeout=" + Timeout;
            */
        }
		
		/// <summary>
		/// The procedure for executing queries 
		/// </summary>
		/// <param name="Query">The query to be executed</param>
		/// <returns></returns>
		private static int ExecuteQuery(string Query)
		{
			int RowsAffecred = 0;
			using(Connection = new SqlConnection(ConStr))
			{
				Connection.Open();
				Cmd = new SqlCommand("", Connection);
				Cmd.CommandText = Query;
				try
				{
					RowsAffecred = Cmd.ExecuteNonQuery();
				}
				catch(Exception ex)
				{
                    Console.Write("Error: "+Query + " - " + ex.Message, "Error" );
				}
				Connection.Close();
				Cmd.Dispose();
			}
			return RowsAffecred;
		}


        /// <summary>
        /// The procedure for executing queries 
        /// </summary>
        /// <param name="Query">The query to be executed</param>
        /// <returns></returns>
        private static int ExecuteQuery(string Query, Field[] Parameters)
        {
            int RowsAffecred = 0;
            using (Connection = new SqlConnection(ConStr))
            {
                Connection.Open();
                Cmd = new SqlCommand("", Connection);
                Cmd.CommandText = Query;

                if (Parameters != null)
                {
                    IEnumerator Item = Parameters.GetEnumerator();
                    while (Item.MoveNext())
                    {
                        if (((Field)Item.Current).FieldType == DataType.NumericField)
                        {
                            Cmd.Parameters.Add(((Field)Item.Current).Name, SqlDbType.Decimal);
                            Cmd.Parameters[((Field)Item.Current).Name].Value = ((Field)Item.Current).Value;
                        }
                        else
                        {
                            SqlParameter Param = new SqlParameter();
                            Param.ParameterName = ((Field)Item.Current).Name;
                            Param.Value = ((Field)Item.Current).Value;
                            Cmd.Parameters.Add(Param);
                        }
                    }
                }

                try
                {
                    RowsAffecred = Cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.Write("Error: " + Query + " - " + ex.Message, "Error" );
                }
                Connection.Close();
                Cmd.Dispose();
            }
            return RowsAffecred;
        }


        public static object ExecuteScalar(string Query)
        {
            object RowsAffecred = new object();
            using (Connection = new SqlConnection(ConStr))
            {
                Connection.Open();
                Cmd = new SqlCommand("", Connection);
                Cmd.CommandText = Query;
                try
                {
                    RowsAffecred = Cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.Write("Error: " + Query + " - " + ex.Message, "Error" );
                }
                Connection.Close();
                Cmd.Dispose();
            }
            return RowsAffecred;
        }

        public static object ExecuteScalar(string Query, Field[] Parameters)
        {
            object RowsAffecred = new object();
            using (Connection = new SqlConnection(ConStr))
            {
                Connection.Open();
                Cmd = new SqlCommand("", Connection);
                Cmd.CommandText = Query;

                if (Parameters != null)
                {
                    IEnumerator Item = Parameters.GetEnumerator();
                    while (Item.MoveNext())
                    {
                        SqlParameter Param = new SqlParameter();
                        Param.ParameterName = ((Field)Item.Current).Name;
                        Param.Value = ((Field)Item.Current).Value;
                        Cmd.Parameters.Add(Param);
                    }
                }

                try
                {
                    RowsAffecred = Cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.Write("Error: " + Query + " - " + ex.Message, "Error" );
                }
                Connection.Close();
                Cmd.Dispose();
            }
            return RowsAffecred;
        }

        public static void ExecuteSQLCommand(string Query)
        {
            Cmd = new SqlCommand(Query);
            try
            {
                Cmd.CommandText = Query;
            }
            catch (Exception ex)
            {
                Console.Write("Error: " + Query + " - " + ex.Message, "Error" );
            }
            Cmd.Dispose();
        }

		/// <summary>
		/// The procedure for retrieving data from the database 
		/// </summary>
		/// <param name="Query">The query to be executed</param>
		/// <returns></returns>
		public static DataTable GetData(string Query)
		{
            

            Dt = new DataTable();
            using (Connection = new SqlConnection(ConStr))
            {
                try
                {
                    Connection.Open();
                    Cmd = new SqlCommand(Query, Connection);

                    IAsyncResult itfAsynch;
                    itfAsynch = Cmd.BeginExecuteReader(CommandBehavior.CloseConnection);
                    SqlDataReader myDataReader = Cmd.EndExecuteReader(itfAsynch);
                    Dt.Load(myDataReader);
                    myDataReader.Close();

                }
                catch (Exception ex)
                {
                    Console.Write("Error: " +  ex.Message, "Error" );
                }
                finally
                {
                    Connection.Close();
                    Cmd.Dispose();
                }

            }
            return Dt;
		}

        public static DataTable GetDataTable(string Query)
        {
            Dt = new DataTable();
            using (Connection = new SqlConnection(ConStr))
            {
                Cmd = new SqlCommand("", Connection);
                Cmd.CommandText = Query;
                Adapter = new SqlDataAdapter(Cmd);
                try
                {
                    Adapter.Fill(Dt);
                }
                catch (Exception ex)
                {
                    Console.Write("Error: " + Query + " - " + ex.Message, "Error" );
                }
                Cmd.Dispose();
            }
            return Dt;
        }

        public static bool ISIDExist(string table, string col1, string col2, string key)
        {
            object RowsAffecred = new object();
            using (Connection = new SqlConnection(ConStr))
            {
                Connection.Open();
                Cmd = new SqlCommand("", Connection);
                Cmd.CommandText = "SELECT " + col1 + " FROM " + table + " WHERE " + col2 + " = " + key;
                try
                {
                    RowsAffecred = Cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message, "Error" );
                }
                Connection.Close();
                Cmd.Dispose();
            }
            if (RowsAffecred != DBNull.Value && RowsAffecred != null)
                return true;
            else
                return false;
        }

		/// <summary>
		/// The procedure for retrieving data from the database 
		/// </summary>
		/// <param name="Query">The query to be executed</param>
		/// <param name="Reader">The reader to read connectively from the database</param>
		public static void GetData(string Query, ref SqlDataReader Reader)
		{
			using(Connection = new SqlConnection(ConStr))
			{
				Cmd = new SqlCommand("", Connection);
				Cmd.CommandText = Query;
				try
				{
					Reader = Cmd.ExecuteReader(CommandBehavior.Default);
				}
				catch(Exception ex)
				{
                    Console.Write("Error: " + Query + " - " + ex.Message, "Error" );
				}
				Cmd.Dispose();
			}
		}

		/// <summary>
		/// The procedure for executing stored procedure for data retrieval or modification
		/// </summary>
		/// <param name="Query"></param>
		/// <param name="Parameters"></param>
		/// <returns></returns>
		public static DataTable ExecuteStoredProcedure(string StoredProcName, Field [] Parameters)
		{
			Dt = new DataTable();
			using(Connection = new SqlConnection(ConStr))
			{
				Cmd = new SqlCommand("", Connection);
				Cmd.CommandType = CommandType.StoredProcedure;
				Cmd.CommandText = StoredProcName;
				if(Parameters != null)
				{
					IEnumerator Item = Parameters.GetEnumerator();
					while(Item.MoveNext())
					{
						SqlParameter Param = new SqlParameter();
						Param.ParameterName = ((Field)Item.Current).Name;
						Param.Value = ((Field)Item.Current).Value;
						Cmd.Parameters.Add(Param);
					}
				}
				Adapter = new SqlDataAdapter(Cmd);
				try
				{
					Adapter.Fill(Dt);
				}
				catch(Exception ex)
				{
					Console.Write(ex.Message, "Error" );
				}
				Cmd.Dispose();
			}
			return Dt;
		}

        public static DataTable GetSqlStatment_GLACCountCode(string SW, string SMotherCode, string SCode, string SName, string _strTableName)
        {
            string sqlStatment = "";
            Field[] field = new Field[0];

            if (SW == "First")
                sqlStatment = "SELECT     Code, Name	FROM         " + _strTableName + "	WHERE     (IsDeleted = 0) AND (LevelNo = 1)	ORDER BY LevelNo,Code";

            else if (SW == "Second")
                sqlStatment = "SELECT     Code, Name	FROM         " + _strTableName + "	WHERE     (IsDeleted = 0) AND (MotherCode = '" + SMotherCode + "')	order by Code";

            else if (SW == "LastLevel")
                sqlStatment = "SELECT     LastLevel    FROM         " + _strTableName + "    WHERE     (Code = '" + SCode + "')    ";

            else if (SW == "IsRepeated")
            {
                //sqlStatment = "SELECT     Code    FROM         " + _strTableName + "    WHERE     (Name = @SNAME) AND (Code <> '" + SCode + "') AND (IsDeleted = 0)";
                sqlStatment = "SELECT     Code    FROM         " + _strTableName + "    WHERE     (Name = '" + SName + "') AND (Code <> '" + SCode + "') AND (IsDeleted = 0)";
                field = new Field[1];
                field[0].Name = "@SNAME";
                field[0].Value = SName;
                field[0].FieldType = DataType.StringField;
            }
            else if (SW == "MAX")
                sqlStatment = "SELECT     ISNULL(MAX(Code), 0)     FROM         " + _strTableName + "    WHERE     (MotherCode = '" + SMotherCode + "')";

            else if (SW == "Prefix")
                sqlStatment = "SELECT     MAX(Code) AS Expr1    FROM         " + _strTableName + "    WHERE     (Code = '" + SMotherCode + "')    ";

            else if (SW == "RootCode")
                sqlStatment = "SELECT     RootCode, Code    FROM         " + _strTableName + "    WHERE     (Code = '" + SCode + "')    ";

            else if (SW == "IsMainAcc")
                sqlStatment = "SELECT     MotherCode, Code    FROM         " + _strTableName + "    WHERE     (Code = '" + SCode + "')    ";

            else if (SW == "LevelNo")
                sqlStatment = "SELECT     LevelNo    FROM         " + _strTableName + "    WHERE     (Code = '" + SCode + "')    ";

            else if (SW == "LoadData")
                sqlStatment = " SELECT     *	 FROM         " + _strTableName + "	 WHERE     (Code = '" + SCode + "')	 ";

            else if (SW == "MotherName")
                sqlStatment = "	 SELECT     Name	 FROM         " + _strTableName + "	 WHERE     (Code = '" + SMotherCode + "') AND (IsDeleted = 0)	 ";

            else if (SW == "IsMother")
                sqlStatment = "	 SELECT     COUNT(Code) AS Expr1	 FROM         " + _strTableName + "	 WHERE     (MotherCode = '" + SCode + "') AND (IsDeleted = 0)	 ";

            else if (SW == "HaveEntries")
                sqlStatment = "	 SELECT     COUNT(Code) AS Expr1	 FROM         GL_EntryDetails	 WHERE     (AccCode = '" + SCode + "')    ";

            else if (SW == "IsExist")
                sqlStatment = "  SELECT     COUNT(Code) AS Expr1  FROM         " + _strTableName + "  WHERE     (Code = '" + SCode + "')   ";

            else if (SW == "GetAccCode")
            {
                //sqlStatment = "   SELECT     Code   FROM         " + _strTableName + "   WHERE     ([Name] = @SNAME)   ";
                sqlStatment = "   SELECT     Code   FROM         " + _strTableName + "   WHERE     ([Name] = '" + SName + "')   ";
                field = new Field[1];
                field[0].Name = "@SNAME";
                field[0].Value = SName;
                field[0].FieldType = DataType.StringField;
            }
            return GetData(sqlStatment);
        }



        public static string GetMaxChildGLAccountCode(string MotherCode, string _strTableName)
        {
            int[] arrLevelsDigitsNum;
            string ChildGLAccountCode = "";
            int strLevelNo = 0;

            DataTable tbl = SQLDBConnection.GetData("SELECT LevelNo, CharNo FROM  GLB_SettingLevels WHERE (isdefult = 1)");
            arrLevelsDigitsNum = new int[tbl.Rows.Count];
            for (int i = 0; i < tbl.Rows.Count; i++)
                arrLevelsDigitsNum[i] = Convert.ToInt32(tbl.Rows[i][1].ToString());

            DataTable tblLevel = SQLDBConnection.GetSqlStatment_GLACCountCode("LevelNo", "", MotherCode, "", _strTableName);
            if (tblLevel.Rows.Count > 0)
                strLevelNo = Convert.ToInt32(tblLevel.Rows[0][0]) + 1;

            try
            {
                DataTable tblMax = SQLDBConnection.GetSqlStatment_GLACCountCode("MAX", MotherCode, "", "", _strTableName);
                string AccCodeName = tblMax.Rows[0][0].ToString();
                if (AccCodeName != "0")
                {
                    DataTable tblMax1 = SQLDBConnection.GetSqlStatment_GLACCountCode("Prefix", MotherCode, "", "", _strTableName);
                    string Prefix = tblMax1.Rows[0][0].ToString();
                    AccCodeName = AccCodeName.Remove(0, Prefix.Length);

                }
                AccCodeName = (Convert.ToInt32(AccCodeName) + 1).ToString();
                while (AccCodeName.Length < arrLevelsDigitsNum[strLevelNo - 1])
                    AccCodeName = "0" + AccCodeName;
                if (MotherCode == "0")
                    ChildGLAccountCode = AccCodeName;
                else
                    ChildGLAccountCode = MotherCode + AccCodeName;

            }
            catch
            { }
            return ChildGLAccountCode;

        }
        public static void ConString()
        {
            if (ConnectionType == "Cloud")
                ConStr = "Asynchronous Processing=true; Server=" + ServerName + ";Database=" + DataSource + ";User ID=" + User + ";Password=" + Password + " ;Encrypt=" + ConnectionEncrypt + ";TrustServerCertificate=" + Trusted + ";Connection Timeout=" + Timeout;
            else
                ConStr = "Asynchronous Processing=true; Persist Security Info=" + PersistSecurity + " ;Pooling=" + booling + " ;password= " + Password + ";User ID=" + User + ";Initial Catalog = " + DataSource + ";Trusted_Connection=" + Trusted + ";data source = " + ServerName + ";Connect Timeout=" + Timeout;
        }

        #endregion
    }
}