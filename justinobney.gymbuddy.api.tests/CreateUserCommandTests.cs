using System;
using System.Threading.Tasks;
using FluentValidation;
using justinobney.gymbuddy.api.Requests.Users;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests
{
    public class CreateUserCommandTests : BaseTest
    {
        [Test]
        public async Task TestMethod1()
        {
            Func<Task> foo = async () => await Task.Run(() => _mediator.SendAsync(new CreateUserCommand()));
            await foo.ShouldThrowAsync<ValidationException>();
        }
    }
}