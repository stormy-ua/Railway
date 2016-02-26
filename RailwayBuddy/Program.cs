using System;
using RailwayToolkit;

namespace RailwayBuddy
{
	public class Request
	{
		public string Name { get; set; }
		public string Email { get; set; }

		public override string ToString ()
		{
			return string.Format ("[Request: Name={0}, Email={1}]", Name, Email);
		}
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
			
		private static void LogRequest(Request request)
		{
			Console.WriteLine("Name: {0}, Email: {1}", request.Name, request.Email);
		}

		public static void Main (string[] args)
		{
			// combine validation functions
			var combinedValidation = Railway
				// log inpiut request
				.Apply<Request> (r => LogRequest(r))
				// do email and name validation in parallel and combine errors
				.OnSuccess(
					(r1, r2) => r1,
					(e1, e2) => new AggregateException(e1, e2),
					r => ValidateName(r),
					r => ValidateEmail(r)
				)
				// extract request name
				.OnSuccess (request => request.Name)
				// log extracted name
				.OnSuccess (name => Console.WriteLine ("Name: {0}", name))
				// append dash to name
				.OnSuccess (name => name + "-")
				// log name
				.OnSuccess (name => Console.WriteLine ("Name: {0}", name))
				// make nume uppercase
				.OnSuccess (name => name.ToUpper ())
				// log name
				.OnSuccess (name => Console.WriteLine ("Name: {0}", name))
				// log failure if any occured during the pipeline execution
				.OnFailure (e => Console.WriteLine ("Failure: {0} ", e.Message));

			// invoke combined function
			var result = combinedValidation (new Request { Name = "", Email = "" });
			//var result = combinedValidation (new Request { Name = "", Email = "a@b.c" });
			//var result = combinedValidation (new Request { Name = "Kirill", Email = "" });
			//var result = combinedValidation (new Request { Name = "Kirill", Email = "a@b.c" });

			// process result
			switch (result.IsSuccess) {
			case true:
				Console.WriteLine ("Success. {0}", result.Value);
				break;
			case false:
				Console.WriteLine ("Failure: {0}", result.Error);
				break;
			}

			Console.ReadLine ();
		}
	}
}
