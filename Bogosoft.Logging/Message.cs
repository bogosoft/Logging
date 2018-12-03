using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Bogosoft.Logging
{
    /// <summary>
    /// A default implementation of the <see cref="IMessage"/> contract.
    /// </summary>
    public sealed class Message : IMessage
    {
        static readonly object[] Empty = new object[0];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Xor(int lhs, int rhs) => lhs ^ rhs;

        /// <summary>
        /// Compare two messages for equality.
        /// </summary>
        /// <param name="lhs">The left-hand side of the operation.</param>
        /// <param name="rhs">The right-hand side of the operation.</param>
        /// <returns>A value indicating whether or not the two given messages are equal.</returns>
        public static bool operator ==(Message lhs, IMessage rhs)
        {
            return lhs?.CorrelatedId == rhs?.CorrelatedId
                && lhs?.Date == rhs?.Date
                && lhs?.Format == rhs?.Format
                && lhs?.Id == rhs?.Id
                && lhs?.Severity == rhs?.Severity
                && (lhs?.Values ?? Empty).SequenceEqual(rhs?.Values ?? Empty);
        }

        /// <summary>
        /// Compare two messages for inequality.
        /// </summary>
        /// <param name="lhs">The left-hand side of the operation.</param>
        /// <param name="rhs">The right-hand side of the operation.</param>
        /// <returns>A value indicating whether or not the two given messages are not equal.</returns>
        public static bool operator !=(Message lhs, IMessage rhs)
        {
            return lhs?.CorrelatedId != rhs?.CorrelatedId
                || lhs?.Date != rhs?.Date
                || lhs?.Format != rhs?.Format
                || lhs?.Id != rhs?.Id
                || lhs?.Severity != rhs?.Severity
                || !(lhs?.Values ?? Empty).SequenceEqual(rhs?.Values ?? Empty);
        }

        /// <summary>
        /// Convert a given string into a <see cref="Message"/>.
        /// </summary>
        /// <param name="text">A message in the form of a string.</param>
        public static implicit operator Message(string text) => new Message(text);

        /// <summary>
        /// Convert a given <see cref="Message"/> into a string.
        /// </summary>
        /// <param name="message">A message.</param>
        public static implicit operator string(Message message) => message.ToString();

        IEnumerable<object> Components
        {
            get
            {
                yield return CorrelatedId;
                yield return Date;
                yield return Format;
                yield return Id;
                yield return Severity;
                yield return Values;
            }
        }

        /// <summary>
        /// Get or set a message ID to correlate to the current message.
        /// </summary>
        public Guid CorrelatedId { get; set; }

        /// <summary>
        /// Get the date of the current message.
        /// </summary>
        public DateTimeOffset Date { get; private set; }

        /// <summary>
        /// Get the text format of the current message.
        /// </summary>
        public string Format { get; private set; }

        /// <summary>
        /// Get the ID assigned to the current message.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Get the severity of the current message.
        /// </summary>
        public MessageSeverity Severity { get; private set; }

        /// <summary>
        /// Get the format-substitutable values of the current message.
        /// </summary>
        public object[] Values { get; private set; }

        Message()
        {
        }

        /// <summary>
        /// Create a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="format">A text format for the new message.</param>
        /// <param name="values">Format-substitutable values for the new message.</param>
        public Message(string format, params object[] values)
            : this(MessageSeverity.Informational, format, values)
        {
        }

        /// <summary>
        /// Create a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="severity">A level of severity for the new message.</param>
        /// <param name="format">A text format for the new message.</param>
        /// <param name="values">Format-substitutable values for the new message.</param>
        public Message(MessageSeverity severity, string format, params object[] values)
        {
            CorrelatedId = Guid.Empty;
            Date = DateTimeOffset.Now;
            Format = format;
            Id = Guid.NewGuid();
            Severity = severity;
            Values = values;
        }

        /// <summary>
        /// Create an exact copy of the current message.
        /// </summary>
        /// <returns>A new reference to an exact copy of the current message.</returns>
        public IMessage Clone()
        {
            return new Message
            {
                CorrelatedId = CorrelatedId,
                Date = Date,
                Format = Format,
                Id = Id,
                Severity = Severity,
                Values = Values
            };
        }

        object ICloneable.Clone() => Clone();

        /// <summary>
        /// Compare a given message to the current message for equality.
        /// </summary>
        /// <param name="other">Another message to compare to the current message.</param>
        /// <returns>A value indicating whether or not the given message is equal to the current message.</returns>
        public bool Equals(IMessage other)
        {
            return CorrelatedId == other?.CorrelatedId
                && Date == other?.Date
                && Format == other?.Format
                && Id == other?.Id
                && Severity == other?.Severity
                && (Values ?? Empty).SequenceEqual(other?.Values ?? Empty);
        }

        /// <summary>
        /// Compare a given object to the current message for equality.
        /// </summary>
        /// <param name="obj">Another object to compare to the current message.</param>
        /// <returns>A value indicating whether or not the given object is equal to the current message.</returns>
        public override bool Equals(object obj) => obj is IMessage && Equals(obj as IMessage);

        /// <summary>
        /// Compute a hash code against the current message.
        /// </summary>
        /// <returns>A hash code computed against the current message.</returns>
        public override int GetHashCode()
        {
            return Components.Where(o => o != null).Select(x => x.GetHashCode()).Aggregate(0, Xor);
        }

        /// <summary>
        /// Obtain a string representation of the current message.
        /// </summary>
        /// <returns>A string representation of the current message.</returns>
        public override string ToString() => string.Format(Format, Values);
    }
}