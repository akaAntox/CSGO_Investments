using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace InvestmentApp.Converters
{
    public static class ObservableCollectionConverter
    {
        public static IEnumerable<TValue> Convert<TValue>(ObservableCollection<TValue> observableCollection) where TValue : class
        {
            return observableCollection.AsEnumerable();
        }

        public static ObservableCollection<TValue> ConvertIEnumerable<TValue>(IEnumerable<TValue> enumerable) where TValue : class
        {
            return new ObservableCollection<TValue>(enumerable);
        }
    }
}
