using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpubReaderDemo.Utils
{
    public static class Extensions
    {
        public static Dictionary<T, K> AddRange<T, K>(this Dictionary<T, K> source, Dictionary<T, K> toAdd)
        {
            if (source != null)
                toAdd.ToList().ForEach(x =>
                {
                    if (!source.ContainsKey(x.Key))
                        source.Add(x.Key, x.Value);
                });
            else
                return toAdd;
            return null;
        }
        public static ObservableCollection<T> AddRange<T>(this ObservableCollection<T> source, List<T> toAdd)
        {
            if (source != null)
                toAdd.ForEach(x => source.Add(x));
            else
                source = new ObservableCollection<T>(toAdd);
            return source;
        }
    }
}
