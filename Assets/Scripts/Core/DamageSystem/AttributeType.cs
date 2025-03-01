using System;

namespace Minesweeper.Core.DamageSystem
{
    /// <summary>
    /// Represents a type of attribute for an entity.
    /// </summary>
    public class AttributeType : IEquatable<AttributeType>
    {
        /// <summary>
        /// The name of the attribute type.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates a new attribute type with the specified name.
        /// </summary>
        /// <param name="name">The name of the attribute type.</param>
        public AttributeType(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Attribute type name cannot be null or empty", nameof(name));
            }
            
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AttributeType);
        }

        public bool Equals(AttributeType other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Name.ToLowerInvariant().GetHashCode();
        }

        public static bool operator ==(AttributeType left, AttributeType right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(AttributeType left, AttributeType right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return Name;
        }
    }
} 