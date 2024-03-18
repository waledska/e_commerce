namespace e_commerce.Services
{
    public static class Convertor
    {
        // as the string it will looks like => "1, 2, 3, 4, "
        public static string ListToString(List<int> ids)
        {
            // Converts the list of integers to a string, each number followed by a comma and a space
            return string.Join(", ", ids) + ", ";
        }

        public static List<int> StringToList(string IdsInString)
        {
            if (string.IsNullOrWhiteSpace(IdsInString))
                return new List<int>(); // Returns an empty list if the input is null or whitespace

            // Split the string by comma, remove empty entries, trim whitespace, and convert to int
            var integers = IdsInString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(str => int.TryParse(str.Trim(), out int number) ? number : 0)
                                .ToList();

            return integers;
        }

        public static bool IsValidListOfIntegers(dynamic variable)
        {
            if (variable is List<int>)
            {
                return true; // The variable is a list of integers.
            }
            else
            {
                return false; // The variable is not a list of integers.
            }
        }
    }
}
