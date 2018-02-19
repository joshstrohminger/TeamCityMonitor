using System.Collections;
using BlinkStickUniversal;

namespace Interfaces
{
    public interface IBuildViewModel
    {
        RgbColor Investigating { get; }
        RgbColor Queued { get; }
        RgbColor Running { get; }
        RgbColor Failure { get; }
        RgbColor Idle { get; }
        RgbColor Success { get; }
        ILabeledColor[] Colors { get; }
        string Id { get; set; }
        string Name { get; set; }
    }
}