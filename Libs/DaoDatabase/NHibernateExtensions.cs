using NHibernate;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace DaoDatabase
{
    public static class NHibernateExtensions
    {
        public static IList<dynamic> DynamicList(this IQuery query)
        {
            return query.SetResultTransformer(NhTransformers.ExpandoObject)
                        .List<dynamic>();
        }
    }
}
