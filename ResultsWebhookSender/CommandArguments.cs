using CmdLine;
using System;
using System.Data;
using System.Globalization;

namespace ResultsWebhookSender
{
	[CommandLineArguments(Program = "ResultsWebhookSender")]
	public class CommandArguments
	{
		[CommandLineParameter(Command = "Proc", Required = false, Description = "Name of the stored procedure to execute, eg: sp_get_results -- if not specified, the 'Query' parameter must be supplied.")]
		public string Procedure { get; set; }

		[CommandLineParameter(Command = "Query", Required = false, Description = "Name of the query to execute enclosed in quotations, eg: \"select top 1 amount from SalesData order by date desc\" -- if not specified, the 'Proc' parameter must be supplied.")]
		public string Query { get; set; }

		[CommandLineParameter(Command = "Url", Required = true, Description = "Webhook endpoint for your Results.com goal, eg: https://api.results.com/v2/goals/1234/webhook/Eyjtvom4...")]
		public string Url { get; set; }

		[CommandLineParameter(Command = "Date", Required = false, Description = "Date in ISO8601 format yyyy-mm-dd, eg: 2017-05-30. Defaults to today's date if not specified")]
		public string Date { get; set; }

		public CommandType CommandType { get; set; }

		public string CommandText { get; set; }

		/// <summary>
		/// Returnes the parsed command line arguments
		/// </summary>
		/// <returns>The parsed <see cref="CommandArguments"/> object</returns>
		public static CommandArguments Parse()
		{
			CommandArguments args = null;

			try
			{
				args = CommandLine.Parse<CommandArguments>();

				if ((string.IsNullOrWhiteSpace(args.Procedure) && string.IsNullOrWhiteSpace(args.Query))
					|| (string.IsNullOrWhiteSpace(args.Procedure) && string.IsNullOrWhiteSpace(args.Query)))
				{
					throw new CommandLineException("Either 'Proc' or 'Query' must be specified, but not both");
				}

				if (!string.IsNullOrWhiteSpace(args.Query))
				{
					args.CommandType = CommandType.Text;
					args.CommandText = args.Query;
				}
				else
				{
					args.CommandType = CommandType.StoredProcedure;
					args.CommandText = args.Procedure;
				}

				string iso8601Format = "yyyy-MM-dd";
				DateTime targetDate = DateTime.Today;

				if (!string.IsNullOrWhiteSpace(args.Date))
				{
					if (!DateTime.TryParseExact(args.Date, iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out targetDate))
					{
						throw new CommandLineException("The 'Date' parameter, if specified, must be in valid ISO8601 (yyyy-mm-dd) format, eg: " + DateTime.Today.ToString(iso8601Format));
					}
				}

				args.Date = targetDate.ToString(iso8601Format);
			}

			catch (CommandLineException e)
			{
				Console.Error.WriteLine(e.ArgumentHelp.Message);
				Console.Error.WriteLine(e.ArgumentHelp.GetHelpText(Console.BufferWidth));
				Environment.Exit(1);
			}

			return args;
		}
	}
}
