using System;

namespace RailwayToolkit
{
	/// <summary>
	/// Group of functions that are designed to work with a type (here represented by railway track), 
	/// with the design goal that bigger pieces can be built by adapting and combining smaller pieces.
	/// </summary>
	public static class RailwayExtensions
	{
		/// <summary>
		/// Normal composition. A combiner that takes two normal functions and creates a new function 
		/// by connecting them in series.
		/// </summary>
		public static Func<T1, T3> Compose<T1, T2, T3>(this Func<T1, T2> g, Func<T2, T3> f)
		{
			return x => f(g(x));
		}

		/// <summary>
		/// An adapter that takes a switch function and creates a new function that accepts 
		/// two-track values as input.
		/// </summary>
		public static Func<Result<T>, Result<T>> Bind<T>(this Func<T, Result<T>> f)
		{
			return x => 
			{
				if (x.IsFailure) {
					return x;
				}

				return f (x.Value);
			};
		}

		/// <summary>
		/// An adapter that takes a normal one-track function and turns it into a switch function, 
		/// but also catches exceptions.
		/// </summary>
		public static Func<T, Result<T>> TryCatch<T>(this Func<T, Result<T>> f)
		{
			return x => 
			{
				try
				{
					return f (x);
				}
				catch(Exception exc){
					return Result.Failure<T>(exc);
				}
			};
		}

		/// <summary>
		/// An adapter that takes a normal one-track function and turns it into a switch function. 
		/// (Also known as a "lift" in some contexts.)
		/// </summary>
		public static Func<T, Result<T>> Switch<T>(this Func<T, T> f)
		{
			return x => Result.Success(f(x));
		}

		/// <summary>
		/// An adapter that takes a dead-end function and turns it into a one-track function 
		/// that can be used in a data flow. (Also known as tap.)
		/// </summary>
		public static Func<T, T> Tee<T>(this Action<T> f)
		{
			return x => {
				f (x);

				return x;
			};
		}

		/// <summary>
		/// An adapter that takes two one-track functions and turns them into a single two-track function.
		///  (Also known as bimap.)
		/// </summary>
		public static Func<Result<T>, Result<T>> DoubleMap<T>(this Func<T, T> success, Func<Exception, Exception> failure)
		{
			return x => {
				if (x.IsFailure) {
					Result.Failure<T>(failure(x.Error));
					return x;
				}
					
				return Result.Success(success(x.Value));
			};
		}

		/// <summary>
		/// An adapter that takes a normal one-track function and turns it into a two-track function. 
		/// (Also known as a "lift" in some contexts.)
		/// </summary>
		public static Func<Result<T>, Result<T>> Map<T>(this Func<T, T> f)
		{
			return x => {
				if (x.IsFailure) {
					return x;
				}

				return Result.Success(f(x.Value));
			};
		}
	}
}

