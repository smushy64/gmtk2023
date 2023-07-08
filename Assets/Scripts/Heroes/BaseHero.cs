using System;
using UnityEngine;

namespace Heroes {
    public enum HeroType: byte {
        None, Link, Karen, Witcher, Merlin
    }
    public class BaseHero: MonoBehaviour {
        
        // Enum specifying the type of hero the subclass is
        public virtual HeroType heroType => HeroType.None;
        // Allows you to get the variant enum without having to cast to a class
        public virtual byte heroVariantValue => 0;


        // Stubbed out a few of the common mono behavior life cycle classes to make it easier work in the future
        // We can remove these if we don't end up using em
        protected virtual void Awake() { }
        protected virtual void Start() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected virtual void Update() { }
    }
}