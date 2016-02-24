﻿using System;

namespace proto
{
	public class Result
	{
		public static Result<T> Ok<T>(T value)
		{
			return new Result<T>(value, true, null);
		}

		public static Result<T> Fail<T>(string error)
		{
			return new Result<T>(default(T), false, error);
		}
	}

	public class Result<T>
	{
		readonly T _value;
		readonly bool _isSuccess;
		readonly string _error;

		internal Result(T value, bool isSuccess, string error)
		{
			_value = value;
			_isSuccess = isSuccess;
			_error = error;
		}

		public bool IsSuccess { get { return _isSuccess; } }

		public string Error { get { return _error; } }

		public T Value { get { return _value; } }

		public bool IsFailure
		{
			get { return !IsSuccess; }
		}
	}
}

