using RestSharp;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;

namespace ResultsWebhookSender
{
	class Program
	{
		static void Main(string[] args)
		{
			string connectionString = ConfigurationManager.ConnectionStrings["Sql"].ConnectionString;

			if (string.IsNullOrWhiteSpace(connectionString))
			{
				Console.Error.WriteLine("Please edit the .config file and specify a valid connection string for the 'Sql' entry.");
				Environment.Exit(1);
			}

			try
			{
				CommandArguments arguments = CommandArguments.Parse();

				using (var connection = new SqlConnection(connectionString))
				{
					connection.Open();

					using (var command = new SqlCommand(arguments.CommandText, connection))
					{
						command.CommandType = arguments.CommandType;

						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								decimal value = Convert.ToDecimal(reader[0]);

								PostDataToResults(arguments, value);
							}

							else
							{
								Console.Error.WriteLine("Unable to read data from SQL response");
							}
						}
					}
				}
			}

			catch (Exception ex)
			{
				Console.Error.WriteLine("An error occurred: " + ex.Message);
				Environment.Exit(1);
			}
		}


		/// <summary>
		/// Takes the retrieved value and posts it to the webhook endpoint
		/// </summary>
		/// <param name="args">The parsed <see cref="CommandArguments"/> object</param>
		/// <param name="value">The decimal value</param>
		private static void PostDataToResults(CommandArguments args, decimal value)
		{
			Uri uri = new Uri(args.Url);

			var client = new RestClient(uri.Scheme + Uri.SchemeDelimiter + uri.Host);
			var request = new RestRequest(uri.AbsolutePath, Method.POST);
			request.AddParameter("date", args.Date, ParameterType.GetOrPost);
			request.AddParameter("value", value, ParameterType.GetOrPost);
			request.RequestFormat = DataFormat.Json;

			var response = client.Execute(request);

			if (response.StatusCode != HttpStatusCode.Created)
			{
				Console.Error.WriteLine("Not successful. Received response {0}: {1}", response.StatusCode, response.StatusDescription);
				Environment.Exit(1);
			}

			else
			{
				Console.Out.WriteLine("Data value {0} for date {1} successfully posted to webhook {2}",
					value, args.Date, args.Url);
			}
		}
	}
}
