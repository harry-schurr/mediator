﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Mediator.Net.Autofac.Test.Middlewares;
using Mediator.Net.TestUtil;
using Mediator.Net.TestUtil.Handlers.RequestHandlers;
using Mediator.Net.TestUtil.Messages;
using Mediator.Net.TestUtil.Services;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace Mediator.Net.Autofac.Test.Tests
{
    public class TestAllPipelines : TestBase
    {
        private IContainer _container = null;
        private IMediator _mediator;
        
        void GivenAMediatorBuildConnectsToAllPipelines()
        {
            base.ClearBinding();
            var mediaBuilder = new MediatorBuilder();
            mediaBuilder.RegisterUnduplicatedHandlers()
                .ConfigureGlobalReceivePipe(global =>
                {
                    global.UseSimpleMiddleware1();
                })
                .ConfigureCommandReceivePipe(x =>
                {
                    x.UseSimpleMiddleware1();
                })
                .ConfigureEventReceivePipe(e =>
                {
                    e.UseSimpleMiddleware1();
                })
                .ConfigurePublishPipe(p =>
                {
                    p.UseSimpleMiddleware1();
                }).ConfigureRequestPipe(r =>
                {
                    r.UseSimpleMiddleware1();
                });
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<SimpleService>().AsSelf();
            containerBuilder.RegisterType<AnotherSimpleService>().AsSelf();
            containerBuilder.RegisterMediator(mediaBuilder);
            _container = containerBuilder.Build();
        }

        async Task WhenAMessageIsSent()
        {
            _mediator = _container.Resolve<IMediator>();
            await _mediator.SendAsync(new SimpleCommand(Guid.NewGuid()));
            await _mediator.PublishAsync(new SimpleEvent(Guid.NewGuid()));
            await _mediator.RequestAsync<SimpleRequest, SimpleResponse>(new SimpleRequest("Hello"));
        }

        void ThenAllMiddlewaresInPipelinesShouldBeExecuted()
        {
            RubishBin.Rublish.Count.ShouldBe(6);
        }

        [Fact]
        public void Run()
        {
            this.BDDfy();
        }
    }
}
