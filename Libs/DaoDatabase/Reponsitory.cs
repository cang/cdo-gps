using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Transform;
using Remotion.Linq.Parsing;

namespace DaoDatabase
{
    public interface IReponsitoryQuery<T> where T : class, IEntity
    {
        /// <summary>
        ///     set điều kiện
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IReponsitoryQuery<T> Where(Expression<Func<T, bool>> expression);
        /// <summary>
        /// điều kiện Or
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IReponsitoryQuery<T> WhereOr(params Expression<Func<T, bool>>[] expression);
        /// <summary>
        /// query đẳng cấp nasa
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="where"></param>
        /// <param name="groupBy"></param>
        /// <returns></returns>
        IList<Tuple<TKey, TResult,TResult>> WhereMinMax<TKey, TResult>(Expression<Func<T, object>> min,
            Expression<Func<T, object>> max, Expression<Func<T, bool>> where, Expression<Func<T, object>> groupBy);
        /// <summary>
        ///     sắp xếp giảm dần
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        IReponsitoryQuery<T> OrderDesc(Expression<Func<T, object>> column);

        /// <summary>
        ///     sắp xếp tăng dần
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        IReponsitoryQuery<T> OrderAsc(Expression<Func<T, object>> column);

        /// <summary>
        ///     chọn số lương row
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        IReponsitoryQuery<T> Take(int count);

        /// <summary>
        ///     chạy câu truy vấn
        /// </summary>
        /// <returns></returns>
        ICollection<T> Execute();

        /// <summary>
        ///     lựa chọn số dòng hiển thị
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        IReponsitoryQuery<T> Select(Expression<Func<T, object>>[] columns, Expression<Func<object>>[] map);

        /// <summary>
        ///     tổng số dòng
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        int Sum(Expression<Func<T, object>> column);

        /// <summary>
        ///     giá trị trung bình
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        double Avg(Expression<Func<T, object>> column);

        /// <summary>
        ///     lấy giá trị nhỏ nhất
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column"></param>
        /// <returns></returns>
        TResult Min<TResult>(Expression<Func<T, object>> column);

        /// <summary>
        ///     lấy giá trị lớn nhất
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column"></param>
        /// <returns></returns>
        TResult Max<TResult>(Expression<Func<T, object>> column);
        /// <summary>
        /// số lượng record
        /// </summary>
        /// <returns></returns>
        int Count();
    }

    internal class ReponsitoryQuery<T> : IReponsitoryQuery<T> where T : class, IEntity
    {
        private bool _customeSelect;
        private IQueryOver<T, T> _query;

        public ReponsitoryQuery(ISession session)
        {
            _query = session.QueryOver<T>();
        }

        #region Implementation of IReponsitoryQuery<T>

        /// <summary>
        ///     set điều kiện
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IReponsitoryQuery<T> Where(Expression<Func<T, bool>> expression)
        {
            _query = _query.Where(expression);
            return this;
        }
        public IReponsitoryQuery<T> WhereOr(params Expression<Func<T, bool>>[] expression)
        {
            if (expression.Length < 1) throw new Exception("WHereOr : Danh sach dieu kien rong");
            if (expression.Length <= 1) return Where(expression[0]);
            BinaryExpression tmp = null;
            for (var i = 1; i < expression.Length; i++)
            {
                tmp = tmp==null ? Expression.OrElse(expression[i - 1].Body, expression[i].Body) : Expression.OrElse(tmp, expression[i].Body);
            }
            if (tmp == null) return this;
            var condition = Expression.Lambda<Func<T, bool>>(tmp,
                expression.First().Parameters);
            return Where(condition);
        }

        public IList<Tuple<TKey, TResult,TResult>> WhereMinMax<TKey, TResult>(Expression<Func<T, object>> min,
            Expression<Func<T, object>> max, Expression<Func<T, bool>> where, Expression<Func<T, object>> groupBy)
        {

            return _query.SelectList(x => x.SelectMin(min).SelectMax(max).SelectGroup(groupBy))
                .Where(where)
                .List<object[]>()
                .Select(m => new Tuple<TKey, TResult, TResult>((TKey) m[2], (TResult) m[0], (TResult) m[1])).ToList();

        }

        /// <summary>
        ///     sắp xếp giảm dần
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public IReponsitoryQuery<T> OrderDesc(Expression<Func<T, object>> column)
        {
            _query = _query.OrderBy(column).Desc;
            return this;
        }

        /// <summary>
        ///     sắp xếp tăng dần
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public IReponsitoryQuery<T> OrderAsc(Expression<Func<T, object>> column)
        {
            _query = _query.OrderBy(column).Asc;
            return this;
        }

