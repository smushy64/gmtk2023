// * Description:  Item Preparation Station
// * Author:       Alicia Amarilla (smushyaa@gmail.com)
// * File Created: July 08, 2023

using System;
using UnityEngine;
using Runner;
using Items;

public class ItemStation : GameMechanic {

    [SerializeField]
    Item itemToPrepare = Item.None;
    [SerializeField, Min(0.1f)]
    float preparationTime = 1f;
    [SerializeField, Min(0.1f)]
    float failureTime = 1.5f;

    public Item item_type => itemToPrepare;

    [SerializeField]
    GameObject sprite;

    Animator sprite_animator;

    public Action on_standby;
    public Action on_preparing;
    public Action on_ready;
    public Action on_not_ready;

    void Awake() {
        sprite_animator = sprite.GetComponent<Animator>();
    }

    enum StationStatus : byte {
        IS_STANDBY,
        IS_PREPARING,
        IS_READY,
    }

    void update_status( StationStatus new_status ) {
        status = new_status;
        switch( new_status ) {
            case StationStatus.IS_STANDBY:
                sprite.SetActive( false );
                on_standby?.Invoke();
                on_not_ready?.Invoke();
                break;
            case StationStatus.IS_PREPARING:
                on_preparing?.Invoke();
                on_not_ready?.Invoke();
                sprite.SetActive( true );
                sprite_animator?.Play( "Preparing" );
                break;
            case StationStatus.IS_READY:
                sprite.SetActive( false );
                on_ready?.Invoke();
                break;
        }
    }

    public Item on_interact() {
        switch( status ) {
            case StationStatus.IS_STANDBY:
                update_status( StationStatus.IS_PREPARING );
                break;
            case StationStatus.IS_READY:
                update_status( StationStatus.IS_STANDBY );
                return itemToPrepare;
            default:
                break;
        }
        return 0;
    }

    float preparation_timer = 0.0f;
    float failure_timer     = 0.0f;

    StationStatus status = StationStatus.IS_STANDBY;

    void Update() {
        switch( status ) {
            case StationStatus.IS_PREPARING:
                preparation_timer += Time.deltaTime;
                if( preparation_timer >= preparationTime ) {
                    preparation_timer = 0.0f;
                    update_status( StationStatus.IS_READY );
                }
                break;
            case StationStatus.IS_READY:
                failure_timer += Time.deltaTime;
                if( failure_timer >= failureTime ) {
                    failure_timer = 0.0f;
                    update_status( StationStatus.IS_STANDBY );
                }
                break;
            default:
                break;
        }
    }

}