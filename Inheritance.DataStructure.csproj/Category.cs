using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Inheritance.DataStructure
{
    public class Category : IComparable, IComparable<Category>
    {
        [Order(1)]
        public string ProductName { get; set; }
        [Order(2)]
        public MessageType MessageType { get; set; }
        [Order(3)]
        public MessageTopic MessageTopic { get; set; }

        private static PropertyInfo[] comparableProperties;
        static Category()
        {
            var orderAttribute = typeof(OrderAttribute);
            comparableProperties = typeof(Category).GetProperties()
                .Where(p => Attribute.IsDefined(p, orderAttribute)
                && typeof(IComparable).IsAssignableFrom(p.PropertyType))
                .OrderBy(p => (p.GetCustomAttribute(orderAttribute, false) as OrderAttribute).Order)
                .ToArray();
        }

        public Category(string productName, MessageType messageType, MessageTopic messageTopic)
        {
            ProductName = productName ?? "";
            MessageType = messageType;
            MessageTopic = messageTopic;
        }

        private int CompareHelper(Category other)
        {
            var result = 0;
            foreach (var prop in comparableProperties)
            {
                result = (prop.GetValue(this) as IComparable).CompareTo(prop.GetValue(other));
                if (result != 0) break;
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Category);
        }

        public bool Equals(Category other)
        {
            if (other is null) return false;
            if (object.ReferenceEquals(this, other)) return true;
            return this.GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{ProductName}.{MessageType}.{MessageTopic}";
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as Category);
        }

        public int CompareTo(Category other)
        {
            if (other is null) return -1;
            return CompareHelper(other);
        }

        public static bool operator >=(Category c1, Category c2)
        {
            return c1.CompareTo(c2) >= 0;
        }

        public static bool operator <=(Category c1, Category c2)
        {
            return c1.CompareTo(c2) <= 0;
        }

        public static bool operator >(Category c1, Category c2)
        {
            return c1.CompareTo(c2) > 0;
        }
        public static bool operator <(Category c1, Category c2)
        {
            return c1.CompareTo(c2) < 0;
        }

        public static bool operator ==(Category c1, Category c2)
        {
            return c1.CompareTo(c2) == 0;
        }

        public static bool operator !=(Category c1, Category c2)
        {
            return c1.CompareTo(c2) != 0;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class OrderAttribute : Attribute
    {
        private readonly int order_;
        public OrderAttribute([CallerLineNumber] int order = 0)
        {
            order_ = order;
        }

        public int Order { get { return order_; } }
    }
}
