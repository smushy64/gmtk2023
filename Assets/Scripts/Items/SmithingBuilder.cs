// * Description:  Smithing Builder
// * Author:       Alicia Amarilla (smushyaa@gmail.com)
// * File Created: July 08, 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;

public class SmithingBuilder : BaseItemBuilder {
    public override Item itemToBuild => Item.Sword;
    [SerializeField]
    SpriteRenderer spriteRenderer;
}