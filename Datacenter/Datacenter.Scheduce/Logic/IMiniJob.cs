#region header
// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Datacenter.Scheduce
// TIME CREATE : 10:02 PM 18/12/2016
// FILENAME: IMiniJob.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************
#endregion

using Datacenter.QueryRoute;
using DataCenter.Core;
using Log;

namespace Datacenter.Scheduce.Logic
{
    public interface IMiniJob
    {
        void Handle(ReponsitoryFactory db, IDataStore cache, ILog log);
    }
}