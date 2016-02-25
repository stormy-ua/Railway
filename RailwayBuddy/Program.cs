using System;
using RailwayToolkit;

namespace RailwayBuddy
{
	public class Request
	{
		public string Name { get; set; }
		public string Email { get; set; }
	}
		
	class MainClass
	{
		private static Result<Request> ValidateName(Request request)
		{
			if (string.IsNullOrWhiteSpace (request.Name))
			{
				return Result.Failure<Request>(new Exception("Name must not be blank"));
			}

			return Result.Success<Request> (request);
		}

		private static Result<Request> ValidateEmail(Request request)
		{
			if (string.IsNullOrWhiteSpace (request.Email))
			{
				return Result.Failure<Request>(new Exception("Email must not be blank"));
			}

			return Result.Success<Request> (request);
		}

		private static Result<Request> ThrowException(Request request)
		{
			throw new Exception ("I am failing just for fun");
		}

		private static Request NameToUpper(Request request){
			return new Request
			{ 
				Name = request.Name.ToUpper(), 
				Email = request.Email 
			}; 
		}

		private static Request EmailToUpper(Request request){
			return new Request
			{ 
				Name = request.Name, 
				Email = request.Email.ToUpper()
			}; 
		}

		private static void LogRequest(Request request)
		{
			Console.WriteLine("Name: {0}, Email: {1}", request.Name, request.Email);
		}

		private static Request AppendDashToName(Request request)
		{
			return new Request
			{ 
				Name = request.Name + "-", 
				Email = request.Email
			};
		}

		private static void LogFailure(Exception exc)
		{
			Console.WriteLine("Failure: {0} ", exc.Message);
		}

		public static void Main (string[] args)
		{
			// combine validation functions
			var combinedValidation = Railway
                .Apply<Request>(r => ValidateName(r))
				.OnSuccess(r => ValidateEmail(r))
				.OnSuccess(r => NameToUpper(r))
				.OnSuccess(r => AppendDashToName(r))
				.OnSuccess(r => EmailToUpper(r))
                .OnSuccess(r => LogRequest(r))
                .OnFailure(e => LogFailure(e));

			// invoke combined function
			//var result = combinedValidation (new Request { Name = "", Email = "a@b.c" });
			//var result = combinedValidation (new Request { Name = "Kirill", Email = "" });
			var result = combinedValidation (new Request { Name = "Kirill", Email = "a@b.c" });

			// process result
			switch (result.IsSuccess) {
			case true:
				Console.WriteLine ("Success. {0} - {1}", result.Value.Name, result.Value.Email);
				break;
			case false:
				Console.WriteLine ("Failure: {0}", result.Error);
				break;
			}

			Console.ReadLine ();
		}
	}
}
