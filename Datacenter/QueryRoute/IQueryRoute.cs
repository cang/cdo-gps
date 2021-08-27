#region header
// /*********************************************************************************************/
// Project :QueryRoute
// FileName : IQueryRoute.cs
// Time Create : 11:19 AM 17/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DaoDatabase;

namespace Datacenter.QueryRoute
{
    public interface IQueryRoute:IDisposable
    {
        void Insert<T>(T obj,int id) where T : class,IEntity;

        void InsertAll<T>(ICollection<T> objs, int id) where T : class,IEntity;
        //void InsertAll<T>(String entityName, ICollection<T> objs, int id) where T : class, IEntity;

        void Delete<T>(T obj, int id) where T : class, IEntity;
        T Update<T>(T obj, int id) where T : class, IEntity;

        /// <summary>
        /// chỉ cập nhật 1 số trường nhất định
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="id"></param>
        /// <param name="fied"></param>
        /// <returns></returns>
        bool Update<T>(T obj, int id, params Expression<Func<T, object>>[] fied) where T : class, IEntity;
        //void Update<T>(T obj,params Expression<Func<T, object>>[] poperties ) where T : class;
        //void Update<T>(T obj,params string[] poperties) where T : class;
        void InsertOrUpdate<T>(T obj, int id) where T : class, IEntity;
        T Get<T>(object key, int id) where T : class, IEntity;
        ICollection<T> GetWhere<T>(Expression<Func<T, bool>> expression, int id) where T : class, IEntity;
        IDictionary<TKey, T> GetAll<TKey, T>(Func<T, TKey> key, int id) where T : class, IEntity;
        IList<T> TakeDest<T>(Expression<Func<T, bool>> expression,
          int take, int id, Expression<Func<T, object>> order = null) where T : class, IEntity;
         IList<T> TakeAsc<T>(Expression<Func<T, bool>> expression,
            int take, int id, Expression<Func<T, object>> order = null) where T : class, IEntity;
        void CustomHandle<TSql>(Action<TSql> action, int id);
        IReponsitoryQuery<T> CreateQuery<T>(int id) where T : class, IEntity;
        void Commit(int id);
        void Commit();
        Reponsitory GetReponsitory(int id);
        Reponsitory GetReponsitory();
    }
}