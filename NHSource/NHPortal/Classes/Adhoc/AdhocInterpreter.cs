using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.Adhoc
{
    /// <summary>Class used for parsing and interpreting text input into a 'where' clause for the adhoc builder tool.</summary>
    public class AdhocInterpreter
    {
        private const string NOT_OPERATOR = "!";
        private const string WILDCARD = "*";
        private const string BETWEEN_OPERATOR = "-";
        private const string DATE_FORMAT = "YYYYMMDD";
        private const string TIME_FORMAT = "HHMMSS";

        private readonly string[] m_equalityOperators;
        private readonly AdhocField m_field;

        /// <summary>Instantiates a new instance of the AdhocInterpreter class.</summary>
        public AdhocInterpreter(AdhocField field)
        {
            m_equalityOperators = new string[] { "<", ">", "<=", ">=" };
            m_field = field;
        }

        /// <summary>Interprets the value entered by the user into a where clause for an adhoc query.</summary>
        /// <param name="text">Text to interpret.</param>
        /// <returns>Where clause generated for the provided text.</returns>
        public string Interpret(string text)
        {
            List<string> parts = new List<string>();
            if (!String.IsNullOrEmpty(text))
            {
                string[] textParts = text.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < textParts.Length; i++)
                {
                    // Pad station id values with 0 if it doesnt contain a wildcard.
                    if (m_field.DatabaseField == "STATIONID" && GDCoreUtilities.StringUtilities.ContainsDigitsOnly(textParts[i]))
                    {
                        textParts[i] = textParts[i].PadLeft(8, '0');
                    }
                    else if (m_field.DatabaseField == "STICKERNUMBER" && GDCoreUtilities.StringUtilities.ContainsDigitsOnly(textParts[i]))
                    {
                        textParts[i] = textParts[i].PadLeft(10, '0');
                    }
                    parts.Add(ProcessTextPart(textParts[i].Trim()));
                }
            }

            string interpreted = String.Join(" OR ", parts);
            return interpreted;
        }

        private string ProcessTextPart(string textPart)
        {
            bool not = PopNotOperator(ref textPart);
            string op = PopEqualityOperator(ref textPart);

            string qry = "";
            if (textPart.Contains(BETWEEN_OPERATOR))
            {
                qry = GenerateBetweenClause(textPart);
            }
            else
            {
                qry = GenerateClause(textPart, op);
            }

            if (not && !String.IsNullOrEmpty(qry))
            {
                qry = String.Format("NOT ( {0} )", qry);
            }
            return qry;
        }

        private string GenerateClause(string textPart, string op)
        {
            string format = "{0} {1} {2}"; // FIELD, OPERATOR, VALUE ex. VIN >= 11111

            if (textPart.Contains(WILDCARD))
            {
                op = "LIKE";
            }

            string value = GetValueForQuery(textPart);
            return String.Format(format, m_field.DatabaseField, op, value);
        }

        private string GenerateBetweenClause(string values)
        {
            string format = "{0} BETWEEN {1} AND {2}"; // FIELD, VALUEONE, VALUETWO

            string qry = String.Empty;
            string[] subValues = values.Split(new string[] { BETWEEN_OPERATOR }, StringSplitOptions.RemoveEmptyEntries);
            if (subValues.Length == 2)
            {
                string valOne = GetValueForQuery(subValues[0]);
                string valTwo = GetValueForQuery(subValues[1]);

                qry = String.Format(format, m_field.DatabaseField, valOne, valTwo);
            }
            else
            {
                // TODO: throw interpret error ??
                // Or just process parts 1 and 2 ?
            }
            return qry;
        }

        private bool PopNotOperator(ref string text)
        {
            bool hasNot = false;
            if (text.StartsWith(NOT_OPERATOR))
            {
                hasNot = true;
                text = text.TrimStart(NOT_OPERATOR.ToCharArray());
            }
            return hasNot;
        }

        private string PopEqualityOperator(ref string text)
        {
            string op = "=";
            foreach (string eop in m_equalityOperators)
            {
                if (text.StartsWith(eop))
                {
                    op = eop;
                }
            }

            text = text.TrimStart(op.ToCharArray());
            return op;
        }

        //private void ReplaceWildcard(ref string text)
        //{
        //    if (text.EndsWith(WILDCARD))
        //    {
        //        text = GDCoreUtilities.StringUtilities.ReplaceEnd(text, WILDCARD, "%");
        //    }
        //}

        private string GetValueForQuery(string text)
        {
            string val = text.Replace(WILDCARD, "%");
            if (m_field.DatabaseFieldType != AdhocDatabaseFieldType.Number)
            {
                val = String.Format("'{0}'", val);
            }
            return val;
        }
    }
}