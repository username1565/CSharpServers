using System;
using System.IO;			//MemoryStream to get bytes of blob from reader.
using System.Data;			//DataSet and DataTable
using Mono.Data.Sqlite;

/*
PISDA - Provide Instant SQLite Data Access.

	This program allow to do the following things:
		1. Set DBFileName, or use already defined value.
		2. Open database and set connection to this, and keep this connection opened.
		3. Execute SQL-requests for already opened SQLite3 database.
		4. Run different methods with different SQL-requests, for this opened SQLite3 database.
		5. Test all that methods, in Main()
		
	This program can working with System.Data.SQLite.dll, and with any SQLite3 database.
	This program can open any specified SQLite3 database, and execute SQL-requests.
	
	To build it, use:
set fdir=%WINDIR%\Microsoft.NET\Framework
set csc=%fdir%\v4.0.30319\csc.exe
set msbuild=%fdir%\v4.0.30319\msbuild.exe
%csc% /out:PISDA.exe /reference:System.Data.SQLite.dll PISDA.cs
PISDA.exe
pause
*/

/*
//		Use System.Data.SQLite.dll in C#:

private static string DBFileName = @"test.db";																//set database file
private static string ConnectionString = @"Data Source="+DBFileName+";Version=3;New=False;Compress=True";	//string to connect this db-file
private static Mono.Data.Sqlite.SqliteConnection Connection = new Mono.Data.Sqlite.SqliteConnection(ConnectionString);						//database-connection
private statis void OpenConnectionIfNeed(){																	//open this if it's closed
	//Console.WriteLine("Connection.State: "+Connection.State);		//show connection-stata open/close
	if (Connection.State != System.Data.ConnectionState.Open) {		//If db-connection closed
		Connection.Open();												//open this connection
	}
	Console.WriteLine("Connection.State: "+Connection.State);		//show connection-stata open/close
}

private static object db_lock = new object();						//Object to lock database, when some query executed.

//Then, in the code, there is possible to do the following things:
string sql = "SQL-request";
lock (db_lock)											//lock database to do some actions
{
	//	Actions:

	OpenConnectionIfNeed();								//open connection if need
	//DataTable schema = Connection.GetSchema(arg);		//Get database-schema
	
	//Create new Mono.Data.Sqlite.SqliteCommand for the current, opened database-connection, and add sql-query there.
	using(Mono.Data.Sqlite.SqliteCommand cmd = new Mono.Data.Sqlite.SqliteCommand(sql, Connection)){
		//Inside this using, there can executed the different methods:
		//sql...
		//cmd.Parameters.Add(...);	//For writting the bytes, as blog, for example
		//cmd.ExecuteNonQuery();	//execute sql, and return int, without result
		//cmd.ExecuteReader();		//return data
		//cmd.ExecuteScalar();		//return object with different types
		//etc...
		//Connection.Close();		//In the end of execution, there is possible to close database-connection
		//return result;			//and return result.
	}
}	//unlock database
//All this can be also, a public static methods, that is accepting a "string sql".
*/			


namespace PISDA
{
	class PISDA
	{
		//Database FileName string
		private static string _DBFileName = null;  				//Filename of database to connect it
		//get or set value of this
		public static string DBFileName
		{
			get{
				return _DBFileName;
			}
			set
			{
				_DBFileName = value;
				if (!File.Exists(DBFileName)){						//if DB-file not exists
					Mono.Data.Sqlite.SqliteConnection.CreateFile(DBFileName);			//create this file with database
				}
				_ConnectionString = ConnectionString;					//update connection string, with this file.
				_Connection = new Mono.Data.Sqlite.SqliteConnection(_ConnectionString);	//and open Connection with this string.
				//...
				if(new System.IO.FileInfo(DBFileName).Length == 0){
					string sqlquery = @File.ReadAllText(DBFileName+".sql");
					ExecuteSQL(sqlquery);					//execute SQL-query to create DataBase
				}
			}
		}
		// To change this, use:
		//	get:	string file = PISDA.DBFileName;
		//	set:	PISDA.DBFileName = "filename.db3";

		//Connection strins
		private static string _ConnectionString = null;				//string to connect database
		//get or set value of this
		public static string ConnectionString
		{
			get{
				//	Update connection string, according new DB-file, and return this.

				//_ConnectionString = @"URI=file:"+DBFileName;
				//_ConnectionString = @"Data Source=" + DBFileName + ";Version=3;";
				_ConnectionString = @"Data Source="+DBFileName+";Version=3;New=False;Compress=True";	//string to connect database

				return _ConnectionString;
			}
			set
			{
				//or set custom value for this:
				_ConnectionString = value;
				//...
			}
		}
		// To change this, use:
		//	get:	string str = PISDA.ConnectionString;
		//	set:	PISDA.ConnectionString = "value";

		//Connection to database
		private static Mono.Data.Sqlite.SqliteConnection _Connection = null;		//Mono.Data.Sqlite.SqliteConnection to database.
		//When this defined, then:
		//	open - _Connection.Open(); or OpenConnectionIfNeed();
		//	close - _Connection.Close(); or CloseConnectionIfNeed(); (+ CloseConnections = true/false);
		//	Do not do _Connection.Open(); when this opened, else throw exception.
		
