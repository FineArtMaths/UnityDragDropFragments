/**************************

Example of a component you can attach to a Canvas element to make it possible to drag and drop it like a playing card.
The intention is that is can only be dropped onto an element with the DropZone component.

GameState is a component attached to some permanent object in the scene. It contains most of the game logic.
Obviously you will want to write your own thing to resolve questions like "can the current player pick up this item"
and "can this item be dropped in this location" etc.

This comes from a larger Unity prototype of a card game and hasn't been cleaned up, so you will have 
to "pick the bones out" to use it in your own project.

**************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, 
                         IPointerEnterHandler, IPointerExitHandler,
                         IPointerDownHandler, IPointerUpHandler
{
    private static bool dragInProgress = false;
    public bool dragging = false;
    private Vector2 vOffset = new Vector2(0, 0);
    private Quaternion vZero3D = new Quaternion(0, 0, 0, 0);
    public Transform returnParent = null;
    public bool inflated = false;
    private Vector3 offScreen = new Vector3(1000.0f, 1000.0f, 0.0f);
    private GameObject focusCard = null;
    public int belongsToPlayer = -1;
    private GameState gameState;
    private float lastClickTime = 0.0f;

    public enum enCardLocation {Deck, Hand, Slot, Discard};
    public enCardLocation origin;

    public void Start()
    {
        this.focusCard = GameObject.Find("FocusCard");
        if(this.transform.parent != null && this.transform.parent.GetComponent<DropZone>() != null)
        {
            this.belongsToPlayer = this.transform.parent.GetComponent<DropZone>().belongsToPlayer;
            this.returnParent = this.transform.parent;
            this.origin = this.returnParent.GetComponent<DropZone>().slotIndex > -1 ? enCardLocation.Slot : enCardLocation.Hand;
        }
        this.gameState = (GameState)GameObject.Find("GameState").GetComponent<GameState>();
    }

    public void OnBeginDrag(PointerEventData d)
    {
        if(gameState.cardIsPlayable(this, ((Card)(this.GetComponent<Card>())).ac))
        {
            this.dragging = true;
            dragInProgress = true;
            this.vOffset.x = this.transform.position.x - d.position.x;
            this.vOffset.y = this.transform.position.y - d.position.y;
            this.transform.rotation = vZero3D;
            this.returnParent = this.transform.parent;
            DropZone parentZone = (DropZone)this.transform.parent.gameObject.GetComponent<DropZone>();
            if(parentZone != null)
            {
                this.origin = parentZone.slotIndex > -1 ? enCardLocation.Slot : enCardLocation.Hand;
            }
            this.transform.SetParent(this.transform.parent.parent);
            this.GetComponent<CanvasGroup>().blocksRaycasts = false;
            this.deflate();
        } else
        {
            this.dragging = false;
            dragInProgress = false;
        }
    }

    public void OnDrag(PointerEventData d)
    {
        if (this.dragging && this.belongsToPlayer == this.gameState.playerTurn)
        {
            this.transform.position = d.position + this.vOffset;
        }
    }

    public void OnEndDrag(PointerEventData d)
    {
        if (this.dragging)
        {
            this.dragging = false;
            dragInProgress = false;
            this.transform.SetParent(returnParent);
            this.GetComponent<CanvasGroup>().blocksRaycasts = true;
            this.deflate();
        }
    }

    public void OnPointerEnter(PointerEventData d)
    {
        this.inflate();
    }

    public void OnPointerExit(PointerEventData d)
    {
        this.deflate();
    }

    public void deflate()
    {
        this.inflated = false;
        this.focusCard.transform.position = this.offScreen;
    }
    private void inflate()
    {
        if (!dragInProgress)
        {
            this.inflated = true;
            ((Card)this.focusCard.GetComponent<Card>()).setAbstractCard((AbstractCard)(this.GetComponent<Card>()).ac);
            this.focusCard.GetComponent<Card>().drawCard();
            float yDelta = this.belongsToPlayer == 0 ? -50.0f : 50.0f;
            this.focusCard.transform.position = this.transform.position + new Vector3(0.0f, yDelta, 0.0f);
            ((Card)(this.focusCard.GetComponent<Card>())).drawCard();
        }
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        // Do nothing, just humouring the interface
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (Time.time - lastClickTime < 0.25)
        {
            doubleClick();
        } else
        {
            lastClickTime = Time.time;
        }
    }

    private void doubleClick()
    {
        Debug.Log("Double click");
        gameState.doubleClick(this, ((Card)(this.GetComponent<Card>())).ac);
    }
}
