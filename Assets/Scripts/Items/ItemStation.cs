// * Description:  Item Preparation Station
// * Author:       Alicia Amarilla (smushyaa@gmail.com)
// * File Created: July 08, 2023

using UnityEngine;
using Runner;
using Items;
using TMPro;

public class ItemStation : GameMechanic {

    [SerializeField]
    Item itemToPrepare = Item.None;
    [SerializeField, Min(0.1f)]
    float preparationTime = 1f;
    [SerializeField, Min(0.1f)]
    float failureTime = 1.5f;
    [SerializeField]
    TMP_Text timerText;

    enum StationStatus : byte {
        IS_STANDBY,
        IS_PREPARING,
        IS_READY,
    }

    public Item on_interact() {
        switch( status ) {
            default:
                break;
            case StationStatus.IS_STANDBY:
                status = StationStatus.IS_PREPARING;
                break;
            case StationStatus.IS_READY:
                return itemToPrepare;
        }
        return 0;
    }

    float preparation_timer = 0.0f;
    float failure_timer     = 0.0f;

    StationStatus status = StationStatus.IS_STANDBY;

    void Update() {
        switch( status ) {
            case StationStatus.IS_PREPARING:
                if( !timerText.gameObject.activeSelf ) {
                    timerText.gameObject.SetActive( true );
                }
                preparation_timer += Time.deltaTime;
                timerText.SetText(
                    (preparationTime - preparation_timer).ToString("N2")
                );
                if( preparation_timer >= preparationTime ) {
                    preparation_timer = 0.0f;
                    status = StationStatus.IS_READY;
                }
                break;
            case StationStatus.IS_READY:
                failure_timer += Time.deltaTime;
                if( failure_timer >= failureTime ) {
                    failure_timer = 0.0f;
                    status = StationStatus.IS_STANDBY;
                }
                break;
            default:
                timerText.gameObject.SetActive( false );
                break;
        }
    }

}