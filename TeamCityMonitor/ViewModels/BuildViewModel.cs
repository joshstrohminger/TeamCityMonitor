﻿using Windows.UI;
using Interfaces;
using MVVM;

namespace TeamCityMonitor.ViewModels
{
    public class BuildViewModel : ObservableObject, IBuildViewModel
    {
        private string _id;
        private string _name = "new build";

        public ILabeledColor[] Colors { get; } =
        {
            new LabeledColor("Success", "#00FF00"),
            new LabeledColor("Failure", "#FF0000"),
            new LabeledColor("Queued", "#FFFF00"),
            new LabeledColor("Running", "#0000FF"),
            new LabeledColor("Stale", "#5E1C5E"),
            new LabeledColor("Idle", "#000000"),
            new LabeledColor("API Call", "#00FFFF"),
            new LabeledColor("API Error", "#FFFFFF"),
            new LabeledColor("Investigating", "#FFA500"),
        };

        public Color Success => Colors[0].Color;
        public Color Failure => Colors[1].Color;
        public Color Queued => Colors[2].Color;
        public Color Running => Colors[3].Color;
        public Color Stale => Colors[4].Color;
        public Color Idle => Colors[5].Color;
        public Color ApiCall => Colors[6].Color;
        public Color ApiError => Colors[7].Color;
        public Color Investigating => Colors[8].Color;
        public int[] AllLedIndexes { get; set; }
        public int RunningQueuedLedIndex { get; set; }

        public string Id
        {
            get => _id;
            set => UpdateOnPropertyChanged(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => UpdateOnPropertyChanged(ref _name, value);
        }
    }
}
