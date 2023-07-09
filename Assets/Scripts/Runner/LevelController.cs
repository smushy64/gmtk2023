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
                                requestItem = Item.Potion,
                                spawnTimeInSeconds = 1f,
                            },
                            new HeroSpawn() {
                                id = 2,
                                door = SpawnDoor.Door2,
                                heroType = HeroType.Karen,
                                variant = 0,
                                requestItem = Item.Potion,
                                spawnTimeInSeconds = 3f,
                            },
                            new HeroSpawn() {
                                id = 3,
                                door = SpawnDoor.Door2,
                                heroType = HeroType.Merlin,
                                variant = 0,
                                requestItem = Item.Potion,
                                spawnTimeInSeconds = 4f,
                            }
                        }
                    };
                    break;
                }
                default: {
                    var random = new System.Random();
                    var heroCount = 5;
                    List<HeroType> possibleHeroes = new();
                    List<SpawnDoor> possibleDoors = new();
                    
                    possibleHeroes.Add(HeroType.Link);
                    possibleHeroes.Add(HeroType.Karen);
                    possibleHeroes.Add(HeroType.Merlin);
                    possibleHeroes.Add(HeroType.Gremlin);//eh just add them all for now
                    
                    possibleDoors.Add(SpawnDoor.Door1);
                    possibleDoors.Add(SpawnDoor.Door2);
                    possibleDoors.Add(SpawnDoor.Door3);//eh just add them all for now lol
                    if (currentLevel > 6) {
                        heroCount = 10;
                    } else if (currentLevel > 4) {
                        heroCount = 7;
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
                            requestItem = Item.Potion,
                            spawnTimeInSeconds = 2f + i * extraSpawnTimeScaler,
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
            if( status == GameRunner.Status.End ) {
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
                this.timeTilClosing = this.baseClosingTime + this.closingTimeExtraPerMadHero * this.heroesMad.Count;
                this.onClosingTimeStarted?.Invoke();
            }
            
            this.onHeroMadHappyCountDidChange?.Invoke();
        }
    }
}