﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wyam.Common.Documents;
using Wyam.Common.Execution;

namespace Wyam.Common.Configuration
{
    /// <summary>
    /// A union configuration value that can be either a delegate
    /// that uses a context or a simple value. Use the factory methods
    /// in the <see cref="Config"/> class to create one. Instances can also be created
    /// through implicit casting from the value type. Note that due to overload ambiguity,
    /// if a value type of object is used, then all overloads should also be <see cref="ContextConfig{T}"/>.
    /// </summary>
    /// <typeparam name="T">The value type for this config data.</typeparam>
    public class ContextConfig<T> : DocumentConfig<T>
    {
        // Only created by the Config factory methods to ensure matching value types
        internal ContextConfig(Func<IExecutionContext, Task<T>> func)
            : base((_, ctx) => func(ctx))
        {
        }

        // Used the by the casting operators
        internal ContextConfig(Func<IDocument, IExecutionContext, Task<object>> func)
            : base(func)
        {
        }

        public override bool IsDocumentConfig => false;

        public Task<T> GetValueAsync(IExecutionContext context) => GetValueAsync(null, context);

        public static implicit operator ContextConfig<T>(T value) => new ContextConfig<T>(_ => Task.FromResult(value));

        // These special casting operators for object variants ensure we don't accidentally "wrap" an existing ContextConfig/DocumentConfig

        public static implicit operator ContextConfig<IEnumerable<object>>(ContextConfig<T> contextConfig)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                return new ContextConfig<IEnumerable<object>>(async (doc, ctx) => ((IEnumerable)await contextConfig.Delegate(doc, ctx)).Cast<object>());
            }
            return new ContextConfig<IEnumerable<object>>(async (doc, ctx) => new object[] { await contextConfig.Delegate(doc, ctx) });
        }

        public static implicit operator DocumentConfig<IEnumerable<object>>(ContextConfig<T> contextConfig)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                return new ContextConfig<IEnumerable<object>>(async (doc, ctx) => ((IEnumerable)await contextConfig.Delegate(doc, ctx)).Cast<object>());
            }
            return new ContextConfig<IEnumerable<object>>(async (doc, ctx) => new object[] { await contextConfig.Delegate(doc, ctx) });
        }

        public static implicit operator ContextConfig<object>(ContextConfig<T> contextConfig) => new ContextConfig<object>(contextConfig.Delegate);

        public static implicit operator DocumentConfig<object>(ContextConfig<T> contextConfig) => new ContextConfig<object>(contextConfig.Delegate);
    }
}