		//get or set value of this
		public static Mono.Data.Sqlite.SqliteConnection Connection
		{
			get																	//get Mono.Data.Sqlite.SqliteConnection to the database
			{
				if (_Connection == null)											//if connection not defined
				{
					_Connection = new Mono.Data.Sqlite.SqliteConnection(ConnectionString);				//generate connection string, and connect to database with this
					_Connection.Open();													//open database, after connect to this.
				}
				else if (_Connection.State != System.Data.ConnectionState.Open)		//if connection already defined
				{
					_Connection.Open();													//Just open this
				}
				else																//else if already opened
				{}
				
				return _Connection;													//and return an opened Mono.Data.Sqlite.SqliteConnection to this database.
				
			}
			set																	//or set this connection from ConnectionString (value)
			{
				_Connection = new Mono.Data.Sqlite.SqliteConnection(value);							//set connection with value
				_Connection.Open();													//open this
			}
		}
		//	get: _cmd = new Mono.Data.Sqlite.SqliteCommand(Connection);	//connection will be opened, and _cmd will be binded.
		//	set: Connection = new Mono.Data.Sqlite.SqliteConnection(_ConnectionString);	//create new connection from _ConnectionString
		
		private static bool CloseConnections = false;	//true, by default, and connection will be closed after each operation.
		//Or false, if need to keep connection: PISDA.CloseConnections = false;

		private static void CloseConnectionIfNeed(){
			if(CloseConnections == true){
				_Connection.Close();
				return;
			}
		}

		private static void OpenConnectionIfNeed(){
			_Connection = Connection;	//it will be opened there, if it's closed.
		}

		private static Mono.Data.Sqlite.SqliteCommand _cmd = null;				//Mono.Data.Sqlite.SqliteCommand for opened database after connection with this.
																//		cmd = new Mono.Data.Sqlite.SqliteCommand(sql, Connection) or cmd = new Mono.Data.Sqlite.SqliteCommand(Connection)
		//get or set value of this
		public static Mono.Data.Sqlite.SqliteCommand cmd
		{
			get{
				if(
						_cmd == null											//if _cmd not defined
					||	_Connection == null										//or if connection not defined
					||	_Connection.State != System.Data.ConnectionState.Open	//or if connection closed
				){
					_cmd = new Mono.Data.Sqlite.SqliteCommand(Connection);					//Create and open database connection, and create new Mono.Data.Sqlite.SqliteCommand for this.
				}
				return _cmd;												//or/and return that Mono.Data.Sqlite.SqliteCommand _cmd
			}
			set{
				_cmd = value;	//create new Mono.Data.Sqlite.SqliteCommand with sql
			}
		}
		//	get: _cmd = PISDA.cmd; //cmd will be created, if not exists
		//	set: cmd = new Mono.Data.Sqlite.SqliteCommand(sql, Connection); //create new and set
		

		//object to lock database, while some command executed for this.
		//Another commands will wait the finish of this.
		private static object db_lock = new object();

		//Now, begin SQL-methods:
		public static DataSet GetDataSet(
			string sql
		,	bool show = false
		)
		{
			DataSet dataSet = new DataSet();

			lock (db_lock)
			{
				using(cmd = new Mono.Data.Sqlite.SqliteCommand(sql, Connection)){	//create new Mono.Data.Sqlite.SqliteCommand for current connection, after open this, and add sql-query there.
					Mono.Data.Sqlite.SqliteDataAdapter adp = new Mono.Data.Sqlite.SqliteDataAdapter(_cmd);
					adp.Fill(dataSet);
					CloseConnectionIfNeed();
				}
			}

			if(show == true){
				Console.WriteLine("dataSet: "+dataSet+", dataSet.Tables.Count: "+dataSet.Tables.Count);

				foreach (DataTable table in dataSet.Tables)
				{
					foreach (DataRow row in table.Rows)
					{
						foreach (DataColumn column in table.Columns)
						{
							object item = row[column];
							// read column and item
							Console.Write("{0}, ", item );
						}
						Console.Write("\n");
					}
					Console.Write("\n\n");
				}
			}

			return dataSet;
		}

		public static DataTable GetDataTable(
			string sql
		,	bool show = false
		)
		{
			lock (db_lock)
			{
				DataSet dataSet = GetDataSet(sql);
				if (dataSet.Tables.Count > 0){
					if(show == true){
						//Console.WriteLine("dataSet.Tables[0]: "+dataSet.Tables[0]+", dataSet.Tables[0].Rows.Count: "+dataSet.Tables[0].Rows.Count);
						//Console.WriteLine(sql);
						foreach (DataTable table in dataSet.Tables)
						{
							foreach (DataRow row in table.Rows)
							{
								foreach (DataColumn column in table.Columns)
								{
									object item = row[column];
									// read column and item
									Console.Write("{0}, ", item );
								}
								Console.Write("\n");
							}
							Console.Write("\n\n");
						}
					}
					return dataSet.Tables[0];
				}
				return null;
			}
		}
		
