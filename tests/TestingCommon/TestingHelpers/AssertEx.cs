using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestingCommon.TestingHelpers
{
    public static class AssertEx
    {
        public static void AssertExceptionMessageContains(IEnumerable<string> expectedSubstrings, Exception exception)
        {
            var exceptionTypeName = exception.GetType().Name;
            var exceptionMessages = new StringBuilder();

            foreach (var expectedSubstring in expectedSubstrings)
            {
                if (!exception.Message.Contains(expectedSubstring))
                {
                    exceptionMessages.AppendLine($"A {exceptionTypeName} exception was thrown," +
                        $"however, we were expecting the message to contain:\"{expectedSubstring}\"." +
                        $"The actual message is: {exception.Message}");
                }
            }

            if (exceptionMessages.Length > 0)
            {
                throw new AssertFailedException(exceptionMessages.ToString());
            }
        }
    }
}
