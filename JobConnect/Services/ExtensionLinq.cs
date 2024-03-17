using System.Linq.Expressions;

namespace Services
{
    public static class ExtensionLinq
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                propertyName = "id desc";
            }

            var parameterExpression = Expression.Parameter(query.ElementType);
            var split = propertyName.Split(',').ToList();
            foreach (var item in split)
            {
                var proprty = item.Split(' ');

                var proprtyName = proprty[0];
                var direction = proprty[1];

                if (proprtyName.EndsWith("DateString") == true)
                {
                    proprtyName = proprtyName.Replace("DateString", "Date");
                }

                var orderByMethod = "OrderBy";
                if (direction.ToLower() == "desc")
                {
                    orderByMethod = "OrderByDescending";
                }

                if (split.IndexOf(item) > 0)
                {
                    orderByMethod = "ThenBy";
                    if (direction.ToLower() == "desc")
                    {
                        orderByMethod = "ThenByDescending";
                    }
                }

                var memberExpression = Expression.Property(parameterExpression, proprtyName);
                var orderByCall = Expression.Call(typeof(Queryable), orderByMethod,
                new Type[] {
                    query.ElementType,
                    memberExpression.Type
                }
                , query.Expression
                , Expression.Quote(Expression.Lambda(memberExpression, parameterExpression))
                );

                query = query.Provider.CreateQuery(orderByCall) as IQueryable<T>;
            }
            return query;
        }
    }
}
