using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DaoDatabase.AutoMapping.MapAtribute;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NHibernate.Engine;

namespace DaoDatabase
{
    public class MongoUnit:Reponsitory
    {
        private MongoDatabase _mongoDatabase;
        //private MongoConfig _config;


        /// <summary>
        /// tạo MongoQuery delete theo key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private IMongoQuery CreatDeleteExpression<T>(T obj)
        {
            /*Tạo 1 đối câu truy vấn theo key */

            var propertyId =
                typeof (T).GetProperties().FirstOrDefault(m => m.GetCustomAttribute<BsonIdAttribute>() != null);
            if (propertyId == null)
                throw new Exception("Đối tượng chưa có khóa chính  : " + typeof (T).Name);
            return CreateExpression<T>(propertyId, propertyId.GetValue(obj));
        }

        /// <summary>
        /// Tạo 1 expression so sánh theo thông tin property và value truyền vào
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private IMongoQuery CreateExpression<T>(PropertyInfo p,object  id)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T), p.Name);// create m=>
            MemberExpression mex = Expression.PropertyOrField(pe, p.Name); // m.Id
            ConstantExpression constOrderId = Expression.Constant(id);// == value
            BinaryExpression filter = Expression.Equal(mex, constOrderId);

            Expression<Func<T, bool>> exprLambda = Expression.Lambda<Func<T, bool>>(filter, pe);// complete m=> m.Id==value
            return Query<T>.Where(exprLambda);
        }

        /// <summary>
        /// tạo IMongoQuery get object theo id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        private IMongoQuery CreateGetByIdExpression<T>(object id)
        {
            var propertyId =
               typeof(T).GetProperties().FirstOrDefault(m => m.GetCustomAttribute<BsonIdAttribute>() != null);
            if (propertyId == null)
                throw new Exception("Đối tượng chưa có khóa chính  : " + typeof(T).Name);
            return CreateExpression<T>(propertyId, id);
        }

        private void CreateIdObject<T>(T obj)
        {
            var propertyId =
             typeof(T).GetProperties().FirstOrDefault(m => m.GetCustomAttribute<BsonIdAttribute>() != null);
            if (propertyId == null)
                throw new Exception("Đối tượng chưa có khóa chính  : " + typeof(T).Name);
            if (propertyId.PropertyType == typeof (ObjectId))
                propertyId.SetValue(obj, ObjectId.GenerateNewId());
            else
                throw new Exception("Invalid Id Type :" + typeof (T).Name);
        }

        //internal override bool Create(string name)
        //{
        //    Name = name;
        //    /*tạo name index cho table*/
        //    foreach (var type in _config.Maps)
        //    {
        //        var tab = type.GetCustomAttribute<TableAttribute>();
        //        if(tab==null) continue;
        //        if (!_mongoDatabase.CollectionExists(tab.Name))
        //            _mongoDatabase.CreateCollection(tab.Name);
        //        var session = _mongoDatabase.GetCollection(tab.Name);

        //        var properties = type.GetProperties();
        //        foreach (var propertyInfo in properties)
        //        {
        //            var bsColumn = propertyInfo.GetCustomAttribute<BasicColumnAttribute>();
        //            if (bsColumn != null && bsColumn.IsIndex)
        //            {
        //                var clName = string.IsNullOrEmpty(bsColumn.Name) ? propertyInfo.Name : bsColumn.Name;
        //                if (!session.IndexExists(clName))
        //                    session.CreateIndex(clName);
        //            }
        //        }

        //       //session.FindAndModify("")
        //    }
        //    return true;
        //}


        /// <summary>
        /// khởi tạo 1 kết nối
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public MongoUnit(MongoDatabase db)
        {
            _mongoDatabase = db;


        }
        public override void Commit()
        {
            //throw new NotImplementedException();
        }

        public override IList<T> TakeAsc<T>(Expression<Func<T, bool>> expression, int take, Expression<Func<T, object>> order = null)
        {
            throw new NotImplementedException();
        }

        public override IList<T> TakeDestMinimum<T>(Expression<Func<T, bool>> expression, int take, Expression<Func<T, object>> order, params Expression<Func<T, object>>[] propertiSelect)
        {
            throw new NotImplementedException();
        }

        public override IReponsitoryQuery<T> CreateQuery<T>()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
           //TODO: không cần dispose session đã mở 
           
        }

        private TableAttribute GetTableInfo<T>()
        {
            return typeof (T).GetCustomAttribute<TableAttribute>();
        }
        public override void Insert<T>(T obj)
        {
           // _mongoDatabase.GetCollection<T>()
            var ss = _mongoDatabase.GetCollection(GetTableInfo<T>().Name);
            CreateIdObject(obj);
            ss.Insert(obj);
        }

        public override void InsertAll<T>(ICollection<T> objs)
        {
            var ss = _mongoDatabase.GetCollection(GetTableInfo<T>().Name);
            foreach (var obj in objs)
            {
                CreateIdObject(obj);
            }
            ss.InsertBatch(objs);
        }

        public override void Delete<T>(T obj)
        {
            var ss = _mongoDatabase.GetCollection(GetTableInfo<T>().Name);
            ss.Remove(CreatDeleteExpression(obj));
        }

        public override T Update<T>(T obj)
        {
            var ss = _mongoDatabase.GetCollection(GetTableInfo<T>().Name);
            ss.Save(obj);
            return obj;
        }

        public override bool Update<T>(T obj, params Expression<Func<T, object>>[] poperties)
        {
            throw new NotImplementedException();
        }

        public override void InsertOrUpdate<T>(T obj)
        {
            Update(obj);
        }

        public override T Get<T>(object key)
        {
            var ss = _mongoDatabase.GetCollection(GetTableInfo<T>().Name);

            return ss.FindOneAs<T>(CreateGetByIdExpression<T>(key));
        }

        public override ICollection<T> GetWhere<T>(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] fetch)
        {
            var ss = _mongoDatabase.GetCollection(GetTableInfo<T>().Name);

            return ss.FindAs<T>(Query<T>.Where(expression)).ToList();

        }

        public override ICollection<T> GetWhere<T>(Expression<Func<T, bool>> expression)
        {
            var ss = _mongoDatabase.GetCollection(GetTableInfo<T>().Name);

            return ss.FindAs<T>(Query<T>.Where(expression)).ToList();
        }

        public override ICollection<T> GetWhereSpecial<T>(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public override IDictionary<Tkey, T> GetAll<Tkey, T>(Func<T, Tkey> key)
        {
            // TODO: không hỗ trợ hàm này
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            
        }

        public override void CustomHandle<TSql>(Action<TSql> action)
        {
            
        }

        public override IList<T> TakeDest<T>(Expression<Func<T, bool>> expression, int take, Expression<Func<T, object>> order = null)
        {
            throw new NotImplementedException();
        }

        public override void DeleteWhere<T>(Expression<Func<T, bool>> where)
        {
            throw new NotImplementedException();
        }
    }
}