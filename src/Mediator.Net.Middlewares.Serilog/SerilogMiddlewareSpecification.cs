﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using Serilog;
using Serilog.Events;

namespace Mediator.Net.Middlewares.Serilog
{
    class SerilogMiddlewareSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : IContext<IMessage>
    {
        private readonly ILogger _logger;
        private readonly LogEventLevel _level;
        private readonly Func<bool> _shouldExcute;

        public SerilogMiddlewareSpecification(ILogger logger, LogEventLevel level, Func<bool> shouldExcute )
        {
            _logger = logger;
            _level = level;
            _shouldExcute = shouldExcute;
        }
        public bool ShouldExecute(TContext context, CancellationToken cancellationToken)
        {
            if (_shouldExcute == null)
            {
                return true;
            }
            return _shouldExcute.Invoke();
        }

        public Task BeforeExecute(TContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task Execute(TContext context, CancellationToken cancellationToken)
        {
            if (ShouldExecute(context, cancellationToken))
            {
                switch (_level)
                {
                    case LogEventLevel.Error:
                        _logger.Error("Receive message {@Message}", context.Message);
                        break;
                    case LogEventLevel.Debug:
                        _logger.Debug("Receive message {@Message}", context.Message);
                        break;
                    case LogEventLevel.Fatal:
                        _logger.Fatal("Receive message {@Message}", context.Message);
                        break;
                    case LogEventLevel.Information:
                        _logger.Information("Receive message {@Message}", context.Message);
                        break;
                    case LogEventLevel.Verbose:
                        _logger.Verbose("Receive message {@Message}", context.Message);
                        break;
                    case LogEventLevel.Warning:
                        _logger.Verbose("Receive message {@Message}", context.Message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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
