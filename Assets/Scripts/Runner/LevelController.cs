using System;
using System.Collections.Generic;
using Heroes;
using Items;
using RamenSea.Foundation.Extensions;
using UnityEngine;

namespace Runner {
    public class LevelController: GameMechanic {
        /**
         * Nasty static stuff ugh
         */
        [SerializeField] private float baseClosingTime = 5f;
        [SerializeField] private float closingTimeExtraPerMadHero = 5f;
        
        public static int currentLevel { private set; get; } = 1;
        
        private LevelData levelData;
        public LevelData GetLevelData() => this.levelData;

        private List<int> heroesMad;
        private List<int> heroesHappy;

        public Action onHeroMadHappyCountDidChange;
        public Action onClosingTimeStarted;

        public int madHeroesCount => this.heroesMad.Count;
        public int happyHeroesCount => this.heroesHappy.Count;
        public int totalHeroCount => this.levelData.heroes.Length;
        
        public float timeTilClosing { private set; get; }
        public bool isClosingTime { private set; get; }

        private bool hasTickedLevel = false;
        
        private void Awake() {
            this.heroesMad = new();
            this.heroesHappy = new();

        }

        private void Update() {
            if( runner.status != GameRunner.Status.Running) {
                return;
            }

            if (this.isClosingTime) {
                this.timeTilClosing -= Time.deltaTime;
                if (this.timeTilClosing <= 0) {
                    this.timeTilClosing = 0;
                    this.runner.StoreDidClose();
                }
            }
        }

