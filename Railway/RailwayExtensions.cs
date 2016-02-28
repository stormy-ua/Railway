using System;
using System.Linq;

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
		public static Func<Result<T>, Result<V>> Bind<T, V>(this Func<T, Result<V>> f)
		{
			return x => 
			{
				if (x.IsFailure) {
					return Result.Failure<V>(x.Error);
				}

				return f (x.Value);
			};
		}

		/// <summary>
		/// An adapter that takes a normal one-track function and turns it into a switch function, 
		/// but also catches exceptions.
		/// </summary>
		public static Func<T, Result<V>> TryCatch<T, V>(this Func<T, Result<V>> f)
		{
			return x => 
			{
				try
				{
					return f (x);
				}
				catch(Exception exc){
					return Result.Failure<V>(exc);
				}
			};
		}

		/// <summary>
		/// An adapter that takes a normal one-track function and turns it into a switch function. 
		/// (Also known as a "lift" in some contexts.)
		/// </summary>
		public static Func<T, Result<V>> Switch<T, V>(this Func<T, V> f)
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
		public static Func<Result<T>, Result<V>> DoubleMap<T, V>(this Func<T, V> success, Func<Exception, Exception> failure)
		{
			return x => {
				if (x.IsFailure) {
					return Result.Failure<V>(failure(x.Error));
				}
					
				return Result.Success(success(x.Value));
			};
		}

		/// <summary>
		/// An adapter that takes a normal one-track function and turns it into a two-track function. 
		/// (Also known as a "lift" in some contexts.)
		/// </summary>
		public static Func<Result<T>, Result<V>> Map<T, V>(this Func<T, V> f)
		{
			return x => {
				if (x.IsFailure) {
					return Result.Failure<V>(x.Error);
				}

				return Result.Success(f(x.Value));
			};
		}

		/// <summary>
		/// A combiner that takes two switch functions and creates a new switch function 
		/// by joining them in "parallel" and "adding" the results. 
		/// (Also known as ++ and <+> in other contexts.)
		/// </summary>
		public static Func<U, Result<V>> Plus<U, V>(
			this Func<U, Result<V>> switch1,
			Func<U, Result<V>> switch2, 
			Func<V, V, V> aggregateSuccess, Func<Exception, Exception, Exception> aggregateFailure)

		{
			return x => {
				var result1 = switch1(x);
				var result2 = switch2(x);

				if(result1.IsSuccess && result2.IsSuccess) {
					return Result.Success(aggregateSuccess(result1.Value, result2.Value));
				}
				
				if(result1.IsSuccess && result2.IsFailure){
					return Result.Failure<V>(result2.Error);
				}

				if(result1.IsFailure && result2.IsSuccess) {
					return Result.Failure<V>(result1.Error);
				}

				return Result.Failure<V>(aggregateFailure(result1.Error, result2.Error));
			};
		}
	}
}

