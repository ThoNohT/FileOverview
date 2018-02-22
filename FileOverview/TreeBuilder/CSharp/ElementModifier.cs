
namespace ThoNohT.FileOverview.TreeBuilder.CSharp
{
    using System;

    /// <summary>
    /// Enumeration listing the possible access modifiers for elements.
    /// </summary>
    [Flags]
    public enum ElementModifier
    {
        /// <summary>
        /// No modifiers.
        /// </summary>
        None = 0,

        /// <summary>
        /// Public access.
        /// </summary>
        Public = 1 << 0,

        /// <summary>
        /// Private access to the type itself.
        /// </summary>
        Private = 1 << 1,

        /// <summary>
        /// Internal access to the namespace.
        /// </summary>
        Internal = 1 << 2,

        /// <summary>
        /// Protected access to the type and its subtypes.
        /// </summary>
        Protected = 1 << 3,

        /// <summary>
        /// Class is intended to be a base class of other classes.
        /// </summary>
        Abstract = 1 << 4,

        /// <summary>
        /// Asynchronous method.
        /// </summary>
        Async = 1 << 5,

        /// <summary>
        /// Cannot be modified.
        /// </summary>
        Const = 1 << 6,

        /// <summary>
        /// An event.
        /// </summary>
        Event = 1 << 7,

        /// <summary>
        /// Method is implemented extenally.
        /// </summary>
        Extern = 1 << 8,

        /// <summary>
        /// Overrides an implementation from a base class.
        /// </summary>
        Override = 1 << 9,

        /// <summary>
        /// Hides a member from a base class.
        /// </summary>
        New = 1 << 10,

        /// <summary>
        /// The element is partial.
        /// </summary>
        Partial = 1 << 11,

        /// <summary>
        /// Field can only be assigned values as part of the declaration or in a constructor in the same class.
        /// </summary>
        Readonly = 1 << 12,

        /// <summary>
        /// Class cannot be inherited.
        /// </summary>
        Sealed = 1 << 13,

        /// <summary>
        /// The element belongs to the type rather to an instance of the type.
        /// </summary>
        Static = 1 << 14,

        /// <summary>
        /// Unsafe context.
        /// </summary>
        Unsafe = 1 << 15,

        /// <summary>
        /// Implementation can be overridden in a derived class.
        /// </summary>
        Virtual = 1 << 16,

        /// <summary>
        /// Field can be modified in the program by something such as the operating system, the hardware, or a
        /// concurrently executing thread.
        /// </summary>
        Volatile = 1 << 17,
    }
}