using justinobney.gymbuddy.api.DependencyResolution;
using justinobney.gymbuddy.api.Enums;
using MediatR;
using NUnit.Framework;
using StructureMap;

namespace justinobney.gymbuddy.api.tests
{
    public class BaseTest
    {
        public IMediator _mediator { get; set; }

        [SetUp]
        public void SetUp()
        {
            var container = new Container(new DefaultRegistry());
            _mediator = container.GetInstance<IMediator>();
        }
    }
}