		public static void ShowDataTable(
			DataTable table
		){
			foreach (DataRow row in table.Rows)
			{
				foreach (DataColumn column in table.Columns)
				{
					object item = row[column];
					// read column and item
					Console.Write("{0}, ", item );
				}
				Console.Write("\n");
			}
			return;
		}

		//SQL-query without return data.
		public static int ExecuteSQL(
			string sql
		,	bool show = false
		)
		{
			lock (db_lock)
			{
				using(cmd = new Mono.Data.Sqlite.SqliteCommand(sql, Connection)){	//create new Mono.Data.Sqlite.SqliteCommand for current connection, after open this, and add sql-query there.
					int result = _cmd.ExecuteNonQuery();
					CloseConnectionIfNeed();
					
					if(show==true){
						Console.WriteLine("result: "+result);
					}
					return result;
				}
			}
		}
		
		public static void TransactionCommit(
			string [] sqls
		){
			lock (db_lock)
			{
				using (var transaction = Connection.BeginTransaction())
				{
					for (int i = 0; i < sqls.Length; i++)
					{
						using (cmd = new Mono.Data.Sqlite.SqliteCommand(sqls[i], _Connection))
						{
							_cmd.ExecuteNonQuery();
						}
					}
					transaction.Commit();
					CloseConnectionIfNeed();
					return;
				}
			}
		}

//	Usage:
//		string[] sqls = new string[0];
//		for(int j = 0; j<1000; j++){
//			Array.Resize(ref sqls, sqls.Length + 1);
//			sqls[sqls.Length - 1] = "insert into Cars (CarName, Price) values ('Audi', "+j+")";
//		}
//		TransactionCommit(sqls);

//	Or as one SQL-query, using NonQuery():
//BEGIN TRANSACTION;
//	UPDATE builtuparea_luxbel SET ADMIN_LEVEL = 6 where PK_UID = 2;
//	UPDATE builtuparea_luxbel SET ADMIN_LEVEL = 6 where PK_UID = 3;
//COMMIT TRANSACTION; 

		//Read TEXT
		public static Mono.Data.Sqlite.SqliteDataReader read(
			string sql
		,	bool show = false
		)
		{
			lock(db_lock){
				using(cmd = new Mono.Data.Sqlite.SqliteCommand(sql, Connection)){	//create new Mono.Data.Sqlite.SqliteCommand for current connection, after open this, and add sql-query there.
					Mono.Data.Sqlite.SqliteDataReader result = _cmd.ExecuteReader();
					
					while (result.Read()){
						if(show == true){
							for(var i = 0; i<result.FieldCount; i++){
								Console.Write(i+":{0}; ",result[i]);
							}
							Console.Write("\n");
						}
					}
					CloseConnectionIfNeed();
					
					return result;
				}
			}
		}
	

		//Write text, or BLOB
		public static int NonQuery(
			string sql
		,	byte[] blob = null
		,	bool show = false
		)
		{
		//	Console.WriteLine("sql"+sql);
			lock(db_lock){
				using(cmd = new Mono.Data.Sqlite.SqliteCommand(sql, Connection)){	//create new Mono.Data.Sqlite.SqliteCommand for current connection, after open this, and add sql-query there.
					if(blob != null){
						cmd.Parameters.Add("@blob", System.Data.DbType.Binary, 20).Value = blob;		
					}
					int result = cmd.ExecuteNonQuery();
					CloseConnectionIfNeed();
					return result;
				}
			}
		}

		//Write one BLOB
		public static int WriteBlob(
			string TableName
		,	string columnName
		,	byte[] blob
		)
		{
			lock(db_lock){
				string sql = "INSERT OR REPLACE INTO "+TableName+" ("+columnName+") VALUES (@"+columnName+")";			
				using(cmd = new Mono.Data.Sqlite.SqliteCommand(sql, Connection)){	//create new Mono.Data.Sqlite.SqliteCommand for current connection, after open this, and add sql-query there.
					_cmd.Parameters.Add("@"+columnName, System.Data.DbType.Binary, 20).Value = blob;
					int result = _cmd.ExecuteNonQuery();
					CloseConnectionIfNeed();
					return result;
				}
			}
		}
	
	
		//read BLOB
		public static byte[] readBlob(
			string sql
		,	bool show = false
		)
		{
			lock(db_lock){
				using(cmd = new Mono.Data.Sqlite.SqliteCommand(sql, Connection)){	//create new Mono.Data.Sqlite.SqliteCommand for current connection, after open this, and add sql-query there.
					byte[] buffer = new byte[0];
					using (var reader = _cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							buffer = GetBytes(reader);
						}
						CloseConnectionIfNeed();
					}
		
					if(show == true){
						Console.WriteLine("blob bytes:");
						for(int i = 0; i<buffer.Length; i++){
							Console.WriteLine(buffer[i]);
						}
						Console.WriteLine("buffer.Length: "+buffer.Length);
					}
					return buffer;
				}
			}
		}
	
