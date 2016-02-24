using System;

namespace proto
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
				return Result.Fail<Request>("Name must not be blank");
			}

			return Result.Ok<Request> (request);
		}

		private static Result<Request> ValidateEmail(Request request)
		{
			if (string.IsNullOrWhiteSpace (request.Email))
			{
				return Result.Fail<Request>("Email must not be blank");
			}

			return Result.Ok<Request> (request);
		}

		private static Result<Request> ThrowException(Request request)
		{
			throw new Exception ("I am failing just for fun");
		}

		public static void Main (string[] args)
		{
			// define validation functions
			Func<Request, Result<Request>> validateName = request => ValidateName (request);
			Func<Request, Result<Request>> validateEmail = request => ValidateEmail (request);
			Func<Request, Result<Request>> throwException = request => ThrowException (request);

			// combine validation functions
			var combinedValidation = validateName
				.Compose(validateEmail.Bind())
				.Compose(throwException.TryCatch().Bind());

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
