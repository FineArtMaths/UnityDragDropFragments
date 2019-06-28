/**************************

Example of a component you can attach to a Canvas element to make it possible to place a card there.
The card would have the Draggable component.

GameState is a component attached to some permanent object in the scene. It contains most of the game logic.
Obviously you will want to write your own thing to resolve questions like "can the current player pick up this item"
and "can this item be dropped in this location" etc.

This comes from a larger Unity prototype of a card game and hasn't been cleaned up, so you will have 
to "pick the bones out" to use it in your own project.

**************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

    public int belongsToPlayer = -1;
    public int slotIndex = -1;
    public bool dropHereEndsTurn = false;
    public bool mouseOverGlow = true;
    public Color glowColour;
    private Color nonGlowColour;
    private GameState gameState;

    public void Start()
    {
        this.gameState = (GameState)GameObject.Find("GameState").GetComponent<GameState>();
        this.nonGlowColour = this.GetComponent<Image>().color;
    }

    public void Update()
    {
        // This is very specific code for the prototype this came from. You probably don't want to use it.
        if(this.slotIndex == -1)
        {
            int count = 0;
            foreach (Transform child in transform)
            {
                if (child != this.transform)
                {
                    count++;
                }
            }
            if (count < 4)
            {
                this.GetComponent<HorizontalLayoutGroup>().spacing = 10;
            }
            else
            {
                this.GetComponent<HorizontalLayoutGroup>().spacing = 0 - 10 * count;
            }
        }
    }

    public void OnDrop(PointerEventData d)
    {
        Draggable drg = (Draggable)d.pointerDrag.GetComponent<Draggable>();
        if (drg.dragging)
        {
            if (mouseOverGlow && this.belongsToPlayer == drg.belongsToPlayer)
            {
                this.GetComponent<Image>().color = nonGlowColour;
            }
            this.gameState.dropCard(drg, this);
        }
    }

    public void OnPointerEnter(PointerEventData d)
    {
        if (d != null && d.pointerDrag != null)
        {
            Draggable drg = (Draggable)d.pointerDrag.GetComponent<Draggable>();
            if (gameState.isLegalDrop(drg, this))
            {
                if (mouseOverGlow)
                {
                    this.GetComponent<Image>().color = glowColour;
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData d)
    {
        if (d != null && d.pointerDrag != null)
        {
            Draggable drg = (Draggable)d.pointerDrag.GetComponent<Draggable>();
            if (mouseOverGlow && drg != null && this.belongsToPlayer == drg.belongsToPlayer && this.belongsToPlayer == this.gameState.playerTurn)
            {
                this.GetComponent<Image>().color = nonGlowColour;
            }
        }
    }



}
