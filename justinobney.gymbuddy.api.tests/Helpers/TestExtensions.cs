using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace justinobney.gymbuddy.api.tests.Helpers
{
    public static class TestExtensions
    {
        public static void ShouldBe<T>(this T actual, T expected)
        {
            ShouldBe(actual, expected, string.Empty);
        }

        public static void ShouldBe<T>(this T actual, T expected, string message)
        {
            Assert.AreEqual(expected, actual, message);
        }

        public static void ShouldNotBe<T>(this T actual, T expected)
        {
            ShouldNotBe(actual, expected, string.Empty);
        }

        public static void ShouldNotBe<T>(this T actual, T expected, string message)
        {
            Assert.AreNotEqual(expected, actual, message);
        }

        public static void ShouldContain(this string actual, string expected)
        {
            ShouldContain(actual, expected, string.Empty);
        }

        public static void ShouldContain(this string actual, string expected, string message)
        {
            Assert.IsTrue(actual.Contains(expected), message);
        }

        public static void ShouldNotContain(this string actual, string expected)
        {
            ShouldNotContain(actual, expected, string.Empty);
        }

        public static void ShouldNotContain(this string actual, string expected, string message)
        {
            Assert.IsFalse(actual.Contains(expected), message);
        }

        public static void ShouldBeSameAs<T>(this T actual, T expected) where T : class
        {
            ShouldBeSameAs(actual, expected, string.Empty);
        }

        public static void ShouldBeSameAs<T>(this T actual, T expected, string message) where T : class
        {
            Assert.AreSame(actual, expected, message);
        }

        public static void ShouldNotBeSameAs<T>(this T actual, T expected) where T : class
        {
            ShouldNotBeSameAs(actual, expected, string.Empty);
        }

        public static void ShouldNotBeSameAs<T>(this T actual, T expected, string message) where T : class
        {
            Assert.AreNotSame(actual, expected, message);
        }

        public static void ShouldBeEqual<T>(this T actual, T expected)
        {
            ShouldBeEqual(actual, expected, string.Empty);
        }

        public static void ShouldBeEqual<T>(this T actual, T expected, string message)
        {
            Assert.AreEqual(actual, expected, message);
        }

        public static void ShouldNotBeEqual<T>(this T actual, T expected)
        {
            ShouldNotBeEqual(actual, expected, string.Empty);
        }

        public static void ShouldNotBeEqual<T>(this T actual, T expected, string message)
        {
            Assert.AreNotEqual(actual, expected, message);
        }

        public static void ShouldBeLessThan<T>(this T actual, T expected) where T : IComparable
        {
            ShouldBeLessThan(actual, expected, string.Empty);
        }

        public static void ShouldBeLessThan<T>(this T actual, T expected, string message) where T : IComparable
        {
            Assert.IsTrue(actual.CompareTo(expected) < 0, message);
        }

        public static void ShouldBeGreaterThan<T>(this T actual, T expected) where T : IComparable
        {
            ShouldBeGreaterThan(actual, expected, string.Empty);
        }

        public static void ShouldBeGreaterThan<T>(this T actual, T expected, string message) where T : IComparable
        {
            Assert.IsTrue(actual.CompareTo(expected) > 0, message);
        }

        public static void ShouldBeNull(this object value)
        {
            Assert.IsNull(value);
        }

        public static void ShouldNotBeNull(this object value)
        {
            Assert.IsNotNull(value);
        }

        public static void ShouldThrow<T>(this Action action) where T : Exception
        {
            ShouldThrow<T>(action, string.Empty);
        }

        public static void ShouldThrow<T>(this Action action, string message) where T : Exception
        {
            string failMessage = $"did not throw expected exception {typeof (T).Name}.";

            try
            {
                action();
                Assert.Fail(failMessage);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    Assert.AreEqual(message, ex.Message);
                }

                Assert.AreEqual(typeof(T), ex.GetType(), failMessage);
            }
        }

        public static async Task ShouldThrowAsync<TException>(this Func<Task> func) where TException : class
        {
            await ShouldThrowAsync<TException>(func, exception => { });
        }

        public static async Task ShouldThrowAsync<TException>(Func<Task> func, Action<TException> action) where TException : class
        {
            var exception = default(TException);
            var expected = typeof(TException);
            Type actual = null;
            try
            {
                await func();
            }
            catch (Exception e)
            {
                exception = e as TException;
                actual = e.GetType();
            }

            Assert.AreEqual(expected, actual);
            action(exception);
        }

        public static void ShouldBeEmpty<T>(this IEnumerable<T> list)
        {
            Assert.IsTrue(!list.Any());
        }

        public static void ShouldNotBeEmpty<T>(this IEnumerable<T> list)
        {
            Assert.IsTrue(list.Any());
        }

        public static void ShouldContain<T>(this IEnumerable<T> list, T value)
        {
            Assert.IsTrue(list.Contains(value));
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> list, T value)
        {
            Assert.IsTrue(!list.Contains(value));
        }

        public static void ShouldOnlyContain<T>(this IEnumerable<T> list, params T[] values)
        {
            var toList = list.ToList();

            Assert.AreEqual(toList.Count(), values.Count());

            foreach (var value in values)
            {
                Assert.IsTrue(toList.Contains(value));
            }
        }
    }
}