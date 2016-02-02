// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using AutoMapper;
using FluentValidation;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace justinobney.gymbuddy.api.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        #region Constructors and Destructors

        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.AssemblyContainingType(typeof (LoggingHandler<,>));
                    scan.WithDefaultConventions();


                    scan.AddAllTypesOf(typeof (IRequestHandler<,>));
                    scan.AddAllTypesOf(typeof (IAsyncRequestHandler<,>));
                    scan.AddAllTypesOf(typeof (INotificationHandler<>));
                    scan.AddAllTypesOf(typeof (IAsyncNotificationHandler<>));

                    scan.AddAllTypesOf(typeof (IAuthorizer<>));
                    scan.AddAllTypesOf(typeof (AbstractValidator<>));
                    scan.AddAllTypesOf(typeof (BaseRepository<>));

                    var handlerType = For(typeof (IRequestHandler<,>));
                    handlerType.DecorateAllWith(typeof (ValidateHandler<,>), DoesNotHaveAttribute(typeof (DoNotValidate)));
                    handlerType.DecorateAllWith(typeof (AuthorizeHandler<,>), HasAttribute(typeof (Authorize)));
                    handlerType.DecorateAllWith(typeof (LoggingHandler<,>), DoesNotHaveAttribute(typeof (DoNotLog)));

                    var asyncHandlerType = For(typeof (IAsyncRequestHandler<,>));
                    asyncHandlerType.DecorateAllWith(typeof(ValidateHandlerAsync<,>), DoesNotHaveAttribute(typeof(DoNotValidate)));
                    asyncHandlerType.DecorateAllWith(typeof (AuthorizeHandlerAsync<,>), HasAttribute(typeof (Authorize)));
                    asyncHandlerType.DecorateAllWith(typeof (LoggingHandlerAsync<,>), DoesNotHaveAttribute(typeof (DoNotLog)));

                });

            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();

            MappingConfig.Register();
            For<IMapper>().Use(_ => MappingConfig.Instance);
        }

        private static Func<Instance, bool> DoesNotHaveAttribute(Type attr)
        {
            return instance => !ContainsAttribute(attr, instance);
        }

        private static Func<Instance, bool> HasAttribute(Type attr)
        {
            return instance => ContainsAttribute(attr, instance);
        }

        private static bool ContainsAttribute(Type attr, Instance instance)
        {
            var type = instance.ReturnedType ?? instance.GetType();
            return type.GetCustomAttribute(attr, false) != null;
        }

        #endregion
    }
}