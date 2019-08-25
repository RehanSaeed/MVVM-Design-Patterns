using System;

namespace ReactiveComponentModel
{
    /// <summary>
    /// Determines whether or not an object of type <typeparamref name="T"/> satisfies a rule and
    /// provides an error if it does not.
    /// </summary>
    /// <typeparam name="T">The type of the object the rule can be applied to.</typeparam>
    public sealed class DelegateRule<T> : Rule<T>
    {
        private readonly Func<T, bool> rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateRule&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="propertyName">>The name of the property the rules applies to.</param>
        /// <param name="error">The error if the rules fails.</param>
        /// <param name="rule">The rule to execute.</param>
        public DelegateRule(string propertyName, object error, Func<T, bool> rule)
            : base(propertyName, error) =>
            this.rule = rule ?? throw new ArgumentNullException(nameof(rule));

        /// <summary>
        /// Applies the rule to the specified object.
        /// </summary>
        /// <param name="obj">The object to apply the rule to.</param>
        /// <returns>
        /// <c>true</c> if the object satisfies the rule, otherwise <c>false</c>.
        /// </returns>
        public override bool Apply(T obj) => rule(obj);
    }
}
