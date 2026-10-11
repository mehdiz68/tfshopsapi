using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TfShop.Infrastructure.Search
{
    /// <summary>
    /// واژگانِ ایندکس + اصلاحِ املا.
    ///
    /// چرا لازمه با اینکه Meilisearch خودش غلط‌یاب داره: غلط‌یابِ Meili بر پایه‌ی Levenshtein
    /// ساده‌ست و برای کلماتِ کوتاه (که توی فارسی زیادن) سخت‌گیره. مثلاً «تارز» به‌جای «تراز» -
    /// یه جابه‌جاییِ مجاورِ ساده - با Levenshtein دو خطاست و روی کلمه‌ی ۴ حرفی رد می‌شه، ولی با
    /// Damerau یک خطاست. این لایه فقط وقتی فعال می‌شه که Meili هیچ نتیجه‌ای نده، پس نه کند
    /// می‌کنه نه روی نتایجِ درست اثر می‌ذاره.
    ///
    /// واژگان از خودِ عنوان‌های ایندکس‌شده ساخته می‌شه (حدود ۱۵۰۰ واژه برای این فروشگاه)، توی
    /// حافظه کش می‌شه و هر بار که ایندکس بازسازی بشه تازه می‌شه.
    /// </summary>
    public static class SearchVocabulary
    {
        // واژه -> تعداد تکرار. تکرار برای انتخاب بینِ چند کاندیدِ هم‌فاصله استفاده می‌شه:
        // واژه‌ای که توی کاتالوگ رایج‌تره، احتمالِ بیشتری داره منظورِ کاربر بوده باشه.
        private static Dictionary<string, int> _vocab = new Dictionary<string, int>(StringComparer.Ordinal);
        private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

        public static int Count
        {
            get
            {
                Lock.EnterReadLock();
                try { return _vocab.Count; }
                finally { Lock.ExitReadLock(); }
            }
        }

        /// <summary>موقعِ بازسازیِ ایندکس صدا زده می‌شه.</summary>
        public static void Rebuild(IEnumerable<string> normalizedTexts)
        {
            var fresh = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var text in normalizedTexts)
            {
                if (string.IsNullOrEmpty(text)) continue;
                foreach (var w in text.Split(' '))
                {
                    // واژه‌های خیلی کوتاه («و»، «با») نه ارزشِ اصلاح دارن نه ارزشِ جستجو
                    if (w.Length < 3) continue;
                    int c;
                    fresh[w] = fresh.TryGetValue(w, out c) ? c + 1 : 1;
                }
            }

            Lock.EnterWriteLock();
            try { _vocab = fresh; }
            finally { Lock.ExitWriteLock(); }
        }

        /// <summary>
        /// اصلاحِ یک عبارت. هر واژه که توی واژگان نباشه با نزدیک‌ترین کاندید جایگزین می‌شه.
        /// اگه هیچ واژه‌ای عوض نشد، null برمی‌گرده تا صدازننده بی‌خود دوباره کوئری نزنه.
        /// </summary>
        public static string Correct(string normalizedQuery)
        {
            if (string.IsNullOrWhiteSpace(normalizedQuery)) return null;

            Dictionary<string, int> vocab;
            Lock.EnterReadLock();
            try { vocab = _vocab; }
            finally { Lock.ExitReadLock(); }

            if (vocab.Count == 0) return null;

            var tokens = normalizedQuery.Split(' ');
            var fixedTokens = new string[tokens.Length];
            bool changed = false;

            for (int i = 0; i < tokens.Length; i++)
            {
                string t = tokens[i];
                if (t.Length < 3 || vocab.ContainsKey(t))
                {
                    fixedTokens[i] = t;
                    continue;
                }

                // سقفِ خطا با طولِ واژه رشد می‌کنه؛ روی واژه‌ی کوتاه دو خطا یعنی هر چیزی
                // می‌تونه کاندید بشه.
                int maxd = t.Length <= 4 ? 1 : 2;
                string best = t;
                int bestDist = maxd + 1;
                int bestFreq = 0;

                foreach (var kv in vocab)
                {
                    if (Math.Abs(kv.Key.Length - t.Length) > maxd) continue;
                    int d = Damerau(t, kv.Key, maxd);
                    if (d > maxd) continue;
                    if (d < bestDist || (d == bestDist && kv.Value > bestFreq))
                    {
                        best = kv.Key;
                        bestDist = d;
                        bestFreq = kv.Value;
                    }
                }

                if (best != t) changed = true;
                fixedTokens[i] = best;
            }

            return changed ? string.Join(" ", fixedTokens) : null;
        }

        /// <summary>
        /// فاصله‌ی Damerau-Levenshtein با سقف (جابه‌جاییِ مجاور = یک خطا).
        /// وقتی کلِ یک سطر از سقف رد بشه زودتر برمی‌گرده - روی ۱۵۰۰ واژه این کوتاه‌سازی
        /// تفاوتِ محسوسی می‌سازه.
        /// </summary>
        private static int Damerau(string a, string b, int maxd)
        {
            int la = a.Length, lb = b.Length;
            if (Math.Abs(la - lb) > maxd) return maxd + 1;

            int[] prev2 = null;
            int[] prev = new int[lb + 1];
            for (int j = 0; j <= lb; j++) prev[j] = j;

            for (int i = 1; i <= la; i++)
            {
                var cur = new int[lb + 1];
                cur[0] = i;
                int rowBest = cur[0];

                for (int j = 1; j <= lb; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    int val = Math.Min(Math.Min(cur[j - 1] + 1, prev[j] + 1), prev[j - 1] + cost);

                    if (i > 1 && j > 1 && a[i - 1] == b[j - 2] && a[i - 2] == b[j - 1])
                        val = Math.Min(val, prev2[j - 2] + 1);

                    cur[j] = val;
                    if (val < rowBest) rowBest = val;
                }

                if (rowBest > maxd) return maxd + 1;
                prev2 = prev;
                prev = cur;
            }

            return prev[lb];
        }
    }
}
