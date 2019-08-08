using System;
using System.Collections.Generic;
using SCCRaceMode.Models;
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive;

namespace SCCRaceMode.ViewModels
{
    public class DriversListViewModel : ViewModelBase
    {
        private string _driverName;
        public string DriverName
        {
            get{ 
                return _driverName;
                }
            set{ 
                this.RaiseAndSetIfChanged(ref _driverName, value);
                }
        }

        public ObservableCollection<Driver> Drivers { get; }

        public DriversListViewModel()
        {
            Drivers = new ObservableCollection<Driver>();
            _driverName = "";
            IObservable<bool> addEnabled = this.WhenAnyValue(
                x => x.DriverName,
                x => !string.IsNullOrWhiteSpace(x)
            );

            Add = ReactiveCommand.Create(AddDriverToList,addEnabled);
        }

        public ReactiveCommand<Unit, Unit> Add { get; }

        private void AddDriverToList(){
            Driver driver = new Driver {Name = DriverName};
            Drivers.Add(driver);
            DriverName = "";
        }
    }
}