# UnityDragDropFragments
Here are a couple of C# scripts to jump start a drag-drop project in Unity. They came from an incomplete and rather messy card game prototype and haven't been cleaned up at all, so you will need to tidy them up for your own use, but they do contain some useful clues.

They consist of a Draggable component that should be attached to each card and a DropZone component that should be attached to places where cards can validly be dropped. You will need a GameState component attached to a permanent object in the scene that handles your game logic, including making decisions about whether a card is allowed to be picked up or dropped and what happens when it is.
