using System;

namespace JPAssets.Unity
{
    /// <summary>
    /// A collection of utility methods that make it easy to cast an object to an interface.
    /// </summary>
    public static class InterfaceProxyUtilities
    {
        /// <exception cref="ArgumentException">Type <typeparamref name="TOut"/> is not an interface type.</exception>
        private static void CheckInterface<TOut>()
        {
            if (!typeof(TOut).IsInterface)
                throw new ArgumentException("The generic argument must be an interface type.");
        }

        /// <summary>
        /// Attempts to cast the source object to the given interface type.
        /// </summary>
        /// <typeparam name="TIn">
        /// The type of source object.
        /// </typeparam>
        /// <typeparam name="TOut">
        /// The desired interface type to cast to.
        /// </typeparam>
        /// <param name="source">
        /// The source object to be cast.
        /// </param>
        /// <param name="result">
        /// The source object as an instance of type <typeparamref name="TOut"/> if successful; otherwise, <see langword="null"/>.
        /// </param>
        /// <inheritdoc cref="CheckInterface{T}"/>
        /// <returns>
        /// <see langword="true"/> if cast succeeded; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryCast<TIn, TOut>(TIn source, out TOut result)
            where TOut : class
        {
            CheckInterface<TOut>();

            if (source is TOut castToInterfaceType)
            {
                result = castToInterfaceType;
                return true;
            }

            result = null;
            return false;
        }

        /// <remarks>
        /// This overload allows the method to be used without the need to explicitly
        /// declare two generic arguments. If you need to cast a struct to an interface,
        /// use the overload with two generic arguments and explicitly declare them to avoid
        /// needless boxing.
        /// </remarks>
        /// <inheritdoc cref="TryCast{TIn, TOut}(TIn, out TOut)"/>
        public static bool TryCast<TOut>(object source, out TOut result)
            where TOut : class
        {
            return TryCast<object, TOut>(source, out result);
        }

        /// <summary>
        /// Casts the source object to the given interface type.
        /// </summary>
        /// <returns>
        /// The source object as an instance of type <typeparamref name="TOut"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The source object is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The source object does not implement interface <typeparamref name="TOut"/>.
        /// </exception>
        /// <inheritdoc cref="TryCast{TIn, TOut}(TIn, out TOut)"/>
        /// <inheritdoc cref="CheckInterface{T}"/>
        public static TOut Cast<TIn, TOut>(TIn source)
            where TOut : class
        {
            CheckInterface<TOut>();

            if (!typeof(TIn).IsValueType && ReferenceEquals((object)source, null))
                throw new ArgumentNullException(nameof(source));

            if (source is TOut result)
                return result;

            throw new ArgumentException($"Unable to cast object of type {source.GetType()} to interface type {typeof(TOut)}.", nameof(source));
        }

        /// <remarks>
        /// This overload allows the method to be used without the need to explicitly
        /// declare two generic arguments. If you need to cast a struct to an interface,
        /// use the overload with two generic arguments and explicitly declare them to avoid
        /// needless boxing.
        /// </remarks>
        /// <inheritdoc cref="Cast{TIn, TOut}(TIn)"/>
        public static TOut Cast<TOut>(object source)
            where TOut : class
        {
            return Cast<object, TOut>(source);
        }
    }
}
