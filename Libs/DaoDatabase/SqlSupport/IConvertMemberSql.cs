using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace DaoDatabase.SqlSupport
{
    public interface IConvertMemberSql
    {
        string GetColumnName(MemberInfo member);
        Type GetIdReference(Type t);
        string GetTableName(Type t);
        string GetTableNameInsert(Type t);
        string GetKey(Type t);
        T DataRowToObject<T>(DataRow row);
        IList<T> DataTableToListObject<T>(DataTable table);
        IList<T> DataTableJoinToListObject<T>(DataTable table);
        IDictionary<string, Tuple<int,object>> ParseObjectToColumn<T>(T obj);
        IEnumerable<Tuple<string,string, string, string>> GetFullOuterJoin(Type t);
        void UpdateIdObject(object obj, object keyValue);
        IList<string> GetColumnOfTable(Type t);
        IList<string> GetFullColumnOfTableWithRelationShip(Type t);
        PropertyInfo GetIdFromType(Type t);
    }
}