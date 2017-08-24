using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.Adhoc
{
    /// <summary>Helper class used to store information about the input controls for the adhoc builder.</summary>
    public class AdhocQueryHelper
    {
        /// <summary>Instantiates a new instance of the AdhocQueryBuilder class.</summary>
        public AdhocQueryHelper()
        {
            SelectFields = new List<string>();
            SelectAggregateFields = new List<string>();
            WhereFields = new List<string>();
            GroupByFields = new List<string>();
        }

        /// <summary>Gets a list of fields to select in the adhoc query.</summary>
        public List<string> SelectFields { get; private set; }

        /// <summary>Gets a list of aggregate fields to select in the adhoc query.</summary>
        public List<string> SelectAggregateFields { get; private set; }

        /// <summary>Gets a list of fields to use as criteria in the adhoc query.</summary>
        public List<string> WhereFields { get; private set; }

        /// <summary>Gets a list of fields to use as the group by clause in the adhoc query.</summary>
        public List<string> GroupByFields { get; private set; }

        /// <summary>Gets a list of all select fields to use in the adhoc query.</summary>
        public string[] SelectStatements
        {
            get
            {
                return SelectFields.Union(SelectAggregateFields).ToArray();
            }
        }
    }
}