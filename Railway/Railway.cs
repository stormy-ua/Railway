using System;
using System.Linq;

namespace RailwayToolkit
{
    public static class Railway
    {
        public static Func<T, Result<V>> Apply<T, V>(Func<T, Result<V>> f)
        {
            return f.TryCatch();
        }

        public static Func<T, Result<T>> Apply<T>(Func<T, Result<T>> f)
        {
            return f.TryCatch();
        }

        public static Func<T, Result<T>> Apply<T>(Action<T> f)
        {
            return f.Tee().Switch().TryCatch();
        }

        public static Func<T, Result<V>> Apply<T, V>(Func<T, V> f)
        {
            return f.Switch().TryCatch();
        }

        public static Func<T, Result<T>> Apply<T>(Func<T, T> f)
        {
            return f.Switch().TryCatch();
        }

        public static Func<T, Result<V>> OnSuccess<T, U, V>(this Func<T, Result<U>> f, Func<U, Result<V>> onSuccess)
        {
            return f.Compose(onSuccess.TryCatch().Bind());
        }

        public static Func<T, Result<V>> OnSuccess<T, U, V>(this Func<T, Result<U>> f, Func<U, V> onSuccess)
        {
            return f.Compose(onSuccess.Switch().TryCatch().Bind());
        }

        public static Func<T, Result<V>> OnSuccess<T, V>(this Func<T, Result<V>> f, Action<V> onSuccess)
        {
            return f.Compose(onSuccess.Tee().Switch().TryCatch().Bind());
        }

		public static Func<T, Result<V>> OnSuccess<T, U, V>(this Func<T, Result<U>> f, 
			Func<V, V, V> aggregateSuccess, Func<Exception, Exception, Exception> aggregateFailure,
			Func<U, Result<V>> onSuccess,
			params Func<U, Result<V>>[] onSuccesses)
		{
			return f.Compose(onSuccess.Plus(aggregateSuccess, aggregateFailure, onSuccesses).Bind());
		}

        public static Func<T, Result<V>> OnFailure<T, V>(this Func<T, Result<V>> f, Func<Exception, Exception> onFailure)
        {
            return f.Compose(Unit<V>().DoubleMap(onFailure));
        }

        public static Func<T, Result<V>> OnFailure<T, V>(this Func<T, Result<V>> f, Action<Exception> onFailure)
        {
            return f.Compose(Unit<V>().DoubleMap(onFailure.Tee()));
        }

        private static Func<T, T> Unit<T>()
        {
            return t => t;
        }
    }
}