		//get bytes from reader
		static byte[] GetBytes(
			Mono.Data.Sqlite.SqliteDataReader reader
		)
		{
			if(reader.IsDBNull(0)){return null;}
			const int CHUNK_SIZE = 2 * 1024;
			byte[] buffer = new byte[CHUNK_SIZE];
			long bytesRead;
			long fieldOffset = 0;
			using (MemoryStream stream = new MemoryStream())
			{
				while ((bytesRead = reader.GetBytes(0, fieldOffset, buffer, 0, buffer.Length)) > 0)
				{
					stream.Write(buffer, 0, (int)bytesRead);
					fieldOffset += bytesRead;
				}
				return stream.ToArray();
			}
		}
	
		public static DataTable ReturnDataTable(
			string sql
		,	bool show = false
		)
		{
			lock(db_lock){
				using(cmd = new Mono.Data.Sqlite.SqliteCommand(sql, Connection)){	//create new Mono.Data.Sqlite.SqliteCommand for current connection, after open this, and add sql-query there.
					Mono.Data.Sqlite.SqliteDataReader dataReader = cmd.ExecuteReader();
					DataTable dTable = new DataTable();
					dTable.Load(dataReader);
					CloseConnectionIfNeed();

					if(show == true){
						if (dTable.Rows.Count > 0)
						{
							for (int i = 0; i < dTable.Rows.Count; i++){
								foreach(object cell in dTable.Rows[i].ItemArray) {
									Console.Write(cell+"\t");
									//((cell.GetType()).Equals(typeof(byte[])))		//is blob? (byte[])
								}
								Console.Write("\n");
							}
						}
						else{
							Console.WriteLine("Table is is empty");
						}
					}
					return dTable;
				}
			}
		}
		
		public static DataRow ReturnDataRow(
			string sql
		,	bool show = false
		){
			DataTable result = GetDataTable(sql);
			if(result.Rows.Count == 0){return null;}
			DataRow row = result.Rows[0];
			if(show == true){
								foreach(object cell in row.ItemArray) {
									Console.Write(cell+"\t");
									//((cell.GetType()).Equals(typeof(byte[])))		//is blob? (byte[])
								}
								Console.Write("\n");			
			}
			return row;
		}		
		
		
		public static DataTable GetSchema(
			string arg
			/*
					Arg:
				MetaDataCollections
				DataSourceInformation
				Catalogs
				Columns
				ForeignKeys
				Indexes
				IndexColumns
				Tables
				Views
				ViewColumns
			*/
		){
			lock(db_lock){
				DataTable schema = Connection.GetSchema(arg);
				CloseConnectionIfNeed();
			
				for (int i = 0; i < schema.Rows.Count; i++){
					foreach(object cell in schema.Rows[i].ItemArray) {
						Console.Write(cell+"\t");
					}
					Console.Write("\n");
				}
				return schema;
			}
		}
		
		
		public static void ConnectTablesByForeignKey(){
			/*
				// It's SQL-request:
				//First, create table without the parent_id:
				//
				//	CREATE TABLE child(
				//		id INTEGER PRIMARY KEY,
				//		description TEXT
				//	);
				//	
				//Then, alter table:
				//	ALTER TABLE child ADD COLUMN parent_id INTEGER REFERENCES parent(id);
			*/

			//REINDEX name
			
			CloseConnectionIfNeed();
			Console.WriteLine("ConnectTablesByForeignKey: See comments in the source code.");
			return;
		}

		
		//Add in the table, the row with different types.
		public static int AddRow(
			string TableName
		,	object[] values = null	//object[] values = new object[]{null, "Text", 123, null, 1234, 123, 123, new byte[] { 1, 2, 3, 4, 5 } };
									//array with objects, with different types (NULL, TEXT, NUMERIC, INTEGER, REAL, BLOB).
									//First value must to be "null" for - auto-incremented column.
		)
		{
			if(values != null){
				lock(db_lock){
					//Build SQL-request, to add this values in the cells of the row
					string sql =
							"INSERT INTO "+TableName
						+	" VALUES("	 //?,?,?,...,?);
					;
					//Add this ? and , and bracket ")"
				
					int i = values.Length;				//set this as 0
					for(; i > 0; i-- ){
								sql +=
								"?" +
									( ( i != 1 )
										?	","
										:	""
									);
					}
					sql +=");"; //end of set SQL-command

					using(cmd = new Mono.Data.Sqlite.SqliteCommand(sql, Connection)){	//create new Mono.Data.Sqlite.SqliteCommand for current connection, after open this, and add sql-query there.
						//Add values, as parameters:
						if(values != null){		//if object with values with dynamic types is specified
							for(;i<values.Length; i++){		//for each item
								cmd.Parameters.AddWithValue("param"+(i+1), values[i]);	//add this as value with custom type to parameters.
							}
						}
						int result = cmd.ExecuteNonQuery();		//and execute this (with added parameters)
						CloseConnectionIfNeed();
						return result;
					}
				}
			}
			else{		//else nothing to insert.
				Console.WriteLine("AddRow-method: object[] values = null . Nothing to insert.");
				return 0;
			}
		}
	
	
	
