using System.Collections.Generic;
using BeauUtil;

namespace FieldDay.Assets {
    /// <summary>
    /// Interface for a global configuration asset.
    /// </summary>
    [TypeIndexCapacity(512)]
    public interface IGlobalAsset {
        void Mount();
        void Unmount();
    }
}