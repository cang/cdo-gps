#region header
// /*********************************************************************************************/
// Project :Datacenter.DataTranport
// FileName : DataTranportFactory.cs
// Time Create : 2:40 PM 22/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System.ComponentModel.Composition;
using StarSg.Core;

namespace Datacenter.DataTranport
{
    [Export(typeof(IModuleFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DataTranportFactory:IModuleFactory,IPartImportsSatisfiedNotification
    {
        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            
        }

        #endregion
    }
}