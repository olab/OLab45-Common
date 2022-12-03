using System.Text.RegularExpressions;

namespace OLabWebAPI.Model
{
    public partial class SystemCounterActions
    {
        public bool ApplyFunctionToCounter(SystemCounters targetCounter)
        {
            if (targetCounter.IsValueNumeric())
                return ProcessNumericCounter(targetCounter);
            else
                return ProcessStringCounter(targetCounter);
        }

        private bool ProcessNumericCounter(SystemCounters targetCounter)
        {
            // test for numeric constant function expression
            Regex regex = new Regex("=[-,+]?[0-9]+");
            Match match = regex.Match(Expression);
            if (match.Success)
            {
                if (!decimal.TryParse(match.Value[1..], out decimal newValue))
                    targetCounter.ValueFromString(SystemCounters.NotANumber);
                else
                    targetCounter.ValueFromNumber(newValue);

                return true;
            }

            // test for numeric mathematic function expression
            regex = new Regex("[-,+]?[0-9]+");
            match = regex.Match(Expression);
            if (match.Success)
            {
                decimal orgValue = targetCounter.ValueAsNumber();
                if (!decimal.TryParse(match.Value[1..], out decimal newValue))
                    targetCounter.ValueFromString(SystemCounters.NotANumber);
                else
                    targetCounter.ValueFromNumber(orgValue + newValue);

                return true;

            }

            // test for legacy numeric constant function expression
            regex = new Regex("[0-9]+");
            match = regex.Match(Expression);
            if (match.Success)
            {
                if (!decimal.TryParse(match.Value[1..], out decimal newValue))
                    targetCounter.ValueFromString(SystemCounters.NotANumber);
                else
                    targetCounter.ValueFromNumber(newValue);

                return true;

            }

            return false;

        }

        public bool ProcessStringCounter(SystemCounters targetCounter)
        {
            try
            {
                // test string literal function expression
                Regex regex = new Regex("=[a-z,A-Z,0-9,\\ ]+");
                Match match = regex.Match(Expression);

                if (match.Success)
                {
                    string orgValue = targetCounter.ValueAsString();
                    string newValue = match.Value[1..];
                    if (orgValue != newValue)
                        targetCounter.ValueFromString(newValue);
                }

                return match.Success;
            }
            catch (System.Exception)
            {
                targetCounter.ValueFromString(SystemCounters.NotANumber);
            }

            return false;
        }



    }
}
