using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.Adhoc
{
    /// <summary>Represents an aggregate tag in the adhoc XML file.</summary>
    public class AdhocAggregate
    {
        /// <summary>Instantiates a new instance of the AdhocAggregate class.</summary>
        /// <param name="type">Name of the aggregate.</param>
        public AdhocAggregate(AdhocAggregateType type)
        {
            m_type = type;
        }

        private readonly AdhocAggregateType m_type;
        /// <summary>Type of the aggregate.</summary>
        public AdhocAggregateType Type
        {
            get { return m_type; }
        }

        /// <summary>Returns a string representing the aggregate function for use in an Oracle query.</summary>
        /// <param name="field">Field to pass to the aggregate function.</param>
        /// <returns>Aggregate function string to format.</returns>
        public string GetOracleAggFunction(string field, string colName)
        {
            string aggFunc = String.Empty;
            switch (Type)
            {
                case AdhocAggregateType.Count:
                    aggFunc = "COUNT( {0} ) AS \"{1} Cnt\"";
                    break;
                case AdhocAggregateType.Sum:
                    aggFunc = "SUM( {0} ) AS \"{1} Sum\"";
                    break;
                case AdhocAggregateType.Min:
                    aggFunc = "MIN( {0} ) AS \"{1} Min\"";
                    break;
                case AdhocAggregateType.Max:
                    aggFunc = "MAX( {0} ) AS \"{1} Max\"";
                    break;
                case AdhocAggregateType.Average:
                    aggFunc = "ROUND ( AVG ( {0} ), 2 ) AS \"{1} Avg\"";
                    break;
            }
            return String.Format(aggFunc, field, colName);
        }
    }

    /// <summary>Defines the aggregate types available for the adhoc builder tool.</summary>
    public enum AdhocAggregateType
    {
        /// <summary>Represents the count aggregate.</summary>
        Count,
        /// <summary>Represents the sum aggregate.</summary>
        Sum,
        /// <summary>Represents the min aggregate.</summary>
        Min,
        /// <summary>Represents the max aggregate.</summary>
        Max,
        /// <summary>Represents the average aggregate.</summary>
        Average,
    }
}