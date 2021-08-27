using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DaoDatabase.SqlSupport
{
    public class ParseExpressionToSql
    {
        //private IConvertMemberSql _convert;
        //public ParseExpressionToSql(IConvertMemberSql convert)
        //{
        //    _convert = convert;
        //}

        private string GetMemberName(MemberInfo member)
        {
            // return _convert.GetColumnName(member);
            return member.Name;
        }

        private string ParseExpression(Expression ex,string tName)
        {

            if ((ex as BinaryExpression) == null)
                throw new Exception($"type doesn't support: {ex.Type} -- {ex.GetType()}");

            var b = ex as BinaryExpression;


            if (b.NodeType == ExpressionType.Add ||
                b.NodeType == ExpressionType.Divide ||
                b.NodeType == ExpressionType.Subtract ||
                b.NodeType == ExpressionType.Multiply
                )
            {
                var op = "";
                switch (b.NodeType)
                {
                    case ExpressionType.Add:
                        op = "+";
                        break;
                    case ExpressionType.Divide:
                        op = "/";
                        break;
                    case ExpressionType.Subtract:
                        op = "-";
                        break;
                    case ExpressionType.Multiply:
                        op = "*";
                        break;
                }
                var c1 = b.Left as MemberExpression;

                var d1 = b.Right as ConstantExpression;
                return $"({GetMemberName(c1.Member)}  {op} {d1.Value})";
            }

            if (b.NodeType != ExpressionType.GreaterThan &&
                b.NodeType != ExpressionType.GreaterThanOrEqual &&
                b.NodeType != ExpressionType.Equal &&
                b.NodeType != ExpressionType.NotEqual &&
                b.NodeType != ExpressionType.LessThan &&
                b.NodeType != ExpressionType.LessThanOrEqual)
            {
                var op = "";
                switch (b.NodeType)
                {
                    case ExpressionType.AndAlso:
                        op = "and";
                        break;
                    case ExpressionType.OrElse:
                        op = "or";
                        break;
                }
                return $"{ParseExpression(b.Left, tName)} {op} {ParseExpression(b.Right, tName)}";
            }
            var c = b.Left as MemberExpression;
            
            object value = null;
            if (b.Right is MethodCallExpression)
            {
                var tt = b.Right as MethodCallExpression;
                value = Expression.Lambda(tt).Compile().DynamicInvoke();
                
            }

            if (b.Right is ConstantExpression)
            {
                var tt = b.Right as ConstantExpression;
                value = tt.Value;
            }
            if (b.Right is MemberExpression)
            {
                var tt = b.Right as MemberExpression;
                value=Expression.Lambda(tt).Compile().DynamicInvoke();
            }
            var tta = b.Right.GetType();
            var ope = "";
            switch (ex.NodeType)
            {
                case ExpressionType.GreaterThan:
                    ope = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    ope = ">=";
                    break;
                case ExpressionType.Equal:
                    ope = "=";
                    break;
                case ExpressionType.NotEqual:
                    var type = c.Member.Module.GetTypes();
                    //if (_convert.GetIdReference(type.FirstOrDefault(m=>m.Name==c.Member.Name)) == typeof(Guid))
                    if ( (type.FirstOrDefault(m => m.Name == c.Member.Name)) == typeof(Guid))
                        ope = "Is not";
                    else
                        ope = "!=";
                    break;
                case ExpressionType.LessThan:
                    ope = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    ope = "<=";
                    break;
            }

            return string.Format("( {3}.{0} {1} {2})", GetMemberName(c.Member, c), ope,
                    ConvertToString(value), tName);
        }

        private bool IsNumber(object value)
        {
            double num;
            if (!double.TryParse(value.ToString(), out num))
                return false;
            return true;
        }

        private string GetMemberName(MemberInfo member, MemberExpression c)
        {
            if (c.Expression.NodeType != ExpressionType.MemberAccess)
            {
                return GetMemberName(member);
            }
            var mmc = c.Expression as MemberExpression;
            if (mmc == null) throw new NullReferenceException();

            //return _convert.GetColumnName(mmc.Member); 
            return  mmc.Member.Name + "_" + c.Member.Name;
        }

        public string Parse<T>(Expression<Func<T, bool>> where)
        {
            return ParseExpression(where.Body, typeof (T).Name );
        }

        private string ConvertToString(object val)
        {
            if (val == null) return "NULL";
            else if (IsNumber(val))
                return val.ToString();
            else if (val is DateTime)
                return $"'{((DateTime)val).ToString("MM/dd/yyyy HH:mm:ss")}'";
            else
                return $"'{ val.ToString()}'";
        }

    }
}