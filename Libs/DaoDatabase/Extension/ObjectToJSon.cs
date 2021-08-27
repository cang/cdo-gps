using System.Collections.Generic;
using System.Reflection;
using DaoDatabase.AutoMapping.MapAtribute;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DaoDatabase.Extension
{
    public static class ObjectToJSon
    {
        public static BsonDocument GetBson<T>(this T obj)
        {
            BsonDocument result=new BsonDocument();
            
            var properties = typeof (T).GetProperties();
            foreach (var propertyInfo in properties)
            {
                var primary = propertyInfo.GetCustomAttribute<PrimaryKeyAttribute>();
                if (primary != null)
                {

                    var name = string.IsNullOrEmpty(primary.Name) ? propertyInfo.Name : primary.Name;
                    result.Add(name, new BsonObjectId(propertyInfo.GetValue(obj).ToString()));
                }
                else
                {
                    var bsColumn = propertyInfo.GetCustomAttribute<BasicColumnAttribute>();
                    if (bsColumn != null)
                    {
                        var name = string.IsNullOrEmpty(bsColumn.Name) ? propertyInfo.Name : bsColumn.Name;
                        result.Add(new Dictionary<string, object> {{name, propertyInfo.GetValue(obj)}});
                    }
                    else
                    {
                        var comColumn = propertyInfo.GetCustomAttribute<ComponentColumnAttribute>();
                        if (comColumn != null)
                        {
                            var child = new BsonDocument();
                            var childProperties = propertyInfo.PropertyType.GetProperties();
                            foreach (var childProperty in childProperties)
                            {
                                var bsChildColumn = propertyInfo.GetCustomAttribute<BasicColumnAttribute>();
                                var name = string.IsNullOrEmpty(bsChildColumn.Name) ? childProperty.Name : bsChildColumn.Name;
                                child.Add(new Dictionary<string, object>
                                {
                                    {name, childProperty.GetValue(propertyInfo.GetValue(obj))}
                                });

                            }
                            result.Add(propertyInfo.Name, child);
                        }

                    }
                }
            }

            return result;
        }
    }
}