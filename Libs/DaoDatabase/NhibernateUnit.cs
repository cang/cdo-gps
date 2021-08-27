using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using DaoDatabase.AutoMapping.IAttribute;
using FluentNHibernate.Utils;
using NHibernate;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using System.Data.SqlClient;

namespace DaoDatabase
{
    /// <summary>
    ///     The unit of work.
    /// </summary>
    public class NhibernateUnit : Reponsitory
    {
        /// <summary>
        ///     The _factory.
        /// </summary>
        /// <summary>
        ///     The _session.
        /// </summary>
        private ISession _session;

        public NhibernateUnit(ISession ss)
        {
            _session = ss;
        }


        /// <summary>
        ///     The refresh.
        /// </summary>
        private void Refresh()
        {
            if (_session != null && !_session.IsOpen)
            {
                _session.Connection.Open();
            }
            LastTimeUsing = DateTime.Now;
        }
        
        public override IList<T> TakeAsc<T>(Expression<Func<T, bool>> expression, int take, Expression<Func<T, object>> order = null)
        {
            Refresh();
            if (order != null)
                return
                    _session.QueryOver<T>()
                        .Where(expression).OrderBy(order).Asc
                        .TransformUsing(new DistinctRootEntityResultTransformer())
                        .Take(take)
                        .List();
            return
                _session.QueryOver<T>()
                    .Where(expression)
                    .TransformUsing(new DistinctRootEntityResultTransformer())
                    .Take(take)
                    .List();
        }

        public override IList<T> TakeDestMinimum<T>(Expression<Func<T, bool>> expression, int take, Expression<Func<T, object>> order, params Expression<Func<T, object>>[] propertiSelect)
        {
            Refresh();
            if (order != null)
            {
                var tmp= _session.QueryOver<T>()
                        .Where(expression)
                        .Select(propertiSelect)
                        .OrderBy(order).Asc
                        .TransformUsing(new DistinctRootEntityResultTransformer())
                        .Take(take)
                        .List<object[]>();
                return null;
            }
            return
                _session.QueryOver<T>()
                    .Where(expression)
                    .Select(propertiSelect)
                    .TransformUsing(new DistinctRootEntityResultTransformer())
                    .Take(take)
                    .List();
        }

        public override IReponsitoryQuery<T> CreateQuery<T>()
        {
            return new ReponsitoryQuery<T>(_session);
        }

        /// <summary>
        ///     hủy bỏ phiên làm việc
        /// </summary>
        public override void Dispose()
        {
            if (_session != null)
            {
                _session.CancelQuery();
                _session.Dispose();
                _session = null;
            }
        }

        /// <summary>
        ///     commit dữ liệu từ ram sang database
        /// </summary>
        public override void Commit()
        {
            Refresh();
            lock (this) /*lock đối tượng lại không cho các thread khác truy cập*/
            {
                var ssTransaction = _session.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    //_session.Flush();
                    ssTransaction.Commit();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Commit Session Fail : {0}", ex);
                    ssTransaction.Rollback();
                    _session.Clear();
                    throw ex;
                }
                finally
                {
                    ssTransaction.Dispose();
                }
                // _session.Clear();
            }

            LastTimeUsing = DateTime.Now;
        }

        /// <summary>
        ///     The insert.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public override void Insert<T>(T obj)
        {
            Refresh();
            obj.FixNullObject();
            //if (_session.Contains(obj))
            {
                //var newObj = _session.Merge(obj);
                _session.Save(obj);

                //var entity = obj as IEntity;
                //if (entity != null)
                //    entity.FixNullObject();
                //_session.Flush();
            }
            //else
            //{
            //    if (obj is IEntity)
            //        (obj as IEntity).FixNullObject();
            //    _session.Save(obj);
            //}
        }

        ///// <summary>
        /////     The insert all.
        ///// </summary>
        ///// <param name="objs">
        /////     The objs.
        ///// </param>
        ///// <typeparam name="T">
        ///// </typeparam>
        //public override void InsertAll<T>(ICollection<T> objs)
        //{
        //    Refresh();
        //    //_statelessSession.SetBatchSize(objs.Count);
        //    objs.Each(Insert);
        //}


