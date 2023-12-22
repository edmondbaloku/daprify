using CLI.Models;
using FluentAssertions;

namespace CLITests.Assert
{
    public class Asserts
    {

        public static void VerifyBool(bool actual, bool expected)
        {
            actual.Should().Be(expected);
        }
        public static void VerifyNotNull<T>(T item)
        {
            item.Should().NotBeNull();
        }

        public static void VerifyItemCount<T>(IEnumerable<T> items, int expectedCount)
        {
            items.Should().NotBeNull().And.HaveCount(expectedCount);
        }

        public static void VerifyString(string actual, string expected)
        {
            actual.Should().Be(expected);
        }

        public static void VerifyType<T>(object actual)
        {
            actual.Should().BeOfType<T>();
        }

        public static void VerifyEnumerableString(IEnumerable<string> actual, IEnumerable<string> expected)
        {
            actual.Should().BeEquivalentTo(expected);
        }

        public static void VerifyException<T>(Action act) where T : Exception
        {
            act.Should().ThrowExactly<T>();
        }

        public static void VerifyExceptionWithMessage<T>(Action act, string message) where T : Exception
        {
            act.Should().ThrowExactly<T>().WithMessage(message);
        }
    }
}