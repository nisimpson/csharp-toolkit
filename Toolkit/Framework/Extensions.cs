// Copyright(c) 2018 Nathan Simpson
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nerdshoe
{
    public static class Extensions
    {
        public static async Task GetDataFromUrl(this object obj,
            string url,
            Func<MemoryStream, Task> action,
            Func<Exception, Task> error = null)
        {
            byte[] image;
            HttpClient http = new HttpClient {
                Timeout = TimeSpan.FromSeconds(30)
            };

            try {
                image = await http.GetByteArrayAsync(url);
            } catch (Exception ex) {
                await error?.Invoke(ex);
                image = null;
            }

            if (image != null) {
                MemoryStream stream = new MemoryStream(image);
                await action(stream);
            }
        }

        public static bool EqualsStrict<T>(this T obj, object rhs)
            where T : class, IEquatable<T>
        {
            if (rhs is null) return false;
            if (ReferenceEquals(obj, rhs)) return true;
            if (rhs.GetType() != obj.GetType()) return false;
            return obj.Equals(rhs as T);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dict,
            TKey key,
            TValue defaultValue = default(TValue))
        {
            if (!dict.TryGetValue(key, out TValue result)) {
                result = defaultValue;
            }
            return result;
        }

        public static void AddOrUpdate<TKey, TValue>(
            this IDictionary<TKey, TValue> dict,
            TKey key,
            TValue value)
        {
            if (dict.ContainsKey(key)) {
                dict[key] = value;
            } else {
                dict.Add(key, value);
            }
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (rng == null) throw new ArgumentNullException(nameof(rng));

            return source.OrderBy(_ => rng.NextDouble());
        }

        public static T Next<T>(this IEnumerable<T> items)
        {
            return items.Shuffle().Take(1).FirstOrDefault();
        }

        public static bool FlipCoin(this Random rng, bool heads)
        {
            bool result = rng.Next(0, 2) == 0;
            return heads && result;
        }
    }
}