        /// <summary>
        /// Insert tốc độ cao
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        /// <param name="entityName"></param>
        public override void InsertAll<T>(ICollection<T> objs)
        {
            Refresh();

            //foreach (var item in objs)
            //    _session.Save(entityName, item);

            //_session.Flush();
            //_session.Clear();

            //using (var tx = _session.BeginTransaction())
            //{
            //    _session.SetBatchSize(100);
            //    foreach (var item in objs)
            //        _session.Save(item);
            //    tx.Commit();
            //}

            using (IStatelessSession session = _session.SessionFactory.OpenStatelessSession(_session.Connection))
            using (ITransaction transaction = session.BeginTransaction())
            {
                //session.SetBatchSize(100);
                foreach (var item in objs)
                    session.Insert(item);
                transaction.Commit();
            }
        }

        /// <summary>
        ///     The delete.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public override void Delete<T>(T obj)
        {
            Refresh();
            //var newObj = _session.Merge(obj);
            //var types = typeof (T);
            //foreach (var p in types.GetProperties())
            //{
            //    if (p.GetCustomAttribute<HasOneColumnAttribute>() != null)
            //    {
            //        var tmp = p.GetValue(obj);
            //        _session.Delete(_session.Merge(tmp));
            //    }
            //}
            _session.Delete(obj);
        }

        /// <summary>
        ///     The update.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public override T Update<T>(T obj)
        {
            Refresh();

            obj.FixNullObject();
            _session.Update(obj);
            //if (newObj is IEntity)
            //    (newObj as IEntity).FixNullObject();
            //var types = typeof(T);
            ////foreach (var p in types.GetProperties())
            ////{
            ////    if (p.GetCustomAttribute<HasOneColumnAttribute>() != null)
            ////    {
            ////        var x = p.GetValue(newObj);
            ////       // if (_session.Contains(x))
            ////        //{
            ////        //    var tmp = ((IEntity) _session.Merge(x));
            ////        //    tmp.FixNullObject();
            ////        //    _session.SaveOrUpdate(tmp);
            ////        //}
            ////        //else
            ////        //{
            ////        var tmp = ((IEntity)x);
            ////        tmp.FixNullObject();
            ////        _session.SaveOrUpdate(tmp);
            ////        //}
            ////    }
            ////}
            //_session.SaveOrUpdate(newObj);
            return obj;
        }

        public override bool Update<T>(T obj, params Expression<Func<T, object>>[] poperties)
        {
            var dicVal = new Dictionary<string, object>();
            obj.FixNullObject();
            // get table name
            var tabName = "";
            var attr = obj.GetType().GetCustomAttributes(true);
            var allProperty = obj.GetType().GetProperties();

            foreach (var att in attr)
            {
                if (att is ITable)
                {
                    var tab = att as ITable;
                    tabName = tab.Name;
                }
            }
            // not define table , take typename
            if (string.IsNullOrEmpty(tabName))
            {
                tabName = obj.GetType().Name;
            }

            // create set query
            var setQuery = "";
            var stt = 0;
            foreach (var exp in poperties)
            {
                //var tmpex = (UnaryExpression)exp.Body;
                MemberExpression member;
                if (exp.Body is MemberExpression)
                    member = (MemberExpression)exp.Body;
                else if (exp.Body is UnaryExpression)
                    member = (MemberExpression)((UnaryExpression)exp.Body).Operand;
                else
                    continue;
                var mAtrr = member.Member.GetCustomAttributes(true); // get attribute

                // get member name reference in sql table
                var memberName = "";
                foreach (var ma in mAtrr)
                {
                    if (ma is IBasicColumn)
                    {
                        memberName = ((IBasicColumn) ma).Name;
                    }
                    if (ma is IHasOneColumn)
                    {
                        //memberName = ((IHasOneColumn)ma).;
                    }
                }

                if (string.IsNullOrEmpty(memberName))
                    memberName = member
                        .Member.Name;

                var pro = allProperty.FirstOrDefault(m => m.Name == member.Member.Name);

                //pro.GetValue(obj,0);
                setQuery += memberName + "= :" + pro.Name;
                stt++;
                if (stt < poperties.Length)
                    setQuery += " ,";
                dicVal.Add(pro.Name, pro.GetValue(obj));


            }
            if (setQuery == "")
                return false;

            // create where query , always under the key
            var whereQuery = "";
            foreach (var p in allProperty)
            {
                var pAttr = p.GetCustomAttributes(true);
                foreach (var pa in pAttr)
                {
                    if (pa is IPrimaryKeyColumn)
                    {
                        var tmpCl = pa as IPrimaryKeyColumn;
                        {
                            var keyName = string.IsNullOrEmpty(tmpCl.Name) ? p.Name : tmpCl.Name;
                            whereQuery += keyName + "= :" + keyName + ";";
                            dicVal.Add(keyName, p.GetValue(obj));
                            break;
                        }
                    }
                }
                if (whereQuery != "") break;
            }
            if (whereQuery == "")
                return false;

            using (var transaction = _session.BeginTransaction())
            {
                var query =
                    _session.CreateQuery($"Update {tabName} set {setQuery} where {whereQuery}");
                foreach (var para in dicVal)
                    query.SetParameter(para.Key, para.Value);
                query.ExecuteUpdate();
                transaction.Commit();
                transaction.Dispose();
            }
            return true;
        }


