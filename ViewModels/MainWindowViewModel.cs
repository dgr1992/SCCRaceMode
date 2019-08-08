using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using Avalonia.Input;
using ReactiveUI;
using SCCRaceMode.Models;
using SCCRaceMode.Controller;
using Avalonia;

namespace SCCRaceMode.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _driverName;
        public string DriverName
        {
            get{ return _driverName; }
            set{ this.RaiseAndSetIfChanged(ref _driverName, value); }
        }

        private int _heatSize;
        public int HeatSize
        {
            get{return _heatSize;}
            set{this.RaiseAndSetIfChanged(ref _heatSize, value);}
        }

        private int _numberHeats;
        public int NumberHeats
        {
            get{ return _numberHeats; }
            set{ _numberHeats = value; }
        }

        private int _rounds;
        public int Rounds
        {
            get{return _rounds;}
            set{this.RaiseAndSetIfChanged(ref _rounds, value);}
        }

        public ObservableCollection<Driver> Drivers { get; }

        public ObservableCollection<Round> RaceArangement { get; }
        
        public ReactiveCommand<Unit, Unit> Add { get; }
        public ReactiveCommand<Unit, Unit> Generate { get; }

        public ReactiveCommand<Unit, Unit> Export { get; }

        public MainWindowViewModel()
        {
            Drivers = new ObservableCollection<Driver>();
            RaceArangement = new ObservableCollection<Round>();

            _driverName = "";
            _heatSize = 12;
            _numberHeats = 2;
            _rounds = 5;

            //Enable of add button
            IObservable<bool> addEnabled = this.WhenAnyValue(
                x => x.DriverName,
                x => !string.IsNullOrWhiteSpace(x)
            );
            Add = ReactiveCommand.Create(AddDriverToList,addEnabled);

            //Enable of generate button
            IObservable<bool> numOfDrivers = this.WhenAnyValue(
                x => x.Drivers.Count,
                x => x > 0
            );
            IObservable<bool> numOfHeats = this.WhenAnyValue(
                x => x.HeatSize,
                x => x > 0
            );
            IObservable<bool> numOfRounds = this.WhenAnyValue(
                x => x.Rounds,
                x => x > 0
            );
            Observable.CombineLatest(numOfDrivers, Observable.CombineLatest(numOfHeats, numOfRounds, (a, b)=> a&&b), (a, b)=>a&&b)
                .DistinctUntilChanged();
            Generate = ReactiveCommand.Create(GenerateHeatArangements,numOfDrivers);

            //Enable Export
            IObservable<bool> createdRounds = this.WhenAnyValue(
                x => x.RaceArangement.Count,
                x => x > 0
            );
            Export = ReactiveCommand.Create(ExportToCSV,createdRounds);
        }

        private void AddDriverToList()
        {
            Driver driver = new Driver {Name = DriverName};
            Drivers.Add(driver);
            DriverName = "";
        }

        private void GenerateHeatArangements()
        {
            //CreateTestData();
            List<Driver> drivers = new List<Driver>(Drivers);
            
            SCCRaceModeGenerator generator = new SCCRaceModeGenerator(drivers, _numberHeats, _heatSize, _rounds);
            LinkedList<Round> rounds = generator.ArangeHeats();

            RaceArangement.Clear();
            foreach (Round round in rounds)
            {
                RaceArangement.Add(round);
            }
        }

        private void CreateTestData()
        {
            Drivers.Add(new Driver{Name = "Mathias Domig"});
            Drivers.Add(new Driver{Name = "Niklas Domig"});
            Drivers.Add(new Driver{Name = "Christian Gut"});
            Drivers.Add(new Driver{Name = "Patrick Haltiner"});
            Drivers.Add(new Driver{Name = "Thomi Oberkofler"});
            Drivers.Add(new Driver{Name = "Jakob Pirolt"});
            Drivers.Add(new Driver{Name = "Guntram Rümmele"});
            Drivers.Add(new Driver{Name = "Lukas Segato"});
            Drivers.Add(new Driver{Name = "Henri Weidmann"});
            Drivers.Add(new Driver{Name = "Martin Weidmann"});
            Drivers.Add(new Driver{Name = "Michel Buschor"});
            Drivers.Add(new Driver{Name = "Adriano Fruci"});
            Drivers.Add(new Driver{Name = "Laurin Grabher"});
            Drivers.Add(new Driver{Name = "Imrich Horvath"});
            Drivers.Add(new Driver{Name = "Luca Jerabek"});
        }

        private void ExportToCSV()
        {
            SCCExport export = new SCCExport();
            export.ExportToCSVAsync(new LinkedList<Round>(RaceArangement),new LinkedList<Driver>(Drivers),Application.Current.MainWindow);
        }
    }
}
