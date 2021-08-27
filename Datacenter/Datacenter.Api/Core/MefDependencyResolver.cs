#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : MefDependencyResolver.cs
// Time Create : 9:40 AM 12/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Web.Http.Dependencies;
using Log;

#endregion

namespace Datacenter.Api.Core
{
    public class MefDependencyResolver : IDependencyResolver, System.Web.Mvc.IDependencyResolver
    {
        /// <summary>
        ///     The _container.
        /// </summary>
        private CompositionContainer _container;

        private bool _disposed;
        private ILog _log;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MefDependencyResolver" /> class.
        /// </summary>
        /// <param name="container">
        /// </param>
        public MefDependencyResolver(CompositionContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// </summary>
        public FilteredCatalog FilteredCat { get; set; }

        /// <summary>
        ///     The begin scope.
        /// </summary>
        /// <returns>
        ///     The <see cref="IDependencyScope" />.
        /// </returns>
        public IDependencyScope BeginScope()
        {
            var filteredCat = new FilteredCatalog(_container.Catalog,
                def => def.Metadata.ContainsKey(CompositionConstants.PartCreationPolicyMetadataName) &&
                       (CreationPolicy) def.Metadata[CompositionConstants.PartCreationPolicyMetadataName] ==
                       CreationPolicy.NonShared);
            var child = new CompositionContainer(filteredCat, _container);

            return new MefDependencyResolver(child) {FilteredCat = filteredCat};
            //return this;
        }

        /// <summary>
        ///     Called to request a service implementation.
        ///     Here we call upon MEF to instantiate implementations of dependencies.
        /// </summary>
        /// <param name="serviceType">
        ///     Type of service requested.
        /// </param>
        /// <returns>
        ///     Service implementation or null.
        /// </returns>
        public object GetService(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            try
            {
                var name = AttributedModelServices.GetContractName(serviceType);
                //var contain=_container.GetExportedValue<IUserManage>();
                var export = _container.GetExportedValueOrDefault<object>(name);
                return export;
            }
            catch (Exception ex)
            {
                // CustomLog.Writer(ex.ToString());
                return null;
            }
        }

        /// <summary>
        ///     Called to request service implementations.
        ///     Here we call upon MEF to instantiate implementations of dependencies.
        /// </summary>
        /// <param name="serviceType">
        ///     Type of service requested.
        /// </param>
        /// <returns>
        ///     Service implementations.
        /// </returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            var exports = _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
            return exports;
        }

        /// <summary>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) // Managed:
                {
                    //NOOP since MEF does not have the idea of a Scoped Container (except it does have a concept of a filtered container!)
                    //
                    Trace.WriteLine("DISPOSING MEF CONTAINER.");
                    _container.Dispose();
                    _container = null;
                    FilteredCat?.Dispose();
                    FilteredCat = null;
                }
                // Unmanaged:


                _disposed = true;
            }
        }

        /// <summary>
        /// </summary>
        ~MefDependencyResolver()
        {
            Dispose(false);
        }
    }
}