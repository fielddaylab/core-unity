using System.Runtime.CompilerServices;
using BeauUtil;
using FieldDay.Assets;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.UI;
using UnityEngine;

namespace FieldDay.Core {
    /// <summary>
    /// Object lookup shortcuts.
    /// </summary>
    public class Lookup {
        /// <summary>
        /// Looks up the global asset of the given type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public T GlobalAsset<T>() where T : class, IGlobalAsset {
            return Game.Assets.GetGlobal<T>();
        }

        /// <summary>
        /// Looks up the lite asset with the given id.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public T LiteAsset<T>(StringHash32 id) where T : struct, ILiteAsset {
            return Game.Assets.GetLite<T>(id);
        }

        /// <summary>
        /// Looks up the shared state of the given type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public T State<T>() where T : class, ISharedState {
            return Game.SharedState.Get<T>();
        }

        /// <summary>
        /// Looks up the shared gui panel of the given type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public T Panel<T>() where T : class, ISharedGuiPanel {
            return Game.Gui.GetShared<T>();
        }

        /// <summary>
        /// Looks up the named RectTransform of the given type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public RectTransform NamedRectTransform(StringHash32 name) {
            return Game.Gui.LookupNamed(name);
        }

        /// <summary>
        /// Looks up the list of components of the given type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public ComponentIterator<T> Components<T>() where T : class, IComponentData {
            return Game.Components.ComponentsOfType<T>();
        }
    }
}