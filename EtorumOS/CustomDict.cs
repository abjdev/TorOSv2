using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * <summary>
 *  Only supports types that inherit IComparable
 * </summary>
 */
namespace EtorumOS {
    public class CustomDict<TK, TV> : IEnumerable where TK : IComparable {
        private List<CustomDictEntry<TK, TV>> entries = new();
        public int Count => entries.Count;

        public void Add(TK key, TV value) {
            if (key == null || value == null) {
                return;
            }

            entries.Add(new(key, value));
        }

        public TV Get(TK key) {
            foreach (var entry in entries) {
                if (entry.key.CompareTo(key) == 0) return entry.value;
            }

            throw new NullReferenceException("CustomDict does not include key");
        }

        public void Set(TK key, TV value) {
            foreach (var entry in entries) {
                if (entry.key.CompareTo(key) == 0) {
                    entry.value = value;
                    return;
                }
            }

            entries.Add(new(key, value));
        }

        public bool Remove(TK key) {
            int i = 0;
            foreach (var entry in entries) {
                if (entry.key.CompareTo(key) == 0) {
                    entries.RemoveAt(i);
                    return true;
                }

                i++;
            }

            return false;
        }

        public TV GetAt(int idx) {
            return entries[idx].value;
        }

        public void RemoveAt(int idx) {
            if (idx > entries.Count || idx < 0) throw new ArgumentOutOfRangeException("idx");
            entries.RemoveAt(idx);
        }

        public int IndexOf(TK key) {
            int i = 0;
            foreach (var entry in entries) {
                if (entry.key.CompareTo(key) == 0) {
                    entries.RemoveAt(i);
                    return i;
                }

                i++;
            }

            return -1;
        }

        public bool Contains(TK key) {
            Kernel.Instance.mDebugger.Send("dict1");

            foreach (var entry in entries) {
                Kernel.Instance.mDebugger.Send("dict2");
                Kernel.Instance.mDebugger.Send(entry == null ? "dict_t1" : "dict_f1");
                Kernel.Instance.mDebugger.Send(entry.key == null ? "dict_t2 " + entry.key : "dict_f2 " + entry.key);

                if (entry.key.CompareTo(key) == 0) {
                    Kernel.Instance.mDebugger.Send("dict3");
                    return true;
                }
                Kernel.Instance.mDebugger.Send("dict4");
            }

            Kernel.Instance.mDebugger.Send("dict5");
            return false;
        }

        public bool TryGet(TK key, out TV val) {
            foreach (var entry in entries) {
                if (entry.key.CompareTo(key) == 0) {
                    val = entry.value;
                    return true;
                } else {
                }
            }

            val = default;
            return false;
        }

        public List<TV> AsValueList() {
            return (from entry in entries select entry.value).ToList();
        }

        public List<TK> AsKeyList() {
            return (from entry in entries select entry.key).ToList();
        }

        public IEnumerator GetEnumerator() {
            return entries.GetEnumerator();
        }

        public TV this[TK key] {
            get => Get(key);
            set => Set(key, value);
        }

        public bool IsEntriesNull() {
            return entries == null;
        }
    }

    public class CustomDictEntry<TK, TV> where TK : IComparable {
        public TK key;
        public TV value;

        public CustomDictEntry(TK key, TV value) {
            this.key = key;
            this.value = value;
        }
    }
}