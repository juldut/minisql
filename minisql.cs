using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.IO;
using System.Globalization;
using System.Collections;

// c:\Windows\Microsoft.NET\Framework\v2.0.50727\csc
// c:\Windows\Microsoft.NET\Framework\v4.0.30319\csc

public class Sbs {
	public static String connectionString ;
	public static String inputPass() {
		String hasil = "";
		ConsoleKeyInfo tempKey;
		do {
			tempKey = Console.ReadKey();
			
			// Backspace Should Not Work
			if (tempKey.Key != ConsoleKey.Backspace && tempKey.Key != ConsoleKey.Enter) {
			   hasil += tempKey.KeyChar;
			   Console.Write("\b*");
			}
			else {
			   if (tempKey.Key == ConsoleKey.Backspace && hasil.Length > 0) {
				  hasil = hasil.Substring(0, (hasil.Length - 1));
				  Console.Write("\b");
			   }
			}			
		} while(tempKey.Key != ConsoleKey.Enter);
		Console.WriteLine();
		return(hasil);
	}
	public static void Main() {
		String userDb, passDb, serverDb, dbName;
		int i, fieldLength;
		
		fieldLength = 15;
		
		Console.Write("Server : ");
		serverDb = Console.ReadLine();
		Console.Write("Database : ");
		dbName = Console.ReadLine();
		Console.Write("Username : ");
		userDb = Console.ReadLine();
		Console.Write("Password : ");
		passDb = inputPass();
		
		connectionString = "Password=" + passDb
			+ ";Persist Security Info=True;User ID=" + userDb
			+ ";Initial Catalog=" + dbName
			+ ";Data Source=" + serverDb;
		
		SqlConnection conn = null;
		SqlCommand command;
		SqlDataReader reader;
		
		try {
			conn = new SqlConnection(connectionString);
			command = new SqlCommand("SELECT 1", conn);
			conn.Open();
			// command.Parameters.AddWithValue("@paramUser", user);
			reader = command.ExecuteReader();
			conn.Close();
		}
		catch (SqlException ex) {
			Console.WriteLine("SqlException: {0}", ex.Message);
			Console.ReadKey();
			Environment.Exit(0);
		}
		
		string pil = "";
		string inputSql, tempString;
		while (!pil.Equals("0")) {
			Console.Clear();
			Console.WriteLine(" -= Mini SQL =- ");
			Console.WriteLine("1 - ExecuteReader");
			Console.WriteLine("2 - ExecuteNonQuery");
			Console.WriteLine("3 - ExecuteReader to .csv");
			Console.WriteLine("0 - Exit");
			Console.Write("Masukkan pilihan : ");
			pil = Console.ReadLine();
			inputSql = "";
			
			if (pil.Equals("1")) {
				while (!inputSql.Equals("quit")) {
					Console.Write(serverDb + "." + dbName + ">");
					inputSql = Console.ReadLine();
					if (inputSql.Equals("exit")) {
						Environment.Exit(0);
					}
					
					try {
						command = new SqlCommand(inputSql, conn);
						conn.Open();
						reader = command.ExecuteReader();
						Console.Write("| ");
						for(i=0; i<reader.FieldCount; i++) {
							tempString = reader.GetName(i);
							if (tempString.Length > fieldLength) tempString = tempString.Substring(0, fieldLength);
							Console.Write(tempString.PadRight(fieldLength, ' ') + " | ");
						}
						Console.WriteLine("");
						for (i=0; i<Console.BufferWidth-1; i++) {
							Console.Write("-");
						}
						Console.WriteLine("");
						while (reader.Read()) {
							Console.Write("| ");
							for(i=0; i<reader.FieldCount; i++) {
								tempString = "";
								if (!reader.IsDBNull(i)) {
									if (reader.GetDataTypeName(i).Equals("datetime")) {
										DateTime tempDateTime = (DateTime)reader[i];
										tempString = tempDateTime.ToString("dd-MMM-yy", new CultureInfo("en-US"));
									}
									else if (reader.GetDataTypeName(i).Equals("int")) {
										tempString = reader.GetInt32(i).ToString();
									}
									else {
										tempString = (string)reader[i];
									}
								}
								if (tempString.Length > fieldLength) tempString = tempString.Substring(0, fieldLength);
								Console.Write(tempString.PadRight(fieldLength, ' ') + " | ");
								
								// Console.Write(reader[i] + " | ");
								// Console.WriteLine(reader.GetDataTypeName(i));
							}
							Console.WriteLine("");
						}
						conn.Close();
					}
					catch (SqlException ex) {
						Console.WriteLine("SqlException: {0}", ex.Message);
						conn.Close();
					}
				}
				
			}
			else if (pil.Equals("2")) {
				while (!inputSql.Equals("quit")) {
					Console.Write(serverDb + "." + dbName + ">");
					inputSql = Console.ReadLine();
					if (inputSql.Equals("exit")) {
						Environment.Exit(0);
					}
					
					try {
						command = new SqlCommand(inputSql, conn);
						conn.Open();
						command.ExecuteNonQuery();
						conn.Close();
						Console.WriteLine("Successfully executed");
					}
					catch (SqlException ex) {
						Console.WriteLine("SqlException: {0}", ex.Message);
						conn.Close();
					}
				}
			}
			else if (pil.Equals("3")) {
				while (!inputSql.Equals("quit")) {
					Console.Write(serverDb + "." + dbName + ">");
					inputSql = Console.ReadLine();
					if (inputSql.Equals("exit")) {
						Environment.Exit(0);
					}
					
					try {
						ArrayList hasil = new ArrayList();
						command = new SqlCommand(inputSql, conn);
						conn.Open();
						reader = command.ExecuteReader();
						
						tempString = "";
						for(i=0; i<reader.FieldCount; i++) {
							tempString += "\"" + reader.GetName(i) + "\",";
						}
						hasil.Add(tempString);
						
						while (reader.Read()) {
							tempString = "";
							for(i=0; i<reader.FieldCount; i++) {
								tempString += "\"" + Convert.ToString(reader[i]) + "\",";
							}
							hasil.Add(tempString);
						}
						conn.Close();
						System.IO.File.WriteAllLines(@"hasil.csv", (String[])hasil.ToArray(typeof( string )));
					}
					catch (SqlException ex) {
						Console.WriteLine("SqlException: {0}", ex.Message);
						conn.Close();
					}
				}
			}
		}
	}
}
