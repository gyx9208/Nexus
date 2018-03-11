using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nexus.Logic.Base
{
	public class NestedCommonPool<TKey, TValue> where TValue : new()
	{
		private Dictionary<TKey, CommonPool<TValue>> _Nested = new Dictionary<TKey, CommonPool<TValue>>();

		public TValue Get(TKey key)
		{
			CommonPool<TValue> pool = null;
			if (!_Nested.TryGetValue(key, out pool))
			{
				pool = new CommonPool<TValue>();
				_Nested.Add(key, pool);
			}
			return pool.Get();
		}

		public void Return(TKey key, TValue value)
		{
			if (value == null)
				return;
			CommonPool<TValue> pool = null;
			if (!_Nested.TryGetValue(key, out pool))
			{
				pool = new CommonPool<TValue>();
				_Nested.Add(key, pool);
			}
			pool.Return(value);
		}

		public void Destroy()
		{
			foreach(var pool in _Nested)
			{
				pool.Value.Destroy();
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