﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;

namespace Mediator.Net.TestUtil.Middlewares
{
    public static class NoWorkMiddleware
    {
        public static void UseNoWorkMiddleware<TContext>(this IPipeConfigurator<TContext> configurator)
            where TContext : IContext<IMessage>
        {
            configurator.AddPipeSpecification(new NoWorkMiddlewareSpecification<TContext>());
        }
    }

    public class NoWorkMiddlewareSpecification<TContext> : IPipeSpecification<TContext> 
        where TContext : IContext<IMessage>
    {
        public bool ShouldExecute(TContext context, CancellationToken cancellationToken)
        {
            return true;
        }

        public Task BeforeExecute(TContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task Execute(TContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task AfterExecute(TContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public void OnException(Exception ex, TContext context)
        {
            throw ex;
        }
    }
}