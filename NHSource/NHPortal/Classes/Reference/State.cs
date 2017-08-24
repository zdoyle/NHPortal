using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.Reference
{
    /// <summary>Used to store and access all known states.</summary>
    public static class States
    {
        /// <summary>Comma delimited string of state abbreviations.</summary>
        public const string STATES = "AL,AK,AZ,AR,CA,CO,CT,DC,DE,FL,GA,HI,ID,IL,IN,IA,KS,KY,LA,ME,MD,MA,MI,MN,MS,MO,MT,NE,NV,NH,NJ,NM,NY,NC,ND,OH,OK,OR,PA,RI,SC,SD,TN,TX,UT,VT,VA,WA,WV,WI,WY";

        private static List<State> m_all;
        /// <summary>Gets an array of all States stored.</summary>
        public static State[] All 
        { 
            get { return m_all.ToArray(); } 
        }

        /// <summaryStatic constructor for the States class.</summary>
        static States()
        {
            Initialize();
        }

        /// <summary>Initializes the array of states in memory.</summary>
        public static void Initialize()
        {
            m_all = new List<State>();
            foreach (string s in STATES.Split(new char[] { ',' }))
            {
                m_all.Add(new State(s, s));
            }
        }
    }

    /// <summary>Represents a state.</summary>
    public class State
    {
        /// <summary>Instantiates a new instance of the State class.</summary>
        /// <param name="stateCode">State abbreviation.</param>
        /// <param name="stateName">Name of the state.</param>
        public State(string stateCode, string stateName)
        {
            m_code = stateCode;
            m_name = stateName;
        }

        private readonly string m_code;
        /// <summary>Gets the abbreviation of the state.  Read-only field.</summary>
        public string Code
        {
            get { return m_code; }
        }

        private readonly string m_name;
        /// <summary>Gets the name of the state.  Read-only field.</summary>
        public string Name
        {
            get { return m_name; }
        }
    }
}