using System;
using Heroes;
using Items;

namespace Runner {
    
    public enum SpawnDoor {
        Door1, Door2, Door3,
    }
    public struct HeroSpawn {
        public static readonly HeroSpawn Null = new HeroSpawn() {
            id = -1,
            heroType = HeroType.None,
        };

        public int id;
        public HeroType heroType;
        public byte variant;
        public SpawnDoor door;
        public Item requestItem;
        public float spawnTimeInSeconds;

        public bool isNull => this.heroType == HeroType.None;
    }
    public struct LevelData {
        public static readonly LevelData Null = new LevelData() {
            level = -1,
            heroes = Array.Empty<HeroSpawn>()
        };
        public int level;
        public HeroSpawn[] heroes;

        public bool isNull => this.level < 0;
    }
}