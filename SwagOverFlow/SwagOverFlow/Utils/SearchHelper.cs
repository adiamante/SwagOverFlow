using System;
using System.Text.RegularExpressions;

namespace SwagOverFlow.Utils
{
    public enum SearchMode
    {
        Instant,
        Delayed
    }

    public enum FilterMode
    {
        CONTAINS,
        EQUALS,
        STARTS_WITH,
        ENDS_WITH,
        BETWEEN_INCLUSIVE,
        BETWEEN_EXCLUSIVE,
        GREATER_THAN,
        LESS_THAN,
        GREATER_THAN_OR_EQUAL_TO,
        LESS_THAN_OR_EQUAL_TO,
        REGULAR_EXPRESSSION
    }

    public static class SearchHelper
    {
        public static bool Evaluate(string compareTarget, string compareValue, Boolean isCaseSensitive, FilterMode filterMode, Boolean bDefault)
        {
            String compareTargetResolved = isCaseSensitive ? compareTarget : compareTarget.ToLower();
            String compareValueResolved = isCaseSensitive ? compareValue : compareValue.ToLower();
            Boolean bPass = bDefault;

            switch (filterMode)
            {
                default:
                case FilterMode.CONTAINS:
                    bPass = compareTargetResolved.Contains(compareValueResolved);
                    break;
                case FilterMode.EQUALS:
                    bPass = compareTargetResolved == compareValueResolved;
                    break;
                case FilterMode.STARTS_WITH:
                    bPass = compareTargetResolved.StartsWith(compareValueResolved);
                    break;
                case FilterMode.ENDS_WITH:
                    bPass = compareTargetResolved.EndsWith(compareValueResolved);
                    break;
                case FilterMode.REGULAR_EXPRESSSION:
                    try
                    {
                        bPass = Regex.IsMatch(compareTarget, compareValue, isCaseSensitive == true ? RegexOptions.None : RegexOptions.IgnoreCase);
                    }
                    catch
                    {
                        bPass = true;
                    }
                    break;
            }

            return bPass;
        }
    }
}
