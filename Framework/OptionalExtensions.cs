﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework
{
    public static class OptionalExtensions
    {
        public static Optional<K> Map<T, K>(this Optional<T> maybe, Func<T, K> transformer)
        {
            if (maybe.HasValue)
            {
                return Optional<K>.From(transformer(maybe.Value));
            }

            return Optional<K>.None;
        }
        
        public static K Unwrap<T, K>(this Optional<T> maybe, Func<T, K> transformer, Func<K> defaultValueProvider)
        {
            if (maybe.HasValue)
            {
                return transformer(maybe.Value);
            }

            return defaultValueProvider();
        }
        
        public static Optional<K> FlatMap<T, K>(this Optional<T> maybe, Func<T, Optional<K>> transformer)
        {
            if (maybe.HasValue)
            {
                return transformer(maybe.Value);
            }

            return Optional<K>.None;
        }

        public static Optional<T> OptionalFirst<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.FirstOrDefault();
        }
    }
}