		//Retrieve single items wity dynamic type, from db (this is a 2-nd rule from Codd's 12 rules):
		//	SELECT ColumnName FROM TableName WHERE UniqueRowID=RowValue
		public static object ExecuteScalar(
			string sql
		,	bool show = false
		)
		{
			try{
				lock(db_lock){
					using(cmd = new Mono.Data.Sqlite.SqliteCommand(sql, Connection)){	//create new Mono.Data.Sqlite.SqliteCommand for current connection, after open this, and add sql-query there.
						object value = _cmd.ExecuteScalar();
						CloseConnectionIfNeed();
						if (value != null)
						{
							//return value.ToString();	//to return all as text.
							if(show == true)
							{
								Console.WriteLine("value: "+value+", type: "+(value.GetType())+", Is blob? "+((value.GetType()).Equals(typeof(byte[]))) );		//is blob? (byte[])
							}
							return value;	//return as an object with dynamic type
						}
						//return string.Empty;	//else empty-string null
						return DBNull.Value;			//or null.
					}
				}
			}
			catch(Exception ex){
				Console.WriteLine("PISDA.PISDA.ExecuteScalar: sql: "+sql+", ex: "+ex);
				return DBNull.Value;
			}
		}

		//Execute SQL, and return results, as DataTable.
		public static DataTable ExecuteWithResults(
			string sql
		,	bool show = false
		){
			return GetDataTable(sql, show);
		}		

		//Log to logfile
		public static void Log(
				string str				//string to log
			,	bool newline = true		//append new line? true/false
		){
			str = (newline == true) ? str + "\n" : str;
			Console.Write(str);							//show str
			File.AppendAllText("test.log", str);		//log str
		}
		
		public static void LogBytes(byte[] bytes)
		{
			Log("Bytes:");
			//show bytes
			for(int i = 0; i<bytes.Length; i++)
			{
				Log(bytes[i]+"\t", false);
				if( ( i > 0 ) && ( ( i % 16 ) == 0 ) )
				{
					Log("\n", false);
				}
			}
		}

		public static void LogDataRow(DataRow row){
			foreach(object cell in row.ItemArray) {
				if(
					((cell.GetType()).Equals(typeof(byte[])))		//is blob? (byte[])
				)
				{
					byte[] bytes = (byte[]) cell;	//get bytes from cell, with blob-object
					LogBytes(bytes);
				}
				else{
					Log(cell+"\t");
				}
			}
			Log("\n");			
		}

		public static void LogDataTable(DataTable table){
			Log("Table: "+table.TableName);
			foreach (DataRow row in table.Rows)
			{
				LogDataRow(row);
			/*
				foreach (DataColumn column in table.Columns)
				{
					object item = row[column];
					// read column and item
					Log(item.ToString()+", ");
				}
				Log("\n");
			*/
			}
			Log("\n\n");
		}
		
		public static void LogDataSet(DataSet dataset){
			foreach (DataTable table in dataset.Tables)
			{
				LogDataTable(table);
			}
			Log("\n\n");
		}

		public static int timeout = 500;	//timeout to run tests.
		
