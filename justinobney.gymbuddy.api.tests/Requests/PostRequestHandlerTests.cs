using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;
using NSubstitute;
using NUnit.Framework;
using StructureMap;

namespace justinobney.gymbuddy.api.tests.Requests
{
    [TestFixture]
    public class PostRequestHandlerTests : BaseTest
    {
        [Test]
        public void MediatorShouldNotifyAllRegisteredNotifiersForCommand()
        {
            var notifier = Substitute.For<IPostRequestHandler<FakeCommand, bool>>();
            Context.Container.Configure(container =>
            {
                container.AddRegistry(new FakeRegistry(notifier));
            });

            var request = new FakeCommand();
            var appt = Mediator.Send(request);

            notifier.Received(1).Notify(request,appt);
            SetUp();
        }

        public class FakeCommand : IRequest<bool>
        {    
        }

        [DoNotValidate]
        public class FakeCommandHandler : IRequestHandler<FakeCommand,bool>
        {
            public bool Handle(FakeCommand message)
            {
                return true;
            }
        }

        public class FakeRegistry : Registry
        {
            public FakeRegistry(IPostRequestHandler<FakeCommand, bool> notifier)
            {
                For<IRequestHandler<FakeCommand, bool>>().Use<FakeCommandHandler>();
                For<IPostRequestHandler<FakeCommand, bool>>().Use(notifier);
            }
        }
    }
}