﻿using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Binding;
using Mediator.Net.Context;
using Mediator.Net.Contracts;

namespace Mediator.Net.Pipeline
{
    class PublishPipe<TContext> : IPublishPipe<TContext> where TContext : IPublishContext<IEvent>
    {
        private readonly IPipeSpecification<TContext> _specification;
        private readonly IDependencyScope _resolver;

        public PublishPipe(IPipeSpecification<TContext> specification, IPipe<TContext> next, IDependencyScope resolver = null)
        {
            Next = next;
            _specification = specification;
            _resolver = resolver;
        }

        public async Task<object> Connect(TContext context, CancellationToken cancellationToken)
        {
            try
            {
                await _specification.BeforeExecute(context, cancellationToken);
                await _specification.Execute(context, cancellationToken);
                if (Next != null)
                {
                    await Next.Connect(context, cancellationToken);
                }
                else
                {
                    if (context.TryGetService(out IMediator mediator))
                    {
                        await mediator.PublishAsync(context.Message, cancellationToken);
                    }
                    else
                    {
                        throw new MediatorIsNotAddedToTheContextException();
                    }
                }

                await _specification.AfterExecute(context, cancellationToken);
            }
            catch (Exception e)
            {
                _specification.OnException(e, context);
            }
            return null;
        }

        public IPipe<TContext> Next { get; }
    }
}