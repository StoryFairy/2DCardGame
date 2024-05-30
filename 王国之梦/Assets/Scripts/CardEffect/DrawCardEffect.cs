using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DrawCardEffect", menuName = "Card Effects/DrawCardEffect")]
public class DrawCardEffect : Effect
{
    public IntEventSO drawCardEvent;
    public override void Execute(CharacterBase from, CharacterBase target)
    {
        drawCardEvent?.RaisEvent(value,this);
    }
}
