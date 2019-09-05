﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mediator.Net.Context;
using Mediator.Net.Contracts;


namespace Mediator.Net.Pipeline
{
    public class EventReceivePipeConfigurator : IEventReceivePipeConfigurator
    {
        private readonly IDependencyScope _resolver;
        private readonly IList<IPipeSpecification<IReceiveContext<IEvent>>> _specifications;

        public IDependencyScope DependencyScope => _resolver;

        public EventReceivePipeConfigurator(IDependencyScope resolver = null)
        {
            _resolver = resolver;
            _specifications = new List<IPipeSpecification<IReceiveContext<IEvent>>>();
        }

        public void AddPipeSpecification(IPipeSpecification<IReceiveContext<IEvent>> specification)
        {
            _specifications.Add(specification);
        }

        public IEventReceivePipe<IReceiveContext<IEvent>> Build()
        {
            IEventReceivePipe<IReceiveContext<IEvent>> current = null;
            if (_specifications.Any())
            {
                for (int i = _specifications.Count - 1; i >= 0; i--)
                {
                    current = i == _specifications.Count - 1
                        ? new EventReceivePipe<IReceiveContext<IEvent>>(_specifications[i], null, _resolver)
                        : new EventReceivePipe<IReceiveContext<IEvent>>(_specifications[i], current, _resolver);
                }
            }
            else
            {
                current = new EventReceivePipe<IReceiveContext<IEvent>>(new EmptyPipeSpecification<IReceiveContext<IEvent>>(), null, _resolver);
            }

            return current;
        }
    }
}