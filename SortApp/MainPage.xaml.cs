using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SortApp
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private readonly Random _random = new Random();

		private bool _processing;

		public MainPage()
		{
			this.InitializeComponent();

			Items = new ObservableCollection<int>();
		}

		public ObservableCollection<int> Items { get; }

		private void Init_Click(object sender, RoutedEventArgs e)
		{
			if (_processing) return;
			_processing = true;

			Items.Clear();

			Task.Run(async () =>
			{
				var list = new List<int>(100);
				for (int i = 1; i <= 100; i++)
					list.Add(i * 5);

				while (list.Count > 0)
				{
					var i = _random.Next(list.Count);

					await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
					{
						Items.Add(list[i]);
					});

					await Task.Delay(10);

					list.RemoveAt(i);
				}

				_processing = false;
			});
		}

		private void Sort_Click(object sender, RoutedEventArgs e)
		{
			if (_processing) return;
			_processing = true;

			Task.Run(async () =>
			{
				var items = Items.Select(x => x).ToList();
				var swaps = new List<Tuple<int, int>>();

				for (int i = 0; i < items.Count; i++)
				{
					var max = i;

					for (int j = 0; j < items.Count; j++)
					{
						if (items[j] > items[max])
						{
							Swap(items, j, max);
							swaps.Add(new Tuple<int, int>(j, max));
							j = 0;
						}
					}
				}

				await Draw(swaps).ConfigureAwait(false);

				_processing = false;
			});
		}

		private void Swap(IList<int> items, int a, int b)
		{
			var item = items[a];
			items[a] = items[b];
			items[b] = item;
		}

		private async Task Draw(List<Tuple<int, int>> swaps)
		{
			foreach (var swap in swaps)
			{
				await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					Swap(Items, swap.Item1, swap.Item2);
				});

				await Task.Delay(10);
			}
		}

		private void Sort2_Click(object sender, RoutedEventArgs e)
		{
			if (_processing) return;
			_processing = true;

			Task.Run(async () =>
			{
				void sort(List<int> values, int a, int b, List<Tuple<int, int>> sw)
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
								Swap(values, i, j);
								sw.Add(new Tuple<int, int>(i, j));
							}

							i++;
							j--;
						}
					}

					if (a < j)
						sort(values, a, j, sw);
					if (b > i)
						sort(values, i, b, sw);
				}

				var items = Items.Select(x => x).ToList();
				var swaps = new List<Tuple<int, int>>();

				sort(items, 0, items.Count - 1, swaps);

				await Draw(swaps).ConfigureAwait(false);

				_processing = false;
			});
		}
	}
}
