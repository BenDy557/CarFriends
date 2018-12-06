using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnibusEvent;

namespace UnibusEvent
{
    public delegate void OnEvent<T>(T action);

    public delegate void OnEvent();

    public delegate void OnEventWrapper(object _object);

    public delegate void OnNoParamEventWrapper();

    public struct DictionaryKey
    {
        public Type Type;
        public object Tag;

        public DictionaryKey(string tag, Type type)
        {
            Tag = tag;
            Type = type;
        }

        public override int GetHashCode()
        {
            return Tag.GetHashCode() ^ (Type == null ? 1 : Type.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is DictionaryKey)
            {
                DictionaryKey key = (DictionaryKey)obj;
                return Tag.Equals(key.Tag) && ((Type == null && key.Type == null) || Type == key.Type);
            }

            return false;
        }

        public override string ToString()
        {
            return Tag + ", " + Type;
        }
    }

    public class UnibusObject : SingletonMonoBehaviour<UnibusObject>
    {
        public const string DefaultTag = "default";

        private Dictionary<DictionaryKey, Dictionary<int, OnEventWrapper>> observerDictionary = new Dictionary<DictionaryKey, Dictionary<int, OnEventWrapper>>(new UnibusKeyComparer());

        private Dictionary<DictionaryKey, Dictionary<int, OnNoParamEventWrapper>> observerDictionaryNoParam = new Dictionary<DictionaryKey, Dictionary<int, OnNoParamEventWrapper>>(new UnibusKeyComparer());

        public void Subscribe<T>(OnEvent<T> eventCallback)
        {
            Subscribe(DefaultTag, eventCallback);
        }

        public void Subscribe(OnEvent eventCallback)
        {
            Subscribe(DefaultTag, eventCallback);
        }

        public void Subscribe<T>(string tag, OnEvent<T> eventCallback)
        {
            DictionaryKey key = new DictionaryKey(tag, typeof(T));

            if (!observerDictionary.ContainsKey(key))
            {
                observerDictionary[key] = new Dictionary<int, OnEventWrapper>();
            }

            if (observerDictionary[key].ContainsKey(eventCallback.GetHashCode()))
            {
                //Debug.LogError("Unibus::Subscribe Already added this callback: " + eventCallback.Target + " - " + eventCallback.Method);
                return;
            }

            observerDictionary[key][eventCallback.GetHashCode()] = (object _object) => { eventCallback((T)_object); };
        }

        public void Subscribe(string tag, OnEvent eventCallback)
        {
            DictionaryKey key = new DictionaryKey(tag, null);

            if (!observerDictionaryNoParam.ContainsKey(key))
            {
                observerDictionaryNoParam[key] = new Dictionary<int, OnNoParamEventWrapper>();
            }

            if (observerDictionaryNoParam[key].ContainsKey(eventCallback.GetHashCode()))
            {
                //Debug.LogError("Unibus::Subscribe Already added this callback: " + eventCallback.Target + " - " + eventCallback.Method);
                return;
            }

            observerDictionaryNoParam[key][eventCallback.GetHashCode()] = () => { eventCallback(); };
        }

        public void Unsubscribe<T>(OnEvent<T> eventCallback)
        {
            Unsubscribe(DefaultTag, eventCallback);
        }

        public void Unsubscribe(OnEvent eventCallback)
        {
            Unsubscribe(DefaultTag, eventCallback);
        }

        public void Unsubscribe<T>(string tag, OnEvent<T> eventCallback)
        {
            DictionaryKey key = new DictionaryKey(tag, typeof(T));

            if (observerDictionary[key] != null)
                observerDictionary[key].Remove(eventCallback.GetHashCode());
        }

        public void Unsubscribe(string tag, OnEvent eventCallback)
        {
            DictionaryKey key = new DictionaryKey(tag, null);

            if (observerDictionary.ContainsKey(key) && observerDictionary[key] != null)
                observerDictionary[key].Remove(eventCallback.GetHashCode());

            if (observerDictionaryNoParam.ContainsKey(key) && observerDictionaryNoParam[key] != null)
                observerDictionaryNoParam[key].Remove(eventCallback.GetHashCode());
        }

        public void Dispatch<T>(T action)
        {
            Dispatch(DefaultTag, action);
        }

        public void Dispatch(string tag)
        {
            DictionaryKey key = new DictionaryKey(tag, null);

            if (observerDictionaryNoParam.ContainsKey(key))
            {
                // Q: Why use ToList()? Why not just iterate through the ValueCollection?
                // A: We need to create a copy of the collection because some unibus events
                // will indirectly lead to modification of the dictionary. This will lead to an out of
                // sync error.

                List<OnNoParamEventWrapper> list = observerDictionaryNoParam[key].Values.ToList();

                for (int i = list.Count - 1; i >= 0; --i)
                    list[i]();
            }
        }

        public void Dispatch<T>(string tag, T action)
        {
            DictionaryKey key = new DictionaryKey(tag, typeof(T));

            if (observerDictionary.ContainsKey(key))
            {
                // Q: Why use ToList()? Why not just iterate through the ValueCollection?
                // A: We need to create a copy of the collection because some unibus events
                // will indirectly lead to modification of the dictionary. This will lead to an out of
                // sync error.

                List<OnEventWrapper> list = observerDictionary[key].Values.ToList();

                for (int i = list.Count - 1; i >= 0; --i)
                    list[i](action);
            }
        }

        public void ClearEvents()
        {
            observerDictionary.Clear();
            observerDictionaryNoParam.Clear();
        }
    }
}

public struct UnibusKeyComparer : IEqualityComparer<DictionaryKey>
{
    bool IEqualityComparer<DictionaryKey>.Equals(DictionaryKey x, DictionaryKey y)
    {
        return x.GetHashCode() == y.GetHashCode();
    }

    int IEqualityComparer<DictionaryKey>.GetHashCode(DictionaryKey obj)
    {
        return obj.GetHashCode();
    }
}