using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.Adhoc.WebControls
{
    /// <summary>CheckBox web control for representing the inclusion of an adhoc aggregate.</summary>
    public class AggregateCheckBox : System.Web.UI.WebControls.CheckBox
    {
        /// <summary>Instantiates a new instance of the AggregateCheckBox class.</summary>
        /// <param name="agg">Adhoc aggregate the CheckBox is for.</param>
        public AggregateCheckBox(AdhocAggregate agg)
        {
            m_aggregate = agg;
        }

        private readonly AdhocAggregate m_aggregate;
        /// <summary>Gets a reference to the backing AdhocAggregate. Read-only field.</summary>
        public AdhocAggregate Aggregate
        {
            get { return m_aggregate; }
        }
    }
}