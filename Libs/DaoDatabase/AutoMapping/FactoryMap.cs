using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.IAttribute;
using DaoDatabase.AutoMapping.MapAtribute;
using FluentNHibernate;
using FluentNHibernate.Mapping;
using FluentNHibernate.Utils;
using NHibernate.Criterion;
using Expression = System.Linq.Expressions.Expression;

namespace DaoDatabase.AutoMapping
{
    public class ComponentFactory<T> : ComponentMap<T>
    {
        public ComponentFactory()
        {
            var type = typeof(T);

            // get property
            var property = type.GetProperties();

            // check attribute of property

            property.Each(m =>
            {
                //get attributes
                var atts = m.GetCustomAttributes(true);
                atts.Each(n =>
                {
                    var map = n as IBasicColumn;
                    if (map != null)
                    {

                        //switch (map.Type)
                        {
                            var pe = Expression.Parameter(typeof(T), m.Name);
                            Expression expPro = Expression.Property(pe, m);
                            Expression conversion = Expression.Convert(expPro, typeof(object));
                            var exp = Expression.Lambda<Func<T, object>>(conversion, pe);
                            //case PropertyType.Map:
                            var part = Map(exp,
                                    !string.IsNullOrEmpty(map.Name) ? map.Name : null);
                                if (!string.IsNullOrEmpty(map.CustomSqlType))
                                    part = part.CustomSqlType(map.CustomSqlType);
                                if (map.NotNull)
                                    part.Not.Nullable();
                                //break;
                        }
                    }
                });
            });
        }
    }
    public class FactoryMap<T> : ClassMap<T> where T : class
    {
        public FactoryMap()
        {
            try
            {
                Cache.ReadWrite();
                var type = typeof (T);

                var classAtributes = type.GetCustomAttributes(true);
                foreach (var atribute in classAtributes)
                {
                    if (atribute is ITable)
                    {
                        var table = atribute as ITable;

                        Table(!string.IsNullOrEmpty(table.Name) ? table.Name : type.Name);
                        if (!string.IsNullOrEmpty(table.Schema))
                            Schema(table.Schema);
                    }
                }

                var propertie = type.GetProperties();
                foreach (var propertyInfo in propertie)
                {
                    var allAtribute = propertyInfo.GetCustomAttributes(true);
                    // build 1 lambda expression để fluent nhibernate sử dụng
                    var pe = Expression.Parameter(typeof (T), propertyInfo.Name);
                    Expression expPro = Expression.Property(pe, propertyInfo);
                    Expression conversion = Expression.Convert(expPro, typeof (object));
                    var exp = Expression.Lambda<Func<T, object>>(conversion, pe);

                    foreach (var o in allAtribute)
                    {
                        if (o is IBasicColumn)
                        {
                            var tmp = o as IBasicColumn;
                            if (string.IsNullOrEmpty(tmp.Name))
                                tmp.Name = propertyInfo.Name;
                            if (tmp.Name == null)
                            {
                                Console.WriteLine("aa");
                            }
                            BuildBasicColumn(exp,tmp, this);
                        }
                        else if (o is IPrimaryKeyColumn)
                        {
                            BuildPrimaryColumn(exp, o as IPrimaryKeyColumn, this);
                        }
                        else if (o is IHasManyColumn)
                        {
                            var column = o as IHasManyColumn;
                            BuildHasManyColumn(column, propertyInfo, this);
                        }
                        else if (o is IReferenceColumn)
                        {
                            var column = o as IReferenceColumn;
                            BuildRefernceColumn(column, propertyInfo, this);
                            //References<T>(exp).Column("").Insert().Update();
                        }
                        else if (o is IComponentColumn)
                        {
                            var column = o as IComponentColumn;
                            BuildComponent(column, propertyInfo, this);
                        }
                        else if (o is IHasOneColumn)
                        {
                            var column = o as IHasOneColumn;
                            BuildHasOne(column, propertyInfo, this);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void BuildBasicColumn<TChild>(Expression<Func<TChild, object>> exp, IBasicColumn clmn,
            ClasslikeMapBase<TChild> factory)
        {
            var name = clmn.Name;
            //if(name==null)
            //    return;
            var mapResult = factory.Map(exp);
            if (clmn.NotNull)
                mapResult.Not.Nullable();
            if (!string.IsNullOrEmpty(clmn.Name))
                mapResult.Column(clmn.Name);
            if (!string.IsNullOrEmpty(clmn.CustomSqlType))
                mapResult.CustomSqlType(clmn.CustomSqlType);
            if (!string.IsNullOrEmpty(clmn.Default))
                mapResult.Default(clmn.Default);
            if (clmn.Length > 0)
                mapResult.Length((int)clmn.Length);
            if (clmn.IsIndex)
                mapResult.Index(name);
            mapResult.Not.LazyLoad();
        }

        private void BuildPrimaryColumn<TChild>(Expression<Func<TChild, object>> exp, IPrimaryKeyColumn column,
            ClassMap<TChild> factory)
        {
            var idResult = factory.Id(exp);
            switch (column.KeyGenerateType)
            {
                case KeyGenerateType.Auto:
                    idResult.GeneratedBy.Native();
                    break;

                case KeyGenerateType.Guid:
                    idResult.GeneratedBy.Guid();
                    break;

                case KeyGenerateType.Manual:
                    idResult.GeneratedBy.Assigned();
                    break;

                case KeyGenerateType.Foreign:
                    if (string.IsNullOrEmpty(column.ForeignKey))
                        throw new Exception("thuộc tính khóa 1 -1 không hợp lệ");
                    idResult.GeneratedBy.Foreign(column.ForeignKey);
                    break;
            }
        }

        private void BuildHasManyColumn<TChild>(IHasManyColumn column, PropertyInfo propertyInfo,
            ClassMap<TChild> factory)
        {
            var pe = Expression.Parameter(typeof (TChild), propertyInfo.Name);
            var expPro = Expression.Property(pe, propertyInfo);
            dynamic hResult = null;
            switch (column.Type)
            {
                case HasManyType.Dictionary: //Expression<Func<T,IDictionary<TKey,TChild>>>
                    // var tDic = typeof (IDictionary<,>).MakeGenericType(ptype);
                    var tDelegateDic = typeof (Func<,>).MakeGenericType(typeof (TChild),
                        propertyInfo.PropertyType);
                    dynamic tDicLampda = Expression.Lambda(tDelegateDic, expPro, pe);
                    // {
                    var pKey = Expression.Parameter(column.Child, column.KeyName);
                    Expression expKey = Expression.Property(pKey, column.Child.GetProperty(column.KeyName));

                    var funKey = typeof (Func<,>).MakeGenericType(column.Child,
                        column.Child.GetProperty(column.KeyName).PropertyType);
                    dynamic lambdaKey = Expression.Lambda(funKey, expKey, pKey);
                    //}
                    hResult = factory.HasMany(tDicLampda);
                    hResult.AsMap(lambdaKey);

                    hResult.Inverse()
                        .Generic()
                        //.Cascade.All(); 
                        .Fetch.Join();
                    //.ForeignKeyCascadeOnDelete();
                    break;

                case HasManyType.List: //Expression<Func<T,IEnumerable<TChid>>
                    var tList = typeof (IEnumerable<>).MakeGenericType(column.Child);
                    var tDelegateList = typeof (Func<,>).MakeGenericType(typeof (TChild), tList);
                    dynamic tLampdaList = Expression.Lambda(tDelegateList, expPro, pe);
                    hResult = factory.HasMany(tLampdaList);
                    hResult.AsBag();
                    break;
            }
            if (hResult == null)
                throw new Exception("Không xác định được kiểu Foreign Key!");
            if (!string.IsNullOrEmpty(column.Name))
                hResult.KeyColumn(column.Name);
            if (!string.IsNullOrEmpty(column.ForeignKeyName))
                hResult.ForeignKeyConstraintName(column.ForeignKeyName);
            //if (column.LazyLoad)
            var a = hResult.Not.LazyLoad();
            var px = a.GetType().GetProperties();
            foreach (var p in px)
            {
                if (p.Name == "Cascade")
                {
                    var pv = p.GetValue(a);
                    //hResult = pv.DeleteOrphan();
                    //hResult = pv.Merge();
                    hResult = pv.AllDeleteOrphan();
                    // when an object is save/update/delete, check the associations and save/update/delete all the objects found. 
                    //In additional to that, when an object is removed from the association and not associated with another object (orphaned), 
                    //also delete it.
                    break;
                }
            }

            //HasMany<TChild>(m => m.Equals(1))
            //    .KeyColumn(column.Name).Cascade.AllDeleteOrphan()
            //    .Not.LazyLoad()
            //    .Inverse()
            //    .Generic().Cascade.AllDeleteOrphan()
            //    .Fetch.Join().Cascade.All().Inverse()
            //    .ForeignKeyCascadeOnDelete().AsList(m => m.Column());
        }

        private void BuildRefernceColumn<TChild>(IReferenceColumn column, PropertyInfo propertyInfo,
            ClassMap<TChild> factory)
        {
            var pe = Expression.Parameter(typeof (TChild), propertyInfo.Name);
            var expPro = Expression.Property(pe, propertyInfo);
            var tDelegateRef = typeof (Func<,>).MakeGenericType(typeof (TChild), propertyInfo.PropertyType);
            dynamic tRefLampda = Expression.Lambda(tDelegateRef, expPro, pe);

            var rResult = factory.References(tRefLampda);
            if (!string.IsNullOrEmpty(column.Name))
                rResult.Column(column.Name);
            rResult.Fetch.Join();
            rResult.Not.LazyLoad();//.Cascade.SaveUpdate(); //.NotFound.Ignore();
            //References<object>(null).Cascade.SaveUpdate()
        }

        private void BuildComponent<TChild>(IComponentColumn column, PropertyInfo propertyInfo, ClassMap<TChild> factory)
        {
            // build expression<func<T,Ttype>>
            var agument = Expression.Parameter(typeof(TChild), propertyInfo.Name);
            var express = Expression.Property(agument, propertyInfo);
            //Expression conversion = Expression.Convert(express, typeof(object));
            var cdelegate = typeof(Func<,>).MakeGenericType(typeof(TChild), propertyInfo.PropertyType);
            dynamic lambda = Expression.Lambda(cdelegate, express, agument);
            var member = ReflectionExtensions.ToMember(lambda);
            var ctype = typeof(ComponentPart<>).MakeGenericType(propertyInfo.PropertyType);
            var t = Activator.CreateInstance(ctype, propertyInfo.PropertyType, member);

            factory.Component(t);
            //factory.Component(lambda);
            // build action
            //factory.Component(lambda, x =>
            //{
            var properties = propertyInfo.PropertyType.GetProperties();
            foreach (var info in properties)
            {
                var bsc = info.GetCustomAttribute<BasicColumnAttribute>();
                //foreach (var attr in allAtribute)
                {
                    if (bsc != null)// build các basic column
                    {
                        var tmpArg = Expression.Parameter(propertyInfo.PropertyType, info.Name);
                        var tmpExp = Expression.Property(tmpArg, info);
                        var tmpObject = Expression.Convert(tmpExp, typeof(object));
                        var markType = typeof(Func<,>).MakeGenericType(propertyInfo.PropertyType, typeof(object));
                        dynamic tmpLambda = Expression.Lambda(markType, tmpObject, tmpArg);
                        var mapResult = t.Map(tmpLambda);
                        if (bsc.NotNull)
                            mapResult.Not.Nullable();
                        if (column.Index > 0)
                            mapResult.Column(!string.IsNullOrEmpty(bsc.Name)
                                ? bsc.Name + column.Index
                                : propertyInfo.Name + '_' + info.Name + column.Index);
                        else mapResult.Column(!string.IsNullOrEmpty(bsc.Name) ? bsc.Name : propertyInfo.Name + '_' + info.Name);
                        if (!string.IsNullOrEmpty(bsc.CustomSqlType))
                            mapResult.CustomSqlType(bsc.CustomSqlType);
                        if (!string.IsNullOrEmpty(bsc.Default))
                            mapResult.Default(bsc.Default);
                        if (bsc.IsIndex)
                            mapResult.Index(!string.IsNullOrEmpty(bsc.Name)
                                ? bsc.Name + column.Index
                                : propertyInfo.Name + '_'+info.Name + column.Index);
                    }
                    else
                    {
                        // buil components column
                        var com = info.GetCustomAttribute<ComponentColumnAttribute>();
                        if (com != null)
                        {
                            BuildComponent1(com, info, t);
                        }
                    }
                }
            }
            //});
        }
        private void BuildComponent1<TChild>(IComponentColumn column, PropertyInfo propertyInfo, ComponentPart<TChild> factory)
        {
            // build expression<func<T,Ttype>>
            var agument = Expression.Parameter(typeof (TChild), propertyInfo.Name);
            var express = Expression.Property(agument, propertyInfo);
            //Expression conversion = Expression.Convert(express, typeof(object));
            var cdelegate = typeof (Func<,>).MakeGenericType(typeof (TChild), propertyInfo.PropertyType);
            dynamic lambda = Expression.Lambda(cdelegate, express, agument);
            var member = ReflectionExtensions.ToMember(lambda);
            var ctype = typeof (ComponentPart<>).MakeGenericType(propertyInfo.PropertyType);
            var t = Activator.CreateInstance(ctype, propertyInfo.PropertyType, member);
          
            factory.Component(t);
            //factory.Component(lambda);
            // build action
            //factory.Component(lambda, x =>
            //{
            var properties = propertyInfo.PropertyType.GetProperties();
            foreach (var info in properties)
            {
                var bsc = info.GetCustomAttribute<BasicColumnAttribute>();
                //foreach (var attr in allAtribute)
                {
                    if (bsc != null)// build các basic column
                    {
                        var tmpArg = Expression.Parameter(propertyInfo.PropertyType, info.Name);
                        var tmpExp = Expression.Property(tmpArg, info);
                        var tmpObject = Expression.Convert(tmpExp, typeof (object));
                        var markType = typeof (Func<,>).MakeGenericType(propertyInfo.PropertyType, typeof (object));
                        dynamic tmpLambda = Expression.Lambda(markType, tmpObject, tmpArg);
                        var mapResult = t.Map(tmpLambda);
                        if (bsc.NotNull)
                            mapResult.Not.Nullable();
                        if (column.Index > 0)
                            mapResult.Column(!string.IsNullOrEmpty(bsc.Name)
                                ? bsc.Name + column.Index
                                : propertyInfo.Name + '_'+info.Name + column.Index);
                        else mapResult.Column(!string.IsNullOrEmpty(bsc.Name) ? bsc.Name : propertyInfo.Name + '_' + info.Name);
                        if (!string.IsNullOrEmpty(bsc.CustomSqlType))
                            mapResult.CustomSqlType(bsc.CustomSqlType);
                        if (!string.IsNullOrEmpty(bsc.Default))
                            mapResult.Default(bsc.Default);
                        if (bsc.IsIndex)
                            mapResult.Index(!string.IsNullOrEmpty(bsc.Name)
                                ? bsc.Name + column.Index
                                : propertyInfo.Name + '_'+info.Name + column.Index);
                    }
                    else
                    {
                        // buil components column
                        var com = info.GetCustomAttribute<ComponentColumnAttribute>();
                        if (com != null)
                        {
                            BuildComponent1(com, info, t);
                        }
                    }
                }
            }
            //});
        }

        private void BuildHasOne<TChild>(IHasOneColumn column, PropertyInfo propertyInfo, ClassMap<TChild> factory)
        {
            var argument = Expression.Parameter(typeof (T), propertyInfo.Name);
            var expPro = Expression.Property(argument, propertyInfo);
            var ctype = typeof (Func<,>).MakeGenericType(typeof (T), propertyInfo.PropertyType);
            dynamic lambda = Expression.Lambda(ctype, expPro, argument);
            var result = factory.HasOne(lambda);
            result.Not.LazyLoad();
            if (column.Type == HasOneType.Parent)
                result.Access.CamelCaseField(Prefix.Underscore).Cascade.All();
            else
            {
                result.Constrained().ForeignKey();
            }
            //HasOne<object>(m => m.Equals(1)).Cascade.All();
        }

        //    var result = HasOne(lambda);
        //    dynamic lambda = Expression.Lambda(ctype, expPro, argument);
        //    var ctype = typeof(Func<,>).MakeGenericType(typeof(T), propertyInfo.PropertyType);
        //    var expPro = Expression.Property(argument, propertyInfo);
        //    var argument = Expression.Parameter(typeof(T), propertyInfo.Name);
        //{

        //private void BuilsHasOneChild(object a, PropertyInfo propertyInfo, ClassMap<T> factory)
        //    result.Constrained().ForeignKey();
        //}
    }
}