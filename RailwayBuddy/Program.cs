using System;
using Railway;

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

		private static Exception LogFailure(Exception exc)
		{
			Console.WriteLine("Failure: {0} ", exc.Message);
			return exc;
		}

		public static void Main (string[] args)
		{
			// define validation functions
			Func<Request, Result<Request>> validateName = ValidateName;
			Func<Request, Result<Request>> validateEmail = ValidateEmail;
			Func<Request, Result<Request>> throwException = ThrowException;
			Func<Request, Request> nameToUpper = NameToUpper;
			Func<Request, Request> emailToUpper = EmailToUpper;
			Action<Request> logRequest = LogRequest;
			Func<Exception, Exception> logFailure = LogFailure;
			Func<Request, Request> appendDashToName = AppendDashToName;

			// combine validation functions
			var combinedValidation = validateName
				.Compose(validateEmail.Bind())
				.Compose(logRequest.Tee().Switch().Bind())
				//.Compose(throwException.TryCatch().Bind())
				.Compose(nameToUpper.Switch().Bind())
				.Compose(appendDashToName.DoubleMap(logFailure))
				.Compose(emailToUpper.Map());

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
