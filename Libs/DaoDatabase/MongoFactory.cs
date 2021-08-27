#region header
// /*********************************************************************************************/
// Project :DaoDatabase
// FileName : MongoFactory.cs
// Time Create : 2:06 PM 28/12/2015
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using MongoDB.Driver;

namespace DaoDatabase
{
    public class MongoFactory : IDbFactory
    {
        private readonly MongoServer _mongoServer;
        public MongoFactory(MongoConfig cfg)
        {
            var sv = new MongoServerAddress(cfg.Host, cfg.Port);
            _mongoServer = new MongoServer(new MongoServerSettings() { Server = sv });//todo: cấu hình connection ở đây.
            _mongoServer.Connect();
            //if (svc.DatabaseExists(cfg.DbName) && cfg.IsCreateNew)
            //    _mongoDatabase.Drop();
            //_mongoDatabase = svc.GetDatabase(cfg.DbName);
        }



        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _mongoServer.Disconnect();
        }

        public Reponsitory CreateContext(string name = "")
        {
            if(string.IsNullOrEmpty(name))
                throw new Exception("name database cant be null");
            return new MongoUnit(_mongoServer.GetDatabase(name));
        }

        #endregion
    }
}