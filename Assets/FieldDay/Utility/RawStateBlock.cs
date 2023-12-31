using BeauUtil;
using BeauUtil.Debugger;

namespace FieldDay {
    /// <summary>
    /// 64-byte arbitrary umanaged data store.
    /// </summary>
    public struct RawStateBlock64 {
        public const int MaxSize = 64;

        // storing as ulong to ensure 8-byte alignment (good enough for most types)
        private unsafe fixed ulong m_State[MaxSize / 8];

        /// <summary>
        /// Loads a value from the state block.
        /// </summary>
        public T Load<T>() where T : unmanaged {
            int tSize = Unsafe.SizeOf<T>();
            Assert.True(tSize <= MaxSize, "Unmanaged type '{0}' of size {1}b cannot fit in RawStateBlock64", typeof(T).FullName, tSize.ToStringLookup());
            unsafe {
                fixed (ulong* buff = m_State) {
                    return Unsafe.Reinterpret<ulong, T>(buff);
                }
            }
        }

        /// <summary>
        /// Stores a value into the state block.
        /// </summary>
        public void Store<T>(in T value) where T : unmanaged {
            int tSize = Unsafe.SizeOf<T>();
            Assert.True(tSize <= MaxSize, "Unmanaged type '{0}' of size {1}b cannot fit in RawStateBlock64", typeof(T).FullName, tSize.ToStringLookup());
            unsafe {
                fixed (ulong* buff = m_State) {
                    *(T*) (buff) = value;
                }
            }
        }

        /// <summary>
        /// Creates and stores a value into a 64-byte state block.
        /// </summary>
        static public RawStateBlock64 Create<T>(in T value) where T : unmanaged {
            RawStateBlock64 stateBlock = default;
            stateBlock.Store<T>(value);
            return stateBlock;
        }
    }

    /// <summary>
    /// 256-byte arbitrary umanaged data store.
    /// </summary>
    public struct RawStateBlock256 {
        public const int MaxSize = 256;

        // storing as ulong to ensure 8-byte alignment (good enough for most types)
        private unsafe fixed ulong m_State[MaxSize / 8];

        /// <summary>
        /// Loads a value from the state block.
        /// </summary>
        public T Load<T>() where T : unmanaged {
            int tSize = Unsafe.SizeOf<T>();
            Assert.True(tSize <= MaxSize, "Unmanaged type '{0}' of size {1}b cannot fit in RawStateBlock256", typeof(T).FullName, tSize.ToStringLookup());
            unsafe {
                fixed (ulong* buff = m_State) {
                    return Unsafe.Reinterpret<ulong, T>(buff);
                }
            }
        }

        /// <summary>
        /// Stores a value into the state block.
        /// </summary>
        public void Store<T>(in T value) where T : unmanaged {
            int tSize = Unsafe.SizeOf<T>();
            Assert.True(tSize <= MaxSize, "Unmanaged type '{0}' of size {1}b cannot fit in RawStateBlock256", typeof(T).FullName, tSize.ToStringLookup());
            unsafe {
                fixed (ulong* buff = m_State) {
                    *(T*) (buff) = value;
                }
            }
        }

        /// <summary>
        /// Creates and stores a value into a 256-byte state block.
        /// </summary>
        static public RawStateBlock256 Create<T>(in T value) where T : unmanaged {
            RawStateBlock256 stateBlock = default;
            stateBlock.Store<T>(value);
            return stateBlock;
        }
    }
}