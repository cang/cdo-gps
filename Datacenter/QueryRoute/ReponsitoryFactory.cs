#region header

// /*********************************************************************************************/
// Project :QueryRoute
// FileName : ReponsitoryFactory.cs
// Time Create : 1:29 PM 17/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using DaoDatabase;

namespace Datacenter.QueryRoute
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ReponsitoryFactory
    {
        private readonly IList<Tuple<int, string,bool>> _allDbInfo = new List<Tuple<int, string,bool>>();

        /// <summary>
        ///     đăng ký thông tin database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isMother"></param>
        /// <param name="dbName"></param>
        /// <param name="cfg"></param>
        public void Register(int id, bool isMother,string dbName, DatabaseConfig cfg)
        {
            UnitOfWorkFactory.RegisterDatabase(dbName, cfg);
            _allDbInfo.Add(new Tuple<int, string,bool>(id, dbName,isMother));
        }

        /// <summary>
        ///     Tạo 1 quản lý kết nối cơ sở dữ liệu
        /// </summary>
        /// <returns></returns>
        public IQueryRoute CreateQuery()
        {
            return new QueryRoute(_allDbInfo.ToArray());
        }
    }
}