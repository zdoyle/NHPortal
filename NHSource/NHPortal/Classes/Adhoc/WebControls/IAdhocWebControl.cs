using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHPortal.Classes.Adhoc.WebControls
{
    /// <summary>Interface for web controls for use with the adhoc builder.</summary>
    public interface IAdhocWebControl
    {
        /// <summary>Gets whether or not the control has input.</summary>
        bool HasInput { get; }

        /// <summary>Gets the input value of the control.</summary>
        string Input { get; }

        /// <summary>Sets the value of the web control.</summary>
        /// <param name="inputValue">Value to set.</param>
        void SetInput(string inputValue);
    }
}
