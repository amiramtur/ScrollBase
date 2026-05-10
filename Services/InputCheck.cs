using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollBase.Services
{
    public static class InputCheck
    {
        public static bool ContainsBothAlphaAndNumeric(string input)
        {
            if (IsEmptyString(input))
            {
                return false;
            }

            bool containsLetter = input.Any(char.IsLetter);
            bool containsDigit = input.Any(char.IsDigit);

            return containsLetter && containsDigit;
        }

        public static bool IsLetterOrDigitOrEmailSpecials(string input)
        {
            string specialChars = "!\\#$%&'*+-/=?^_`{|}@.";
            return input.All(c => char.IsLetterOrDigit(c) || specialChars.Contains(c));
        }



        // checks text input like username or password
        public static bool IsEmailValid(string email)
        {
            // IsEmptyString(email) ||
            if (!IsLetterOrDigitOrEmailSpecials(email)
                || email.Length < 6) { return false; }

            return true;
        }
        public static bool IsEmptyString(string s)
        {
            if (s != null)
            {
                if (s.Equals(string.Empty) || s.Equals("")) {  return true; }
            }
            return false;
        }

        public static bool IsPasswordValid(string password)
        {
            // empty input
            if (IsEmptyString(password) || !(ContainsBothAlphaAndNumeric(password))
                || password.Length < 6) return false;

            return true;
        }

        public static bool IsLinkValid(string link)
        {
            if (string.IsNullOrWhiteSpace(link)) return false;

            // checks if it's a valid URI structure (http / https)
            return Uri.TryCreate(link, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
