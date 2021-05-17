﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Conduit.Helpers
{
    public static class OrderByCustom
    {
        public static IQueryable<TEntity> OrderByProperty<TEntity>(this IQueryable<TEntity> source,
                                                            string orderByProperty, bool descending)
        {
            string command = descending ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command,
                                                   new[] { type, property.PropertyType },
                                                   source.AsQueryable().Expression,
                                                   Expression.Quote(orderByExpression));
            return source.AsQueryable().Provider.CreateQuery<TEntity>(resultExpression);
        }
    }
}
