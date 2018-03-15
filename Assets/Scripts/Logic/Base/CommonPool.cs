using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nexus.Logic.Base
{
	public class NestedCommonPool<TKey, TBase>
	{
		private Dictionary<TKey, Stack<TBase>> _Nested = new Dictionary<TKey, Stack<TBase>>();

		public T Get<T>(TKey key) where T : TBase, new()
		{
			Stack<TBase> pool = null;
			if (!_Nested.TryGetValue(key, out pool))
			{
				pool = new Stack<TBase>();
				_Nested.Add(key, pool);

			}

			if (pool.Count == 0)
			{
				return new T();
			}
			else
			{
				return (T)pool.Pop();
			}
		}

		public void Return(TKey key, TBase value)
		{
			if (value == null)
				return;

			Stack<TBase> pool = null;
			if (!_Nested.TryGetValue(key, out pool))
			{
				pool = new Stack<TBase>();
				_Nested.Add(key, pool);
			}

			pool.Push(value);

#if UNITY_EDITOR
			if (pool.Count > 30)
				Debug.Log(string.Format("{0} has {1} items", key, pool.Count));
#endif
		}

		public void Destroy()
		{
			foreach (var pool in _Nested)
			{
				pool.Value.Clear();
			}
			_Nested.Clear();
			_Nested = null;
		}
	}

	public class CommonPool<T> where T : new()
	{
		public delegate T CreateDelegate();
		private Stack<T> _Pool = new Stack<T>();

		private CreateDelegate _CreateDelegate;

		public CommonPool()
		{
			_CreateDelegate = null;
		}

		public CommonPool(CreateDelegate func)
		{
			_CreateDelegate = func;
		}

		public T Get()
		{
			T ret = default(T);
			if (_Pool.Count > 0)
			{
				ret = _Pool.Pop();
			}
			else
			{
				if (_CreateDelegate != null)
					ret = _CreateDelegate();
				else
					ret = new T();
			}
			return ret;
		}

		public void Return(T item)
		{
			_Pool.Push(item);
		}

		public void Destroy()
		{
			_Pool.Clear();
			_Pool = null;
			_CreateDelegate = null;
		}
	}
}