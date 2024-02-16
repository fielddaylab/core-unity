#if (UNITY_EDITOR && !IGNORE_UNITY_EDITOR) || DEVELOPMENT_BUILD
#define DEVELOPMENT
#endif // (UNITY_EDITOR && !IGNORE_UNITY_EDITOR) || DEVELOPMENT_BUILD

using BeauUtil;

namespace FieldDay.Assets {
    /// <summary>
    /// Lightweight asset data.
    /// Useful for assets whose data can be contained with a struct.
    /// </summary>
    [TypeIndexCapacity(512)]
    public interface ILiteAsset { }
}