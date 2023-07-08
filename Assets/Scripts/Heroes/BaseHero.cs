using System;
using RamenSea.Foundation.Extensions;
using UnityEngine;

namespace Heroes {
    public enum HeroType: byte {
        None, Link, Karen, Witcher, Merlin
    }

    public static class HeroTypeExtensions {
        public static int GetPrefabHashCode(this HeroType heroType, byte variant) {
            return ((int)heroType) * 10000 + variant.ToInt(); //oof
        }
    }
    public class BaseHero: MonoBehaviour {
        // Enum specifying the type of hero the subclass is
        public virtual HeroType heroType => HeroType.None;
        // Allows you to get the variant enum without having to cast to a class
        public virtual byte heroVariantValue => 0;


        protected float timeTilGettingMad = 0f;
        protected float maxTimeTilGettingMad = 0f;
        protected bool isMad = false;
        
        // Stubbed out a few of the common mono behavior life cycle classes to make it easier work in the future
        // We can remove these if we don't end up using em
        protected virtual void Awake() { }
        protected virtual void Start() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected virtual void Update() { }
    }
}