using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernCsv
{
    public class ModelProperties
    {
        private static readonly ConcurrentDictionary<Type, Lazy<PropertyDescriptorCollection>> _cachedProperties = new ConcurrentDictionary<Type, Lazy<PropertyDescriptorCollection>>();

        public static PropertyDescriptorCollection GetCachedProperties(Type type)
        {
            var lazy = new Lazy<PropertyDescriptorCollection>(() => TypeDescriptor.GetProperties(type));
            var cacheItem = _cachedProperties.GetOrAdd(type, lazy);

            return (cacheItem ?? lazy).Value;
        }

        public static TAttr GetAttribute<TAttr>(PropertyDescriptor propertyDescriptor) where TAttr : Attribute
        {
            return propertyDescriptor.Attributes.OfType<TAttr>().FirstOrDefault();
        }
    }
}
