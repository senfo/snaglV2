using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Berico.Common;
using Berico.SnagL.Model.Attributes;
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Data.Searching
{
    /// <summary>
    /// Provides a description, of a given SearchCriterion control,
    /// that can be used to build a query
    /// </summary>
    public class SearchDescriptor
    {
        /// <summary>
        /// Creates a new instance of SearchDescriptor using
        /// the provided propery values
        /// </summary>
        public SearchDescriptor(string targetField, SearchOperator searchOperator, object value, bool isCaseSensitive)
        {
            TargetField = targetField;
            Operator = searchOperator;
            Value = value;
            IsCaseSensitive = isCaseSensitive;
        }

        /// <summary>
        /// Gets or sets the selected field for this descriptor
        /// </summary>
        public string TargetField { get; private set; }

        /// <summary>
        /// Gets or sets the selected operator for this descriptor
        /// </summary>
        public SearchOperator Operator { get; private set; }

        /// <summary>
        /// Gets or sets the value set for this descriptor
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Gets or sets whether this descriptor is case sensitive or not
        /// </summary>
        public bool IsCaseSensitive { get; private set; }

        /// <summary>
        /// Creates a query expression that can be used with LINQ
        /// </summary>
        /// <returns>a query expression based on the descriptor's properties</returns>
        public Expression<Func<Node, bool>> CreateExpression()
        {
            return BuildExpression();
        }

        /// <summary>
        /// Builds and returns a query expression that can be used 
        /// with LINQ
        /// </summary>
        /// <returns>a query expression based on the descriptor's properties</returns>
        private Expression<Func<Node, bool>> BuildExpression()
        {

            Expression<Func<Node, bool>> queryExpression = null;

            // Build the query based on the currently selected operator
            switch (Operator)
            {
                case SearchOperator.Contains:
                    queryExpression = BuildContainsExpresion(false);
                    break;
                case SearchOperator.DoesNotContain:
                    queryExpression = BuildContainsExpresion(true);
                    break;
                case SearchOperator.StartsWith:
                    queryExpression = BuildStartsWithExpresion(false);
                    break;
                case SearchOperator.DoesNotStartWith:
                    queryExpression = BuildStartsWithExpresion(true);
                    break;                    
                case SearchOperator.EndsWith:
                    queryExpression = BuildEndsWithExpresion(false);
                    break;
                case SearchOperator.DoesNotEndWith:
                    queryExpression = BuildEndsWithExpresion(true);
                    break;
                case SearchOperator.Equals:
                    queryExpression = BuildEqualsExpresion(false);
                    break;
                case SearchOperator.DoesNotEqual:
                    queryExpression = BuildEqualsExpresion(true);
                    break;
                default:
                    // We shouldn't be here so we need to throw an error
                    throw new InvalidOperationException("");
            }

            return queryExpression;
        }

        /// <summary>
        /// Constructs an expression, using the properties of the descriptor,
        /// that checks if the source contains the target.  This expression
        /// can be used in LINQ query.
        /// </summary>
        /// <param name="inverse">Specifies whether the operation should be inverted (NOT)</param>
        /// <returns>an appropriately query expression</returns>
        private Expression<Func<Node, bool>> BuildContainsExpresion(bool invert)
        {

            var attributePredicate = PredicateBuilder.True<Node>();

            // Check if TargetField is empty, if it is, the query
            // should check all attributes
            if (string.IsNullOrEmpty(TargetField))
            {
                // Determine if we are case sensitive or not
                if (IsCaseSensitive)
                {
                    // Determine if this is an inverted (NOT) operation
                    if (invert)
                        attributePredicate = attributePredicate.And(node => !node.Attributes.AttributeValues.Any(attributeValue => attributeValue.Value.Contains(Value as string, StringComparison.InvariantCulture)));
                    else
                        attributePredicate = attributePredicate.And(node => node.Attributes.AttributeValues.Any(attributeValue => attributeValue.Value.Contains(Value as string, StringComparison.InvariantCulture)));
                }
                else
                {
                    // Determine if this is an inverted (NOT) operation
                    if (invert)
                        attributePredicate = attributePredicate.And(node => !node.Attributes.AttributeValues.Any(attributeValue => attributeValue.Value.Contains(Value as string, StringComparison.InvariantCultureIgnoreCase)));
                    else
                    {
                        attributePredicate = attributePredicate.And(node => node.Attributes.AttributeValues.Any(attributeValue => attributeValue.Value.Contains(Value as string, StringComparison.InvariantCultureIgnoreCase)));
                        attributePredicate = attributePredicate.Or(node => node.DisplayValue.Contains(Value as string, StringComparison.InvariantCultureIgnoreCase));
                    }
                }
            }
            else
            {
                // Query to ensure we are focusing on the target attribute
                attributePredicate = attributePredicate.And(node => node.Attributes.ContainsAttribute(TargetField));

                // Determine if we are case sensitive or not
                if (IsCaseSensitive)
                {
                    // Determine if this is an inverted (NOT) operation
                    if (invert)
                        attributePredicate = attributePredicate.And(node => !node.Attributes[TargetField].Value.Contains(Value as string, StringComparison.InvariantCulture));
                    else
                        attributePredicate = attributePredicate.And(node => node.Attributes[TargetField].Value.Contains(Value as string, StringComparison.InvariantCulture));
                }
                else
                {
                    // Determine if this is an inverted (NOT) operation
                    if (invert)
                        attributePredicate = attributePredicate.And(node => !node.Attributes[TargetField].Value.Contains(Value as string, StringComparison.InvariantCultureIgnoreCase));
                    else
                        attributePredicate = attributePredicate.And(node => node.Attributes[TargetField].Value.Contains(Value as string, StringComparison.InvariantCultureIgnoreCase));
                }
            }

            return attributePredicate;
        }

        /// <summary>
        /// Constructs an expression, using the properties of the descriptor,
        /// that checks if the source starts with the target.  This expression
        /// can be used in LINQ query.
        /// </summary>
        /// <param name="inverse">Specifies whether the operation should be inverted (NOT)</param>
        /// <returns>an appropriately query expression</returns>
        private Expression<Func<Node, bool>> BuildStartsWithExpresion(bool invert)
        {

            var attributePredicate = PredicateBuilder.True<Node>();
            
            // Query to ensure we are focusing on the target attribute
            attributePredicate.And(node => node.Attributes.ContainsAttribute(TargetField));

            // Determine if we are case sensitive or not
            if (IsCaseSensitive)
            {
                // Determine if this is an inverted (NOT) operation
                if (invert)
                    attributePredicate = attributePredicate.And(node => !node.Attributes[TargetField].Value.StartsWith(Value as string, StringComparison.InvariantCulture));
                else
                    attributePredicate = attributePredicate.And(node => node.Attributes[TargetField].Value.StartsWith(Value as string, StringComparison.InvariantCulture));
            }
            else
            {
                // Determine if this is an inverted (NOT) operation
                if (invert)
                    attributePredicate = attributePredicate.And(node => !node.Attributes[TargetField].Value.StartsWith(Value as string, StringComparison.InvariantCultureIgnoreCase));
                else
                    attributePredicate = attributePredicate.And(node => node.Attributes[TargetField].Value.StartsWith(Value as string, StringComparison.InvariantCultureIgnoreCase));
            }

            return attributePredicate;
        }

        /// <summary>
        /// Constructs an expression, using the properties of the descriptor,
        /// that checks if the source ends with the target.  This expression
        /// can be used in LINQ query.
        /// </summary>
        /// <param name="inverse">Specifies whether the operation should be inverted (NOT)</param>
        /// <returns>an appropriately query expression</returns>
        private Expression<Func<Node, bool>> BuildEndsWithExpresion(bool invert)
        {
            var attributePredicate = PredicateBuilder.True<Node>();

            // Query to ensure we are focusing on the target attribute
            attributePredicate.And(node => node.Attributes.ContainsAttribute(TargetField));

            // Determine if we are case sensitive or not
            if (IsCaseSensitive)
            {
                // Determine if this is an inverted (NOT) operation
                if (invert)
                    attributePredicate = attributePredicate.And(node => !node.Attributes[TargetField].Value.EndsWith(Value as string, StringComparison.InvariantCulture));
                else
                    attributePredicate = attributePredicate.And(node => node.Attributes[TargetField].Value.EndsWith(Value as string, StringComparison.InvariantCulture));
            }
            else
            {
                // Determine if this is an inverted (NOT) operation
                if (invert)
                    attributePredicate = attributePredicate.And(node => !node.Attributes[TargetField].Value.EndsWith(Value as string, StringComparison.InvariantCultureIgnoreCase));
                else
                    attributePredicate = attributePredicate.And(node => node.Attributes[TargetField].Value.EndsWith(Value as string, StringComparison.InvariantCultureIgnoreCase));
            }

            return attributePredicate;
        }

        /// <summary>
        /// Constructs an expression, using the properties of the descriptor,
        /// that checks if the source and target are equal.  This expression
        /// can be used in LINQ query.
        /// </summary>
        /// <param name="inverse">Specifies whether the operation should be inverted (NOT)</param>
        /// <returns>an appropriately query expression</returns>
        private Expression<Func<Node, bool>> BuildEqualsExpresion(bool invert)
        {
            var attributePredicate = PredicateBuilder.True<Node>();

            // Query to ensure we are focusing on the target attribute
            attributePredicate.And(node => node.Attributes.ContainsAttribute(TargetField));

            // Determine if we are case sensitive or not
            if (IsCaseSensitive)
            {
                // Determine if this is an inverted (NOT) operation
                if (invert)
                    attributePredicate = attributePredicate.And(node => !node.Attributes[TargetField].Value.Equals(Value as string, StringComparison.InvariantCulture));
                else
                    attributePredicate = attributePredicate.And(node => node.Attributes[TargetField].Value.Equals(Value as string, StringComparison.InvariantCulture));
            }
            else
            {
                // Determine if this is an inverted (NOT) operation
                if (invert)
                    attributePredicate = attributePredicate.And(node => !node.Attributes[TargetField].Value.Equals(Value as string, StringComparison.InvariantCultureIgnoreCase));
                else
                    attributePredicate = attributePredicate.And(node => node.Attributes[TargetField].Value.Equals(Value as string, StringComparison.InvariantCultureIgnoreCase));
            }

            return attributePredicate;
        }

    }
}
