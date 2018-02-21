﻿using Windows.UI;

namespace Interfaces
{
    public interface IBuildViewModel
    {
        Color Investigating { get; }
        Color Queued { get; }
        Color Running { get; }
        Color Failure { get; }
        Color Idle { get; }
        Color Success { get; }
        ILabeledColor[] Colors { get; }
        string Id { get; set; }
        string Name { get; set; }
    }
}