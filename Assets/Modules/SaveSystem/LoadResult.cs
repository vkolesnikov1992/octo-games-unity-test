using System;

namespace OctoGamesTest.SaveSystem
{
    public readonly struct LoadResult<T>
        where T : class
    {
        private LoadResult(T value, LoadErrorCode errorCode, Exception exception)
        {
            Value = value;
            ErrorCode = errorCode;
            Exception = exception;
        }

        public T Value { get; }

        public LoadErrorCode ErrorCode { get; }

        public Exception Exception { get; }

        public bool Success => ErrorCode == LoadErrorCode.None;

        public static LoadResult<T> Ok(T value)
        {
            return new LoadResult<T>(value, LoadErrorCode.None, null);
        }

        public static LoadResult<T> Failed(LoadErrorCode errorCode, Exception exception = null)
        {
            if (errorCode == LoadErrorCode.None)
            {
                throw new ArgumentException("Failure result requires an error code.", nameof(errorCode));
            }

            return new LoadResult<T>(null, errorCode, exception);
        }
    }
}
