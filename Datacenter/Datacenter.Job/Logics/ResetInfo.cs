#region header
// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Datacenter.Job
// TIME CREATE : 10:27 PM 25/10/2016
// FILENAME: ResetInfo.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************
#endregion

using System;
using Datacenter.QueryRoute;
using DataCenter.Core;
using Log;

namespace Datacenter.Job.Logics
{
    public class ResetInfo:IZipLogic
    {
        public void Handle(IQueryRoute dataContext, IDataStore dataCache, ILog log, DateTime time)
        {
            
        }
    }
}