﻿using System;

namespace NtFreX.Audio.Wasapi.Interop
{
    internal struct PropertyKey
    {

        /// <summary>
        /// The null key
        /// </summary>
        public static PropertyKey NullKey = new PropertyKey(Guid.Empty, 0);

        /// <summary>
        /// The unified device property model format identifier
        /// </summary>
        public readonly Guid FormatId;

        /// <summary>
        /// The unified device property model property identifier
        /// </summary>
        public readonly int PropertyId;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyKey"/>
        /// </summary>
        /// <param name="formatId">The format identifier.</param>
        /// <param name="propertyId">The property identifier.</param>
        public PropertyKey(Guid formatId, int propertyId)
        {
            FormatId = formatId;
            PropertyId = propertyId;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => FormatId.ToString("D") + "-" + PropertyId;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is PropertyKey))
                return false;

            var other = (PropertyKey)obj;
            return other.PropertyId == PropertyId &&
                  Equals(other.FormatId, FormatId);
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return FormatId.GetHashCode() ^ PropertyId;
        }
    }
}
