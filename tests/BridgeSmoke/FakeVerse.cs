using System;
using System.Collections.Generic;
using System.Reflection;

namespace Verse
{
    // Test doubles ONLY. Never copy these types into Aquacultura/ or a release.
    public enum LoadSaveMode { Inactive, Saving, LoadingVars, PostLoadInit }
    public enum LookMode { Deep }
    public interface IExposable { void ExposeData(); }
    public class CompProperties { public Type compClass; }
    public class ThingComp
    {
        public CompProperties props;
        public virtual void CompTickRare() { }
        public virtual void PostExposeData() { }
        public virtual string CompInspectStringExtra() { return null; }
    }

    public static class Scribe
    {
        public static LoadSaveMode mode;
        public static Dictionary<string, object> values = new Dictionary<string, object>();
        public static void Save()
        {
            values = new Dictionary<string, object>();
            mode = LoadSaveMode.Saving;
        }
        public static void Load() { mode = LoadSaveMode.LoadingVars; }
        public static void Finish() { mode = LoadSaveMode.PostLoadInit; }
        public static void Stop() { mode = LoadSaveMode.Inactive; }
    }
    public static class Scribe_Values
    {
        public static void Look<T>(ref T value, string name, T defaultValue)
        {
            if (Scribe.mode == LoadSaveMode.Saving) Scribe.values[name] = value;
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                object raw;
                value = Scribe.values.TryGetValue(name, out raw) ? (T)raw : defaultValue;
            }
        }
    }
    public static class Scribe_Collections
    {
        private static List<T> DeepClone<T>(List<T> source) where T : new()
        {
            if (source == null) return null;
            var clone = new List<T>();
            foreach (var entry in source)
            {
                if (object.Equals(entry, null)) { clone.Add(default(T)); continue; }
                var item = new T();
                foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
                    field.SetValue(item, field.GetValue(entry));
                clone.Add(item);
            }
            return clone;
        }
        public static void Look<T>(ref List<T> list, string name, LookMode mode) where T : new()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
                Scribe.values[name] = DeepClone(list);
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                object value;
                list = Scribe.values.TryGetValue(name, out value)
                    ? DeepClone((List<T>)value) : null;
            }
        }
    }
}