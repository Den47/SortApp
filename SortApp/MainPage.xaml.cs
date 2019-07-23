using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SortApp
{
	public sealed partial class MainPage : Page
	{
		private readonly CollectionHelper _helper = new CollectionHelper();

		private bool _processing;
		private CancellationTokenSource _cancellationToken;

		public MainPage()
		{
			this.InitializeComponent();
			Loaded += MainPage_Loaded;
		}

		public ObservableCollection<int> Items { get; } = new ObservableCollection<int>();

		private void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= MainPage_Loaded;
			Reset_Click(null, null);
		}

		private void Reset_Click(object sender, RoutedEventArgs e)
		{
			if (_processing) return;
			_processing = true;

			Items.Clear();

			Task.Run(async () =>
			{
				var list = _helper.GenerateDefault(100).Select(x => 5 * x);

				foreach (var item in list)
				{
					await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Items.Add(item));
					await Task.Delay(5);
				}

				_processing = false;
			});
		}

		private async Task Draw(IList<ISortAction> actions)
		{
			foreach (var action in actions)
			{
				if (_cancellationToken.Token.IsCancellationRequested)
					return;

				await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action.Activate(Items));
				await Task.Delay(10);
			}
		}

		private void SortClick(Func<IList<int>, IList<ISortAction>> sortAction)
		{
			if (_processing) return;
			_processing = true;

			Task.Run(async () =>
			{
				var actions = sortAction(Items.ToList());
				_cancellationToken = new CancellationTokenSource();
				await Draw(actions).ConfigureAwait(false);
				_processing = false;
			});
		}

		private void Stop_Click(object sender, RoutedEventArgs e)
		{
			_cancellationToken?.Cancel();
		}

		private void Sort1_Click(object sender, RoutedEventArgs e)
		{
			SortClick(_helper.SortSlow);
		}

		private void Sort2_Click(object sender, RoutedEventArgs e)
		{
			SortClick(_helper.SortQuick);
		}

		private void Sort3_Click(object sender, RoutedEventArgs e)
		{
			SortClick(_helper.SortSwap);
		}

		private void Sort4_Click(object sender, RoutedEventArgs e)
		{
			SortClick(_helper.SortInsert);
		}

		private void Sort5_Click(object sender, RoutedEventArgs e)
		{
			SortClick(_helper.SortSelect);
		}
	}
}
