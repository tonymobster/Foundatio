﻿using System;
using System.Threading.Tasks;

namespace Foundatio.Messaging {
    public interface IMessagePublisher {
        void Publish(Type messageType, object message, TimeSpan? delay = null);
    }

    public interface IMessagePublisher2 {
        Task PublishAsync(Type messageType, object message, TimeSpan? delay = null);
    }

    public static class MessagePublisherExtensions {
        public static void Publish<T>(this IMessagePublisher publisher, T message, TimeSpan? delay = null) where T : class {
            publisher.Publish(typeof(T), message, delay);
        }
    }
}
