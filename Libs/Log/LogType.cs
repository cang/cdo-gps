using System;

namespace Log
{
    [Flags]
    public enum LogType
    {
        /// <summary>
        /// Not log
        /// </summary>
        None = 0,

        /// <summary>
        /// The debug.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// The info.
        /// </summary>
        Info = 1 << 1,

        /// <summary>
        /// The fatal.
        /// </summary>
        Fatal = 1 << 2,

        /// <summary>
        /// The warning.
        /// </summary>
        Warning = 1 << 3,

        /// <summary>
        /// The error.
        /// </summary>
        Error = 1 << 4,

        /// <summary>
        /// The success.
        /// </summary>
        Success = 1 << 5,

        /// <summary>
        /// The exception.
        /// </summary>
        Exception = 1 << 6,


        /// <summary>
        /// Log alls
        /// </summary>
        Verbose = Debug | Info | Fatal | Warning | Error | Success | Exception
    }
}
