using System;
using System.Linq;
using System.Collections.ObjectModel;
using Realms;

namespace KbStats.ViewModels
{
    using Models;
    using Core.Utils;

    public class StatisticsViewModel : BaseViewModel, IDisposable
    {
        // just used for visual debugging
        public string Stats => _stats;
        private string _stats;

        public ObservableCollection<IntervalStatistics> CurrentInterval { get; set; } = new ObservableCollection<IntervalStatistics>();// => _currentInterval;
        //private ObservableCollection<IntervalStatistics> _currentInterval = new List<IntervalStatistics>();

        private Realm _realm;
        private IDisposable _allResultsToken;

        public StatisticsViewModel()
        {

            _realm = Realm.GetInstance(Constants.RealmConfiguration);
            _allResultsToken = _realm.All<IntervalStatistics>().SubscribeForNotifications((sender, changes, error) =>
            {
                if (error != null)
                {
                    // Show error message
                    return;
                }
                if (changes == null)
                {
                    // This is the case when the notification is called
                    // for the first time.
                    // Populate tableview/listview with all the items
                    // from `collection`
                    return;
                }

                var lasIndex = changes.InsertedIndices.Last();
                SetProperty(ref _stats, sender[lasIndex].ToString(), nameof(Stats));
                foreach (IntervalStatistics stat in sender)
                {
                    CurrentInterval.Add(stat);
                }
            });

            var stats = _realm.All<IntervalStatistics>();
            foreach (var stat in stats)
            {
                CurrentInterval.Add(stat);
            }

            if (CurrentInterval.Count > 0)
            {
                SetProperty(ref _stats, CurrentInterval.LastOrDefault().ToString(), nameof(Stats));
            }
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            _allResultsToken.Dispose();
        }

    }
}
