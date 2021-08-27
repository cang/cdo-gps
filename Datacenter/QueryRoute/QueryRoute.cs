#region header

// /*********************************************************************************************/
// Project :QueryRoute
// FileName : QueryRoute.cs
// Time Create : 1:36 PM 17/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DaoDatabase;

namespace Datacenter.QueryRoute
{
    internal class ReponsitoryInfo
    {
        public ReponsitoryInfo(string name,bool isMother)
        {
            Name = name;
            IsMother = isMother;
        }

        public string Name { get; }
        public bool IsMother { get; set; }
        public Reponsitory Reponsitory { get; set; }
    }

    public class QueryRoute : IQueryRoute
    {
        private readonly IDictionary<int, ReponsitoryInfo> _allRegisterDb =
            new Dictionary<int, ReponsitoryInfo>();


        public QueryRoute(params Tuple<int,string,bool>[] dbInfo )
        {
            foreach (var tuple in dbInfo.Where(tuple => !_allRegisterDb.ContainsKey(tuple.Item1)))
            {
                _allRegisterDb.Add(tuple.Item1, new ReponsitoryInfo(tuple.Item2, tuple.Item3));
            }
        }

        #region Implementation of IDisposable

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var info in _allRegisterDb.Values)
            {
                info.Reponsitory?.Dispose();
            }
            _allRegisterDb.Clear();
        }

        #endregion

        private void ValidDbCheck(int id)
        {
            if (!_allRegisterDb.ContainsKey(id))
                throw new NullReferenceException("Db không được đăng ký");
        }

        public void Commit()
        {
            GetReponsitory().Commit();
        }

        public Reponsitory GetReponsitory(int id)
        {
            ValidDbCheck(id);
            return _allRegisterDb[id].Reponsitory ??
                   (_allRegisterDb[id].Reponsitory = UnitOfWorkFactory.GetUnitOfWork(_allRegisterDb[id].Name,
                       DbSupportType.MicrosoftSqlServer));
        }

        public Reponsitory GetReponsitory()
        {
            var id = _allRegisterDb.First(m => m.Value.IsMother).Key;
            return GetReponsitory(id);
        }

        #region Implementation of IQueryRoute

        public void Insert<T>(T obj, int id) where T : class, IEntity
        {
            GetReponsitory(id).Insert(obj);
            //throw new NotImplementedException();
        }

        public void InsertAll<T>(ICollection<T> objs, int id) where T : class, IEntity
        {
            GetReponsitory(id).InsertAll(objs);
            //throw new NotImplementedException();
        }

        //public void InsertAll<T>(String entityName,ICollection<T> objs, int id) where T : class, IEntity
        //{
        //    GetReponsitory(id).InsertAll(entityName, objs);
        //}

        public void Delete<T>(T obj, int id) where T : class, IEntity
        {
            GetReponsitory(id).Delete(obj);
            //throw new NotImplementedException();
        }

        public T Update<T>(T obj, int id) where T : class, IEntity
        {
            return GetReponsitory(id).Update(obj);
            //throw new NotImplementedException();
        }

        /// <summary>
        /// chỉ cập nhật 1 số trường nhất định
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="id"></param>
        /// <param name="fied"></param>
        /// <returns></returns>
        public bool Update<T>(T obj,int id ,params Expression<Func<T, object>>[] fied) where T : class, IEntity
        {
            return GetReponsitory(id).Update(obj, fied);
        }

        public void InsertOrUpdate<T>(T obj, int id) where T : class, IEntity
        {
            GetReponsitory(id).InsertOrUpdate(obj);
            //throw new NotImplementedException();
        }

        public T Get<T>(object key, int id) where T : class, IEntity
        {
            return GetReponsitory(id).Get<T>(key);
            //throw new NotImplementedException();
        }

        public ICollection<T> GetWhere<T>(Expression<Func<T, bool>> expression, int id) where T : class, IEntity
        {
            return GetReponsitory(id).GetWhere(expression);
        }

        public IDictionary<TKey, T> GetAll<TKey, T>(Func<T, TKey> key, int id) where T : class, IEntity
        {
            return GetReponsitory(id).GetAll(key);
        }

        public IList<T> TakeDest<T>(Expression<Func<T, bool>> expression, int take, int id, Expression<Func<T, object>> order = null) where T : class, IEntity
        {
            return GetReponsitory(id).TakeDest(expression,take,order);
        }

        public IList<T> TakeAsc<T>(Expression<Func<T, bool>> expression, int take, int id, Expression<Func<T, object>> order = null) where T : class, IEntity
        {
            return GetReponsitory(id).TakeAsc(expression, take, order);
        }

        public void CustomHandle<TSql>(Action<TSql> action, int id)
        {
            GetReponsitory(id).CustomHandle(action);
        }

        public IReponsitoryQuery<T> CreateQuery<T>(int id) where T : class, IEntity
        {
            return GetReponsitory(id).CreateQuery<T>();
        }

        public void Commit(int id)
        {
            GetReponsitory(id).Commit();
        }

        #endregion
    }
}