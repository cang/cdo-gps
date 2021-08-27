﻿using System;

namespace Core.Utils.Delay
{
    internal class RequestInfo
    {
        internal Delegate Delegate { get; set; }
        internal DateTime TimeRegister { get; set; }
        internal TimeSpan TimeOut { get; set; }

        internal bool IsTimeOut => (DateTime.Now - TimeRegister) > TimeOut;
        public bool Lock { get; set; }
    }
}