        public override void OnSetGameRunner(GameRunner runner) {
            base.OnSetGameRunner(runner);

            var basicRequestTime = 20f;
            var possibleItems = new[] {
                Item.Potion,
                Item.Sword,
                Item.SpellBook
            };
            var possibleHeroes = new[] {
                HeroType.Gremlin,
                HeroType.Karen,
                HeroType.Link,
                HeroType.Merlin
            };
            var possibleDoors = new[] {
                SpawnDoor.Door1,
                SpawnDoor.Door2,
                SpawnDoor.Door3
            };
            var random = new System.Random();
            switch (currentLevel) {
                case 1: {
                    this.levelData = new LevelData() {
                        level = currentLevel,
                        heroes = new[] {
                            new HeroSpawn() {
                                id = 1,
                                door = SpawnDoor.Door1,
                                heroType = HeroType.Link,
                                variant = 0,
                                requestItem = Item.Potion,
                                spawnTimeInSeconds = 1f,
                                requestTime = basicRequestTime,
                            }
                        }
                    };
                    break;
                }
                case 2: {
                    this.levelData = new LevelData() {
                        level = currentLevel,
                        heroes = new[] {
                            new HeroSpawn() {
                                id = 1,
                                door = SpawnDoor.Door1,
                                heroType = HeroType.Link,
                                variant = 0,
                                requestItem = Item.Sword,
                                spawnTimeInSeconds = 1f,
                                requestTime = basicRequestTime,
                            },
                            new HeroSpawn() {
                                id = 2,
                                door = SpawnDoor.Door2,
                                heroType = HeroType.Merlin,
                                variant = 0,
                                requestItem = Item.SpellBook,
                                spawnTimeInSeconds = 5f,
                                requestTime = basicRequestTime,
                            }
                        }
                    };
                    break;
                }
                case 3: {
                    this.levelData = new LevelData() {
                        level = currentLevel,
                        heroes = new[] {
                            new HeroSpawn() {
                                id = 1,
                                door = SpawnDoor.Door1,
                                heroType = HeroType.Link,
                                variant = 0,
                                requestItem = possibleItems.RandomElement(random),
                                spawnTimeInSeconds = 1f,
                                requestTime = basicRequestTime,
                            },
                            new HeroSpawn() {
                                id = 2,
                                door = SpawnDoor.Door2,
                                heroType = HeroType.Merlin,
                                variant = 0,
                                requestItem = Item.SpellBook,
                                spawnTimeInSeconds = 4f,
                                requestTime = 6f,
                            }
                        }
                    };
                    break;
                }
                case 4: {
                    this.levelData = new LevelData() {
                        level = currentLevel,
                        heroes = new[] {
                            new HeroSpawn() {
                                id = 1,
                                door = SpawnDoor.Door1,
                                heroType = HeroType.Link,
                                variant = 0,
                                requestItem = Item.Sword,
                                spawnTimeInSeconds = 1f,
                                requestTime = basicRequestTime,
                            },
                            new HeroSpawn() {
                                id = 2,
                                door = SpawnDoor.Door3,
                                heroType = HeroType.Karen,
                                variant = 0,
                                requestItem = possibleItems.RandomElement(random),
                                spawnTimeInSeconds = 4f,
                                requestTime = basicRequestTime,
                            },
                        }
                    };
                    break;
                }
                case 5: {
                    this.levelData = new LevelData() {
                        level = currentLevel,
                        heroes = new[] {
                            new HeroSpawn() {
                                id = 1,
                                door = SpawnDoor.Door1,
                                heroType = HeroType.Link,
                                variant = 0,
                                requestItem = Item.Sword,
                                spawnTimeInSeconds = 1f,
                                requestTime = basicRequestTime,
                            },
                            new HeroSpawn() {
                                id = 2,
                                door = SpawnDoor.Door3,
                                heroType = HeroType.Karen,
                                variant = 0,
                                requestItem = possibleItems.RandomElement(random),
                                spawnTimeInSeconds = 4f,
                                requestTime = basicRequestTime,
                            },
                        }
                    };
                    break;
                }
                case 6: {
                    this.levelData = new LevelData() {
                        level = currentLevel,
                        heroes = new[] {
                            new HeroSpawn() {
                                id = 1,
                                door = SpawnDoor.Door1,
                                heroType = HeroType.Merlin,
                                variant = 0,
                                requestItem = Item.SpellBook,
                                spawnTimeInSeconds = 1f,
                                requestTime = 4f,
                            },
                            new HeroSpawn() {
                                id = 2,
                                door = SpawnDoor.Door3,
                                heroType = HeroType.Merlin,
                                variant = 0,
                                requestItem = Item.Sword,
                                spawnTimeInSeconds = 3f,
                                requestTime = basicRequestTime,
                            },
                            new HeroSpawn() {
                                id = 3,
                                door = SpawnDoor.Door2,
                                heroType = HeroType.Merlin,
                                variant = 0,
                                requestItem = Item.Potion,
                                spawnTimeInSeconds = 6f,
                                requestTime = basicRequestTime,
                            },
                        }
                    };
                    break;
                }
                case 9: {
                    this.levelData = new LevelData() {
                        level = currentLevel,
                        heroes = new[] {
                            new HeroSpawn() {
                                id = 1,
                                door = SpawnDoor.Door1,
                                heroType = HeroType.Karen,
                                variant = 0,
                                requestItem = possibleItems.RandomElement(random),
                                spawnTimeInSeconds = 1f,
                                requestTime = 4f,
                            },
                            new HeroSpawn() {
                                id = 2,
                                door = SpawnDoor.Door2,
                                heroType = HeroType.Karen,
                                variant = 0,
                                requestItem = possibleItems.RandomElement(random),
                                spawnTimeInSeconds = 3f,
                                requestTime = basicRequestTime,
                            },
                            new HeroSpawn() {
                                id = 3,
                                door = SpawnDoor.Door3,
                                heroType = HeroType.Karen,
                                variant = 0,
                                requestItem = possibleItems.RandomElement(random),
                                spawnTimeInSeconds = 6f,
                                requestTime = basicRequestTime,
                            },
                        }
                    };
                    break;
                }
                default: {
                    var heroCount = (currentLevel.ToFloat() * 0.5f).ToInt();

                    if (currentLevel == 12) {
                        //Gremlin rush level
                        // this.
                        basicRequestTime = 5f;
                        heroCount = 12;
                        possibleHeroes = new[] { HeroType.Gremlin };
                    }
                    
                    if (currentLevel == 16) {
                        //Karen rush level
                        // this.
                        basicRequestTime = 5f;
                        heroCount = 12;
                        possibleHeroes = new[] { HeroType.Gremlin };
                    }
                    
                    this.levelData = new LevelData() {
                        level = currentLevel,
                        heroes = new HeroSpawn[heroCount]
                    };
                    for (int i = 0; i < heroCount; i++) {
                        var extraSpawnTimeScaler = 2f;
                        this.levelData.heroes[i] = new HeroSpawn() {
                            id = i,
                            door = possibleDoors.RandomElement(random),
                            heroType = possibleHeroes.RandomElement(random),
                            variant = 0,
                            requestItem = possibleItems.RandomElement(random),
                            spawnTimeInSeconds = 2f + i * extraSpawnTimeScaler,
                            requestTime = basicRequestTime,
                        };
                        
                    }
                    break;
                }
            }

        }

        public override void OnStateChange(
            GameRunner.Status status,
            GameRunner.LevelResult level_result
        ) {
            base.OnStateChange(status, level_result);
            if( status == GameRunner.Status.End && this.hasTickedLevel == false) {
                this.hasTickedLevel = true;
                if( level_result == GameRunner.LevelResult.Won ) {
                    currentLevel += 1;
                } else {
                    currentLevel = 1;
                }
            }
        }

        public void HeroStatusChange(int id, bool isMad) {
            var list = isMad ? this.heroesMad : this.heroesHappy;

            if (list.Contains(id)) {
                return;
            }
            
            list.Add(id);

            var itIsClosingTime = this.heroesHappy.Count + this.heroesMad.Count;
            if (itIsClosingTime == this.levelData.heroes.Length && this.isClosingTime == false) {
                this.isClosingTime = true;
                if (this.madHeroesCount == 0) {
                    this.timeTilClosing = 3f;
                } else {
                    this.timeTilClosing = this.baseClosingTime + this.closingTimeExtraPerMadHero * this.heroesMad.Count;
                }
                this.onClosingTimeStarted?.Invoke();
            }
            
            this.onHeroMadHappyCountDidChange?.Invoke();
        }
    }
}