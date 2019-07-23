using System;
using System.Collections.Generic;

namespace SortApp
{
	public interface ISortAction
	{
		ISortAction Activate<T>(IList<T> values);
	}

	public class Swap : ISortAction
	{
		public Swap(int a, int b)
		{
			A = a;
			B = b;
		}

		public int A { get; private set; }

		public int B { get; private set; }

		public ISortAction Activate<T>(IList<T> values)
		{
			if (A != B)
			{
				var value = values[A];
				values[A] = values[B];
				values[B] = value;
			}

			return this;
		}
	}

	public class Move : ISortAction
	{
		public Move(int from, int to)
		{
			From = from;
			To = to;
		}

		public int From { get; private set; }

		public int To { get; private set; }

		public ISortAction Activate<T>(IList<T> values)
		{
			if (From != To)
			{
				var value = values[From];
				values.RemoveAt(From);
				values.Insert(To, value);
			}

			return this;
		}
	}

	public class CollectionHelper
	{
		private readonly Random _random = new Random();

		public IEnumerable<int> GenerateDefault(int count)
		{
			if (count < 0)
				throw new ArgumentException();

			var list = new List<int>(count);
			for (int i = 1; i <= count; i++)
				list.Add(i);

			while (list.Count > 0)
			{
				var i = _random.Next(list.Count);
				yield return list[i];
				list.RemoveAt(i);
			}
		}

		public IList<ISortAction> SortSlow(IList<int> values)
		{
			var swaps = new List<ISortAction>();

			for (int i = 0; i < values.Count; i++)
			{
				for (int j = 0; j < values.Count; j++)
				{
					if (j < i && values[j] > values[i])
					{
						swaps.Add(new Swap(j, i).Activate(values));
						j = 0;
					}
				}
			}

			return swaps;
		}

		public IList<ISortAction> SortInsert(IList<int> values)
		{
			var moves = new List<ISortAction>();

			var max = 0;

			for (int i = 0; i < values.Count; i++)
			{
				if (i > max && values[i] < values[max])
				{
					var toIndex = max;

					for (int j = i; j >= 0; j--)
					{
						if (values[i] < values[j])
						{
							toIndex = j;
						}
						else if (i != j)
						{
							break;
						}
					}

					moves.Add(new Move(i, toIndex).Activate(values));
				}

				max = i;
			}

			return moves;
		}

		public IList<ISortAction> SortSwap(IList<int> values)
		{
			var swaps = new List<ISortAction>();

			var max = 0;

			for (int i = 0; i < values.Count; i++)
			{
				if (i > max && values[i] < values[max])
				{
					for (int j = i; j >= 0; j--)
					{
						if (values[i] < values[j])
						{
							swaps.Add(new Swap(i, j).Activate(values));
							i = j;
						}
						else if (i != j)
						{
							break;
						}
					}
				}
				else
				{
					max = i;
				}
			}

			return swaps;
		}

		public IList<ISortAction> SortQuick(IList<int> values)
		{
			var swaps = new List<ISortAction>();
			sort(0, values.Count - 1);
			return swaps;

			void sort(int a, int b)
			{
				var mid = a + (b - a) / 2;
				var x = values[mid];

				var i = a;
				var j = b;

				while (i < j)
				{
					while (values[i] < x)
						i++;
					while (values[j] > x)
						j--;

					if (i <= j)
					{
						if (i != j)
						{
							swaps.Add(new Swap(i, j).Activate(values));
						}

						i++;
						j--;
					}
				}

				if (a < j)
					sort(a, j);
				if (b > i)
					sort(i, b);
			}
		}

		public IList<ISortAction> SortSelect(IList<int> values)
		{
			var swaps = new List<ISortAction>();

			for (int i = 0; i < values.Count; i++)
			{
				var min = i;

				if (values.Count > i + 1)
				{
					for (int j = i + 1; j < values.Count; j++)
					{
						if (values[j] < values[min])
							min = j;
					}

					swaps.Add(new Swap(min, i).Activate(values));
				}
			}

			return swaps;
		}
	}
}