		public static void Tests(string dbfile){
			//		TESTS

			//================================================================================================================
			Log("set \""+dbfile+"\" filename of DB, to let program regenerate the connection string.");
			PISDA.DBFileName = dbfile;	//set filename of DB, to let program regenerate the connection string.
			Log("\""+dbfile+"\" was been set");
			Log("");
			System.Threading.Thread.Sleep(timeout);
			//================================================================================================================

			//1. Open DB or create it and open.
			//2. CRUD test
			//3. Tests of methods above

			//================================================================================================================
			File.WriteAllText("test.log", String.Empty);	//clear log-file
			Log("Run tests of PISDA...");
			Log("");
			System.Threading.Thread.Sleep(timeout);
			//================================================================================================================

			//================================================================================================================
			Log("DROP TABLE IF EXISTS [tbl_userinfo]");
			NonQuery("DROP TABLE IF EXISTS [tbl_userinfo]");
			Log("query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);
			//================================================================================================================

			//================================================================================================================
			Log("DROP VIEW IF EXISTS [vw_userinfo_filter]");
			NonQuery("DROP VIEW IF EXISTS [vw_userinfo_filter]");
			Log("query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);
			//================================================================================================================

			//================================================================================================================
			Log("test transaction commit");
			
		string sqlquery = @"
BEGIN TRANSACTION;
	CREATE TABLE [tbl_userinfo](
		[id] INTEGER PRIMARY KEY AUTOINCREMENT,
		[name] TEXT,
		[age] INTEGER,
		[sex] TEXT,
		[contact] TEXT,
		[status] INTEGER
	);
	CREATE VIEW [vw_userinfo_filter] as
		SELECT *
		FROM [tbl_userinfo]
		WHERE  [sex] = 'Female'
	;
	INSERT INTO [tbl_userinfo]
	([name], [age], [sex], [contact], [status])
	VALUES
		('James', 25, 'Male', '5124537326', 1)
	,	('Nueoo', 24, 'Male', '313121', 1)
	,	('Julia', 23, 'Female', '131', 1)
	,	('Oliver', 23, 'Female', '343434334', 1)
	,	('Holia', 40, 'Female', '83623', 1)
	;
COMMIT TRANSACTION; 
"
;
			NonQuery(sqlquery);
			Log("query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);
			//================================================================================================================

			//================================================================================================================
			Log("Extract DataSet");
			DataSet dataset = PISDA.GetDataSet("SELECT * FROM tbl_userinfo");
			LogDataSet(dataset);
			Log("");
			System.Threading.Thread.Sleep(timeout);
			//================================================================================================================

			//================================================================================================================
			Log("Extract DataTable");
			DataTable datatable = PISDA.GetDataTable("SELECT * FROM tbl_userinfo");
			LogDataTable(datatable);
			Log("");
			System.Threading.Thread.Sleep(timeout);
			//================================================================================================================

			//================================================================================================================
			Log("Insert records");
			int
			i = PISDA.ExecuteSQL("INSERT INTO tbl_userinfo (name, age, sex, contact, status) VALUES ('lol', 5, 'Male', 00000, 1)");
			Log(i.ToString());
			i = PISDA.ExecuteSQL("INSERT INTO tbl_userinfo (name, age, sex, contact, status) VALUES ('lola', 5, 'Female', 00000, 1)");
			Log(i.ToString());
			i = PISDA.ExecuteSQL("INSERT INTO tbl_userinfo (name, age, sex, contact, status) VALUES ('lol', 5, 'Male', 00000, 1)");
			Log(i.ToString());
			Log("");
			System.Threading.Thread.Sleep(timeout);
			//================================================================================================================

			//New file will be created, if this does not exists.
			//ConnectionString will be updated after set this.
			//Connection will be created and opened
			//SQLiteComman cmd can be binded to this opened connection, by creating new with this Connection.
			//	It can be closed at any time, by running CloseConnectionIfNeed()
			//	And it will be reopened, _Connection.Open(), after next request, if it's closed.
		
			Log("DROP TABLE IF EXISTS Cars");
			//delete table Cars if exists, and create this again.
			NonQuery("DROP TABLE IF EXISTS Cars");										//remove table Cars, if exists
			Log("query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Log(@"CREATE TABLE Cars( ID INTEGER PRIMARY KEY, CarName TEXT, Price INT )");
			NonQuery(@"CREATE TABLE Cars( ID INTEGER PRIMARY KEY, CarName TEXT, Price INT )");		//create table
			Log("query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Log("Insert records");
			//add rows in table:
			NonQuery("INSERT INTO Cars(CarName, Price) VALUES('Audi',52642)");
			NonQuery("INSERT INTO Cars(CarName, Price) VALUES('Mercedes',57127)");
			NonQuery("INSERT INTO Cars(CarName, Price) VALUES('Skoda',9000)");
			NonQuery("INSERT INTO Cars(CarName, Price) VALUES('Volvo',29000)");
			NonQuery("INSERT INTO Cars(CarName, Price) VALUES('Bentley',350000)");
			NonQuery("INSERT INTO Cars(CarName, Price) VALUES('Citroen',21000)");
			NonQuery("INSERT INTO Cars(CarName, Price) VALUES('Hummer',41400)");
			NonQuery("INSERT INTO Cars(CarName, Price) VALUES('Volkswagen',21600)");
			Log("Table cars created");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			
			Log("Table table and show records");
			//read table, and show records:
			try
			{
				Mono.Data.Sqlite.SqliteDataReader dr = read("SELECT * FROM cars");
			}
			catch (Mono.Data.Sqlite.SqliteException ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}
			Log("Table table and show records");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Log("CloseConnectionIfNeed()");
			CloseConnectionIfNeed();		
			Log("CloseConnectionIfNeed() executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);


			Log("Test binary data");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Log("DROP TABLE IF EXISTS PHOTOS");
			//Write and read bytes as BLOB
			NonQuery("DROP TABLE IF EXISTS PHOTOS");
			Log("Query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Log("CREATE TABLE PHOTOS(ID INTEGER PRIMARY KEY AUTOINCREMENT, PHOTO BLOB)");
			NonQuery("CREATE TABLE PHOTOS(ID INTEGER PRIMARY KEY AUTOINCREMENT, PHOTO BLOB)");
			Log("Query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Log("Write photo as blob");
			byte[] blob = new byte[] { 1, 2, 3, 4, 5 };
			int writeblob = WriteBlob("PHOTOS", "PHOTO", blob);								//write bytes as blob in specified column.
			Log("writeblob: "+writeblob.ToString());
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Log("Read photo as blob");
			byte[] readed = readBlob("SELECT PHOTO FROM PHOTOS WHERE ID = 1", true);		//read blob from this cell, show this, and return this.
			Log("Blob length: "+readed.Length.ToString());
			LogBytes(readed);
			Log("");
			System.Threading.Thread.Sleep(timeout);
	
	
			//write bytes as BLOB by this way
			Log("WriteBlob as NonQuery");
			NonQuery("INSERT INTO PHOTOS (PHOTO) VALUES (@blob)", new byte[] { 1, 2, 3, 4, 5 }, true);
			Log("Query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			//show bytes
			Log("Read photo and show bytes in console");
			byte[] readed2 = readBlob("SELECT PHOTO FROM PHOTOS WHERE ID = 2", true);
			LogBytes(readed2);
			Log("Query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			
			
			Log("Create table with all possible types");
			Log("");
			System.Threading.Thread.Sleep(timeout);
			
			//CreateTableWithAllTypes:
			Log("DROP TABLE IF EXISTS AllTypes");
			NonQuery("DROP TABLE IF EXISTS AllTypes");										//remove table AllTypes, if exists
			Log("Query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Log("Create table");
			NonQuery(
					@"CREATE TABLE AllTypes(
						ID INTEGER PRIMARY KEY,
						TextColumn TEXT,
						IntColumn INT,
						NullColumn NULL,
						NumericColumn NUMERIC,
						RealColumn REAL,
						IntegerColumn INTEGER,
						BlobColumn BLOB
					)"
			);//create table with all possible types.
			Log("Query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Log("Add row with all data-types");
			AddRow("AllTypes", new object[]{null, "Text", 123, null, 1234.8984, 123.1234, 123, new byte[] { 1, 2, 3, 4, 5 } });
			Log("Query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			//return table info (columns and types)
			Log("Show table-info");
			DataTable dTable = ReturnDataTable("PRAGMA table_info("+	"AllTypes"	+")");
			LogDataTable(dTable);
			Log("");
			System.Threading.Thread.Sleep(timeout);

			//create AllTypesView
			Log("Create view");
			NonQuery("DROP VIEW IF EXISTS ["+	"AllTypesView"	+"];");
			Log("Query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Log("Create view");
			NonQuery("CREATE VIEW AllTypesView as SELECT ID, RealColumn from AllTypes");//create table
			Log("Query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			//show AllTypesView, and return this as table
			Log("Show AllTypesView");
			DataTable dTable4 = ReturnDataTable("Select * from AllTypesView", true);
			LogDataTable(dTable4);
			Log("");
			System.Threading.Thread.Sleep(timeout);

			//show table AllTypes, and return this as table
			Log("show table AllTypes");
			DataTable dTable3 = ReturnDataTable("Select * from AllTypes WHERE ID = 1", true);
			LogDataTable(dTable3);
			Log("");
			System.Threading.Thread.Sleep(timeout);
			
			//show row from AllTypes, and return this as row
			Log("show row from AllTypes");
			DataRow row = ReturnDataRow("Select * from AllTypes WHERE ID = 1", true);
			LogDataRow(row);
			Log("");
			System.Threading.Thread.Sleep(timeout);
			
			//return one value with any type
			Log("extract one value with any type");
			object one_value = ExecuteScalar("select BlobColumn from AllTypes WHERE ID = 1", true);
			LogBytes((byte[])one_value);
			Log("");
			System.Threading.Thread.Sleep(timeout);
			
			//get tables
			Log("Get tables");
			DataTable tables = GetSchema("Tables");
			ShowDataTable(tables);
			Log("Query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			//get columns
			Log("Get columns");
			DataTable columns = GetSchema("Columns");
			ShowDataTable(columns);
			Log("Query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);
			//...

			
			

			Log("CloseConnectionIfNeed()");
			//The current connection to db can be closed, at any time.
			CloseConnectionIfNeed();
			Log("CloseConnectionIfNeed() executed.");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			//Show table
			Log("Show table");
			DataTable dTable2 = ReturnDataTable("SELECT * FROM Cars", true);
			Log(dTable2.ToString());
			Log("");
			System.Threading.Thread.Sleep(timeout);

			//ACID-transaction
			Log("ACID-transaction");
			NonQuery(
@"
BEGIN TRANSACTION;
	UPDATE Cars SET CarName = 'Audi' where ID = 1;
	UPDATE Cars SET CarName = 'Audi' where ID = 2;
	UPDATE Cars SET CarName = 'Audi' where ID = 3;
	UPDATE Cars SET CarName = 'Audi' where ID = 4;
COMMIT TRANSACTION;
"
			);
			Log("Query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);
			
			Log("ACID-transaction, using cycle");
			//ACID-transaction commit, by another way:
			string[] sqls = new string[0];
			for(int j = 0; j<1000; j++){
				Array.Resize(ref sqls, sqls.Length + 1);
				sqls[sqls.Length - 1] = "insert into Cars (CarName, Price) values ('Audi', "+j+")";	//add new string in string[] array
			}
			TransactionCommit(sqls);	//commit transaction with array of SQL-queries.
			Log("Query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);
			
			
			
			
			
			
			
			
			
			
			Log("Test CRUD - create, read, update, delete");
			Log("");
			System.Threading.Thread.Sleep(timeout);			

			Log("create record, with all types");
			AddRow("AllTypes", new object[]{null, "Text", 123, null, 1234.8984, 123.1234, 123, new byte[] { 1, 2, 3, 4, 5 } });
			Log("Query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);			
			

			Log("Read record with all types");
			datatable = PISDA.GetDataTable("SELECT * FROM AllTypes WHERE id=2", true);
			//Log(datatable);
			Log("");
			System.Threading.Thread.Sleep(timeout);	

			Log("Update record");
			NonQuery("UPDATE AllTypes SET TextColumn=\"Text2\" WHERE id=2");
			Log("Query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);	

			//return one value with any type
			Log("extract one updated value with any type");
			one_value = ExecuteScalar("select TextColumn from AllTypes WHERE ID = 2", true);
			Log(one_value.ToString());
			Log("");
			System.Threading.Thread.Sleep(timeout);
			
			Log("Delete record");
			NonQuery("DELETE FROM AllTypes WHERE id=2");
			Log("Query executed");
			Log("");
			System.Threading.Thread.Sleep(timeout);	

			try{
				//return one value with any type
				Log("Try read value from deleted row, again");
				one_value = ExecuteScalar("select TextColumn from AllTypes WHERE ID = 2", true);
				Log(one_value.ToString());
				Log("");
				System.Threading.Thread.Sleep(timeout);
			}
			catch (Exception ex){
				Log("Row was been deleted, Exception: "+ex.ToString());
			}
			
			Log("END CRUD-test");
			Log("");
			System.Threading.Thread.Sleep(timeout);
			
			
			Log("Random SQL");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Log("Type: SELECT * FROM AllTypes WHERE id=1");
			string sql = Console.ReadLine();
			datatable = PISDA.ExecuteWithResults(sql);
			LogDataTable(datatable);
			Log("");
			System.Threading.Thread.Sleep(timeout);	
			
			
			
			
			
			
			
			Log("Test open/close _Connection to DataBase.");
			//Test open/close _Connection to DataBase.
			Log("_Connection.State: "+_Connection.State);
			_Connection.Close();			
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Log("_Connection.State: "+_Connection.State);
			_Connection.Open();			
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Log("_Connection.State: "+_Connection.State);
			_Connection.Close();			
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Log("_Connection.State: "+_Connection.State);
			_Connection.Close();			
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Log("_Connection.State: "+_Connection.State);
			_Connection.Open();			
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Log("_Connection.State: "+_Connection.State + " - next call return throw exception");
			try{
				_Connection.Open();			//throw exception
			}
			catch(Exception ex){
				Log("Connection already open, so open again return throw Exception: "+ex.ToString());
			}

			PISDA.CloseConnections = true;
			CloseConnectionIfNeed();
			Log("_Connection.State: "+_Connection.State);
			Log("");
			System.Threading.Thread.Sleep(timeout);

			OpenConnectionIfNeed();
			Log("_Connection.State: "+_Connection.State);
			Log("");
			System.Threading.Thread.Sleep(timeout);

			PISDA.CloseConnections = false;
			CloseConnectionIfNeed();
			Log("_Connection.State: "+_Connection.State);
			OpenConnectionIfNeed();
			Log("_Connection.State: "+_Connection.State);
			Log("");
			System.Threading.Thread.Sleep(timeout);
			
		
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			//Show this in the end
			Log("Show this in the end, to do not close window");
			Log("Press any key to exit...");
			//Do not close the console-window.
			Log("");
			System.Threading.Thread.Sleep(timeout);
			
		}
		
		//Execute specified SQL-request:
		public static void ExecuteSQL(string dbfile, string sql)
		{
			//================================================================================================================
			Log("set \""+dbfile+"\" filename of DB, to let program regenerate the connection string.");
			PISDA.DBFileName = dbfile;	//set filename of DB, to let program regenerate the connection string.
			Log("\""+dbfile+"\" was been set");
			Log("");
			System.Threading.Thread.Sleep(1000);
			//================================================================================================================
		
			if(sql == ""){
				while(true)
				{
					Log("Type your SQL-request and press Enter:");
					Log(sql+"\n");
					Log("Results:");
					sql = Console.ReadLine();			//use specified sql, or read this from console.
					if(sql == "exit"){break;}

					DataTable datatable = PISDA.ExecuteWithResults(sql);
					LogDataTable(datatable);
					Log("");
				}
			}else{
				Log("Specified SQL-request:");
				Log(sql+"\n");
				Log("Results:");
				DataTable datatable = PISDA.ExecuteWithResults(sql);
				LogDataTable(datatable);
				Log("");
			}
		}

		//	Execute SQL, or run tests: 
		static void Main_(
			string[] args		//args[0] - db-filename nothing to run "tests", args[1] - mode SQL-request, as string (optional)
		)
		{
			string dbfile = "test.db";					//set default dbfilename for tests
			string sql = "";							//set this string as empty string, on start
			
			if(args.Length >= 1)						//if args[] exists
			{
				if(!String.IsNullOrEmpty(args[0]))			//and if db-filename exists
				{
					dbfile = args[0];							//use this db-filename
				}
				if(args.Length >= 2)						//and if second argument contains string with SQL-request
				{
					sql = args[1];								//use this SQL
				}
				ExecuteSQL(dbfile, sql);						//execute SQL
			}
			else{										//Otherwise:
				//Define string with Usage.
				string Usage =
@"
1. PISDA.exe [""dbfilename""] [""SQLRequest""] - execute SQLRequest specified as string, and exit.
2. PISDA.exe [""dbfilename""] + input SQLRequests in console.
3. PISDA.exe (without arguments) - to run tests with ""test.db""

See results in ""log.txt"".
";
				//Show usage:
				Log(Usage);
				Log("Press any key to continue...");
				Console.ReadKey();

				//run tests with "test.db"
				Tests(dbfile);
			}

			Log("Run Console.ReadKey(); to do not close window.");
			Log("");
			System.Threading.Thread.Sleep(timeout);

			Console.ReadKey();
		}
	}
}