        /// <summary>
        ///     The insert or update.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public override void InsertOrUpdate<T>(T obj)
        {
            obj.FixNullObject();
            Refresh();
            //if (obj is IEntity)
            //    (obj as IEntity).FixNullObject();
            //_session.Update(obj);
            _session.SaveOrUpdate(obj);
            //Update(obj);
        }

        /// <summary>
        ///     The get.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        public override T Get<T>(object key)
        {
            Refresh();
            var result = _session.Get<T>(key);
            return result;
        }

        /// <summary>
        ///     The get where.
        /// </summary>
        /// <param name="expression">
        ///     The expression.
        /// </param>
        /// <param name="fetch">
        ///     The fetch.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="ICollection" />.
        /// </returns>
        public override ICollection<T> GetWhere<T>(
            Expression<Func<T, bool>> expression,
            params Expression<Func<T, object>>[] fetch)
        {
            Refresh();
            var query = _session.QueryOver<T>().Where(expression);
            fetch.Each(m => query = query.Fetch(m).Eager);
            query.TransformUsing(new DistinctRootEntityResultTransformer());
            
            return query.List();
        }

        /// <summary>
        ///     The get where.
        /// </summary>
        /// <param name="expression">
        ///     The expression.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="ICollection" />.
        /// </returns>
        public override ICollection<T> GetWhere<T>(Expression<Func<T, bool>> expression)
        {
            Refresh();
            return
                _session.QueryOver<T>()
                    .Where(expression)
                    .TransformUsing(new DistinctRootEntityResultTransformer())
                    .List();
        }

        /// <summary>
        /// lay du lieu theo nhibernate linq
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override ICollection<T> GetWhereSpecial<T>(Expression<Func<T, bool>> expression)
        {
            Refresh();
            return
                _session.Query<T>()
                    .Where(expression)
                    // .TransformUsing(new DistinctRootEntityResultTransformer())
                    .ToList();
        }


        public override IDictionary<Tkey, T> GetAll<Tkey, T>(Func<T, Tkey> key)
        {
            Refresh();
            return
                _session.QueryOver<T>()
                    .TransformUsing(new DistinctRootEntityResultTransformer())
                    .List()
                    .ToDictionary(key);
        }

        public override void Clear()
        {
            Refresh();
            _session.Clear();
        }

        public override void CustomHandle<TSql>(Action<TSql> action)
        {
            if (typeof (TSql) == typeof (ISession))
                action.Invoke((TSql) _session);
        }

        public override IList<T> TakeDest<T>(Expression<Func<T, bool>> expression, int take, Expression<Func<T, object>> order = null)
        {
            Refresh();
            if (order != null)
                return
                    _session.QueryOver<T>()
                        .Where(expression).OrderBy(order).Desc
                        .TransformUsing(new DistinctRootEntityResultTransformer())
                        .Take(take)
                        .List();
            return
                _session.QueryOver<T>()
                    .Where(expression)
                    .TransformUsing(new DistinctRootEntityResultTransformer())
                    .Take(take)
                    .List();
        }

        public override void DeleteWhere<T>(Expression<Func<T, bool>> where)
        {
            DaoDatabase.SqlSupport.ParseExpressionToSql parse = new DaoDatabase.SqlSupport.ParseExpressionToSql();
            String SQL = parse.Parse<T>(where);
            SQL = String.Format("DELETE FROM {0} WHERE {1}", typeof(T).Name, SQL);

            Refresh();
            //IQuery query = _session.CreateQuery(SQL);
            ISQLQuery query = _session.CreateSQLQuery(SQL);
            query.ExecuteUpdate();

            //using (var transaction = _session.BeginTransaction())
            //{
            //    IQuery query = _session.CreateQuery(SQL);
            //    query.ExecuteUpdate();
            //    transaction.Commit();
            //    transaction.Dispose();
            //}
        }
       
    }
}