        /// <summary>
        ///     chọn số lương row
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IReponsitoryQuery<T> Take(int count)
        {
            _query.Take(count).Cacheable();
            return this;
        }

        /// <summary>
        ///     chạy câu truy vấn
        /// </summary>
        /// <returns></returns>
        public ICollection<T> Execute()
        {
            if (!_customeSelect)
                return _query.TransformUsing(new DistinctRootEntityResultTransformer()).List();
            return _query.TransformUsing(Transformers.AliasToBean<T>()).List();
        }

        /// <summary>
        ///     lựa chọn số dòng hiển thị
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public IReponsitoryQuery<T> Select(Expression<Func<T, object>>[] columns, Expression<Func<object>>[] map)
        {
            _customeSelect = true;
            _query = _query.SelectList(m =>
            {
                for (var i = 0; i < columns.Length; i++)
                {
                    m = m.Select(columns[i]).WithAlias(map[i]);
                }
                return m;
            });
            return this;
        }

        /// <summary>
        ///     tổng số dòng
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public int Sum(Expression<Func<T, object>> column)
        {
            return _query.SelectList(m => m.SelectSum(column)).List<int>().FirstOrDefault();
        }

        /// <summary>
        ///     giá trị trung bình
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public double Avg(Expression<Func<T, object>> column)
        {
            return _query.SelectList(m => m.SelectAvg(column)).List<int>().FirstOrDefault();
        }

        /// <summary>
        ///     lấy giá trị nhỏ nhất
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column"></param>
        /// <returns></returns>
        public TResult Min<TResult>(Expression<Func<T, object>> column)
        {
            return _query.SelectList(m => m.SelectMin(column)).List<TResult>().FirstOrDefault();
        }

        /// <summary>
        ///     lấy giá trị lớn nhất
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column"></param>
        /// <returns></returns>
        public TResult Max<TResult>(Expression<Func<T, object>> column)
        {
            return _query.SelectList(m => m.SelectMax(column)).List<TResult>().FirstOrDefault();
        }

        /// <summary>
        /// số lượng record
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return _query.RowCount();
        }

        #endregion
    }

    public abstract class Reponsitory : IDisposable
    {
        public DateTime LastTimeUsing { get; set; }
        public bool IsUsing { set; get; }

        public string Name { get; protected set; }

        #region Implementation of IDisposable

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public abstract void Dispose();

        #endregion

        public abstract void Commit();


        public abstract void Insert<T>(T obj) where T : class, IEntity;

        public abstract void InsertAll<T>(ICollection<T> objs) where T : class, IEntity;
        //public abstract void InsertAll<T>(String entityName, ICollection<T> objs) where T : class, IEntity;

        public abstract void Delete<T>(T obj) where T : class, IEntity;

        public abstract T Update<T>(T obj) where T : class, IEntity;
        public abstract bool Update<T>(T obj, params Expression<Func<T, object>>[] poperties) where T : class, IEntity;
        //public abstract void Update<T>(T obj,params string[] poperties) where T : class;

        public abstract void InsertOrUpdate<T>(T obj) where T : class, IEntity;


        public abstract T Get<T>(object key) where T : class, IEntity;

        public abstract ICollection<T> GetWhere<T>(Expression<Func<T, bool>> expression,
            params Expression<Func<T, object>>[] fetch) where T : class, IEntity;

        public abstract ICollection<T> GetWhere<T>(Expression<Func<T, bool>> expression) where T : class, IEntity;

        /// <summary>
        ///     lay du lieu theo nhibernate linq
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public abstract ICollection<T> GetWhereSpecial<T>(Expression<Func<T, bool>> expression) where T : class, IEntity;

        public abstract IDictionary<Tkey, T> GetAll<Tkey, T>(Func<T, Tkey> key) where T : class, IEntity;

        public abstract void Clear();

        public abstract void CustomHandle<TSql>(Action<TSql> action);

        public abstract IList<T> TakeDest<T>(Expression<Func<T, bool>> expression,
            int take, Expression<Func<T, object>> order = null) where T : class, IEntity;

        public abstract IList<T> TakeAsc<T>(Expression<Func<T, bool>> expression,
            int take, Expression<Func<T, object>> order = null) where T : class, IEntity;

        public abstract IList<T> TakeDestMinimum<T>(Expression<Func<T, bool>> expression,
            int take, Expression<Func<T, object>> order, params Expression<Func<T, object>>[] propertiSelect)
            where T : class, IEntity;

        public abstract IReponsitoryQuery<T> CreateQuery<T>() where T : class, IEntity;

        public abstract void DeleteWhere<T>(Expression<Func<T, bool>> where) where T : class, IEntity;

    }
}