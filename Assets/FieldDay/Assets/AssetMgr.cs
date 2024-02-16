using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BeauUtil;
using BeauUtil.Debugger;
using Unity.IL2CPP.CompilerServices;

using GlobalAssetIndex = BeauUtil.TypeIndex<FieldDay.Assets.IGlobalAsset>;
using LiteAssetIndex = BeauUtil.TypeIndex<FieldDay.Assets.ILiteAsset>;

namespace FieldDay.Assets {
    /// <summary>
    /// Asset manager.
    /// </summary>
    public sealed class AssetMgr {
        private readonly IGlobalAsset[] m_GlobalAssetTable = new IGlobalAsset[GlobalAssetIndex.Capacity];
        private readonly IAssetCollection[] m_LiteAssetTable = new IAssetCollection[LiteAssetIndex.Capacity];

        #region Events

        internal void Shutdown() {
            for (int i = 0; i < LiteAssetIndex.Count; i++) {
                if (m_LiteAssetTable[i] != null) {
                    m_LiteAssetTable[i].Clear();
                }
            }

            for(int i = 0; i < GlobalAssetIndex.Count; i++) {
                if (m_GlobalAssetTable[i] != null) {
                    m_GlobalAssetTable[i].Unmount();
                }
            }

            Array.Clear(m_LiteAssetTable, 0, m_LiteAssetTable.Length);
            Array.Clear(m_GlobalAssetTable, 0, m_GlobalAssetTable.Length);
        }

        #endregion // Events

        #region Registration

        #region Global

        /// <summary>
        /// Registers the given global asset.
        /// </summary>
        public void Register(IGlobalAsset asset) {
            Assert.NotNull(asset);

            Type assetType = asset.GetType();
            int index = GlobalAssetIndex.Get(assetType);

            Assert.True(m_GlobalAssetTable[index] == null, "[AssetMgr] Global asset of type '{0}' already registered", assetType);
            m_GlobalAssetTable[index] = asset;

            RegistrationCallbacks.InvokeRegister(asset);
            asset.Mount();
            Log.Msg("[AssetMgr] Global asset '{0}' registered", assetType.FullName);
        }

        /// <summary>
        /// Deregisters the given global asset.
        /// </summary>
        public void Deregister(IGlobalAsset asset) {
            Assert.NotNull(asset);

            Type assetType = asset.GetType();
            int index = GlobalAssetIndex.Get(assetType);

            if (m_GlobalAssetTable[index] == asset) {
                m_GlobalAssetTable[index] = null;

                asset.Unmount();
                RegistrationCallbacks.InvokeDeregister(asset);
                Log.Msg("[AssetMgr] Global asset '{0}' deregistered", assetType.FullName);
            }
        }

        #endregion // Global

        #region Packages

        // TODO: Implement

        #endregion // Packages

        #region Lite

        /// <summary>
        /// Registers the given lightweight asset to be looked up.
        /// </summary>
        public void AddLite<T>(StringHash32 id, T data) where T : struct, ILiteAsset {
            AssetCollection<T> typedCollection = GetLiteCollection<T>(true);
            typedCollection.Register(id, data);
        }

        /// <summary>
        /// Registers the given set of lightweight assets to be looked up.
        /// </summary>
        public void AddLite<T>(T[] data, AssetKeyFunction<T> keyFunc) where T : struct, ILiteAsset {
            if (keyFunc == null) {
                throw new ArgumentNullException("keyFunc");
            }
            AssetCollection<T> typedCollection = GetLiteCollection<T>(true);
            for (int i = 0; i < data.Length; i++) {
                typedCollection.Register(keyFunc(data[i]), data[i]);
            }
        }

        /// <summary>
        /// Registers the given set of lightweight assets to be looked up.
        /// </summary>
        public void AddLite<T>(IEnumerable<T> data, AssetKeyFunction<T> keyFunc) where T : struct, ILiteAsset {
            if (keyFunc == null) {
                throw new ArgumentNullException("keyFunc");
            }
            AssetCollection<T> typedCollection = GetLiteCollection<T>(true);
            foreach(var asset in data) {
                typedCollection.Register(keyFunc(asset), asset);
            }
        }

        /// <summary>
        /// Deregisters the given lightweight asset with the given key.
        /// </summary>
        public void RemoveLite<T>(StringHash32 id) where T : struct, ILiteAsset {
            AssetCollection<T> typedCollection = GetLiteCollection<T>(false);
            typedCollection?.Deregister(id);
        }

