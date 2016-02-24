using System;

namespace proto
{
	public static class FunctionalExtensions
	{
		public static Func<T1, T3> Compose<T1, T2, T3>(this Func<T1, T2> g, Func<T2, T3> f)
		{
			return x => f(g(x));
		}

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

		public static Func<T, Result<T>> TryCatch<T>(this Func<T, Result<T>> f)
		{
			return x => 
			{
				try
				{
					return f (x);
				}
				catch(Exception exc){
					return Result.Fail<T>(exc.Message);
				}
			};
		}

		public static Func<Result<T>, Result<T>> TryCatchBind<T>(this Func<T, Result<T>> f)
		{
			return f.TryCatch().Bind();
		}

		public static Func<T, Result<T>> Switch<T>(this Func<T, T> f)
		{
			return x => Result.Ok<T> (f(x));
		}
	}
}

