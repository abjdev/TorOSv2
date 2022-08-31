using Cosmos.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS {
    public class CustomDictString {
        private List<CustomDictEntry<string, string>> entries = new();
        public int Count => entries.Count;

        public void Add(string key, string value) {
            if (key == null || value == null) {
                return;
            }

            entries.Add(new(key, value));
        }

        public string Get(string key) {
            foreach (var entry in entries) {
                if (entry.key == key) return entry.value;
            }

            throw new NullReferenceException("CustomDict does not include key");
        }

        public void Set(string key, string value) {
            foreach (var entry in entries) {
                if (entry.key == key) {
                    entry.value = value;
                    return;
                }
            }

            entries.Add(new(key, value));
        }

        public bool Remove(string key) {
            int i = 0;
            foreach (var entry in entries) {
                if (entry.key == key) {
                    entries.RemoveAt(i);
                    return true;
                }

                i++;
            }

            return false;
        }

        public string GetAt(int idx) {
            return entries[idx].value;
        }

        public void RemoveAt(int idx) {
            if (idx > entries.Count || idx < 0) throw new ArgumentOutOfRangeException("idx");
            entries.RemoveAt(idx);
        }

        public int IndexOf(string key) {
            int i = 0;
            foreach (var entry in entries) {
                if (entry.key == key) {
                    entries.RemoveAt(i);
                    return i;
                }

                i++;
            }

            return -1;
        }

        public bool Contains(string key) {
            foreach (var entry in entries) {
                if (entry.key == key) {
                    return true;
                }
            }

            return false;
        }

        public bool TryGet(string key, out string val) {
            foreach (var entry in entries) {
                if (entry.key == key) {
                    val = entry.value;
                    return true;
                } else {
                }
            }

            val = default;
            return false;
        }

        public List<string> AsValueList() {
            return (from entry in entries select entry.value).ToList();
        }

        public List<string> AsKeyList() {
            return (from entry in entries select entry.key).ToList();
        }

        public IEnumerator GetEnumerator() {
            return entries.GetEnumerator();
        }

        public string this[string key] {
            get => Get(key);
            set => Set(key, value);
        }

        public bool IsEntriesNull() {
            return entries == null;
        }
    }
}