        /// <summary>
        /// Deregisters the given set of lightweight assets.
        /// </summary>
        public void RemoveLite<T>(T[] data, AssetKeyFunction<T> keyFunc) where T : struct, ILiteAsset {
            if (keyFunc == null) {
                throw new ArgumentNullException("keyFunc");
            }
            AssetCollection<T> typedCollection = GetLiteCollection<T>(false);
            if (typedCollection != null) {
                for(int i = 0; i < data.Length; i++) {
                    typedCollection.Deregister(keyFunc(data[i]));
                }
            }
        }

        /// <summary>
        /// Deregisters the given set of lightweight assets.
        /// </summary>
        public void RemoveLite<T>(IEnumerable<T> data, AssetKeyFunction<T> keyFunc) where T : struct, ILiteAsset {
            if (keyFunc == null) {
                throw new ArgumentNullException("keyFunc");
            }
            AssetCollection<T> typedCollection = GetLiteCollection<T>(false);
            if (typedCollection != null) {
                foreach (var asset in data) {
                    typedCollection.Deregister(keyFunc(asset));
                }
            }
        }

        #endregion // Lite

        #endregion // Registration

        #region Lookup

        #region Global

        /// <summary>
        /// Returns the global asset of the given type.
        /// This will assert if none is found.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IGlobalAsset GetGlobal(Type type) {
            int index = GlobalAssetIndex.Get(type);
            IGlobalAsset asset = m_GlobalAssetTable[index];
#if DEVELOPMENT
            if (asset == null) {
                Assert.Fail("No global asset found for type '{0}'", type.FullName);
            }
#endif // DEVELOPMENT
            return asset;
        }

        /// <summary>
        /// Returns the global asset of the given type.
        /// This will assert if none is found.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetGlobal<T>() where T : class, IGlobalAsset {
            int index = GlobalAssetIndex.Get<T>();
            IGlobalAsset asset = m_GlobalAssetTable[index];
#if DEVELOPMENT
            if (asset == null) {
                Assert.Fail("No global asset found for type '{0}'", typeof(T).FullName);
            }
#endif // DEVELOPMENT
            return (T) asset;
        }

        /// <summary>
        /// Attempts to return the global asset of the given type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetGlobal(Type type, out IGlobalAsset asset) {
            int index = GlobalAssetIndex.Get(type);
            asset = index < m_GlobalAssetTable.Length ? m_GlobalAssetTable[index] : null;
            return asset != null;
        }

        /// <summary>
        /// Attempts to return the global asset of the given type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetGlobal<T>(out T asset) where T : class, IGlobalAsset {
            int index = GlobalAssetIndex.Get<T>();
            asset = (T) (index < m_GlobalAssetTable.Length ? m_GlobalAssetTable[index] : null);
            return asset != null;
        }

        #endregion // Global

        #region Lite

        /// <summary>
        /// Looks up the lightweight asset with the given id.
        /// </summary>
        [Il2CppSetOption(Option.NullChecks, false)]
        public T GetLite<T>(StringHash32 id) where T : struct, ILiteAsset {
            AssetCollection<T> typedCollection = GetLiteCollection<T>(true);
            return typedCollection.Lookup(id);
        }

        /// <summary>
        /// Attempts to look up the lightweight asset with the given id.
        /// </summary>
        [Il2CppSetOption(Option.NullChecks, false)]
        public bool TryGetLite<T>(StringHash32 id, out T asset) where T : struct, ILiteAsset {
            AssetCollection<T> typedCollection = GetLiteCollection<T>(true);
            return typedCollection.TryLookup(id, out asset);
        }

        #endregion // Lite

        #endregion // Lookup

        #region Internal

        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.NullChecks, false)]
        private AssetCollection<T> GetLiteCollection<T>(bool create) where T : struct, ILiteAsset {
            int index = LiteAssetIndex.Get<T>();
            AssetCollection<T> typedCollection;
            ref IAssetCollection collection = ref m_LiteAssetTable[index];
            if (collection == null && create) {
                collection = typedCollection = new AssetCollection<T>();
            } else {
                typedCollection = (AssetCollection<T>) collection;
            }
            return typedCollection;
        }

        #endregion // Internal
    }
}