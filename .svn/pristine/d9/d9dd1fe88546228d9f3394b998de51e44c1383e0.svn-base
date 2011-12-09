//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Berico.Common
{
    /// <summary>
    /// Extension library that provides the functionality to build
    /// predicate expressions for a where clause
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// Gets a new expression that just returns 'true'.  This is used
        /// as a starting point for building predicates.
        /// </summary>
        /// <typeparam name="T">The data type for the expression</typeparam>
        /// <returns>an expression that evaluates to 'true'</returns>
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        /// <summary>
        /// Gets a new expression that just returns 'false'.  This is used
        /// as a starting point for building predicates.
        /// </summary>
        /// <typeparam name="T">The data type for the expression</typeparam>
        /// <returns>an expression that evaluates to 'false'</returns>
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        /// <summary>
        /// Returns a new expression that combines the two provided expressions
        /// using an 'Or' operator
        /// </summary>
        /// <typeparam name="T">The data type used in the provided expressions</typeparam>
        /// <param name="expr1">The left-hand expression</param>
        /// <param name="expr2">The right-hand expression</param>
        /// <returns>a new 'Or' expression</returns>
        public static Expression<Func<T, bool>> Or<T> (this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// Returns a new expressions that combines the two provided expressions 
        /// using an 'And' operator
        /// </summary>
        /// <typeparam name="T">The data type used in the provided expressions</typeparam>
        /// <param name="expr1">The left-hand expression</param>
        /// <param name="expr2">The right-hand expression</param>
        /// <returns>a new 'And' expression</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }
}