using System;

namespace RailwayToolkit
{
    public static class Railway
    {
        public static Func<T, Result<T>> Apply<T>(Func<T, Result<T>> f)
        {
            return f.TryCatch();
        }

        public static Func<T, Result<T>> Apply<T>(Action<T> f)
        {
            return f.Tee().Switch().TryCatch();
        }

        public static Func<T, Result<T>> Apply<T>(Func<T, T> f)
        {
            return f.Switch().TryCatch();
        }

        public static Func<T, Result<T>> OnSuccess<T>(this Func<T, Result<T>> f, Func<T, Result<T>> onSuccess)
        {
            return f.Compose(onSuccess.TryCatch().Bind());
        }

        public static Func<T, Result<T>> OnSuccess<T>(this Func<T, Result<T>> f, Func<T, T> onSuccess)
        {
            return f.Compose(onSuccess.Switch().TryCatch().Bind());
        }

        public static Func<T, Result<T>> OnSuccess<T>(this Func<T, Result<T>> f, Action<T> onSuccess)
        {
            return f.Compose(onSuccess.Tee().Switch().TryCatch().Bind());
        }

        public static Func<T, Result<T>> OnFailure<T>(this Func<T, Result<T>> f, Func<Exception, Exception> onFailure)
        {
            return f.Compose(Unit<T>().DoubleMap(onFailure));
        }

        public static Func<T, Result<T>> OnFailure<T>(this Func<T, Result<T>> f, Action<Exception> onFailure)
        {
            return f.Compose(Unit<T>().DoubleMap(onFailure.Tee()));
        }

        private static Func<T, T> Unit<T>()
        {
            return t => t;
        }
    }
}