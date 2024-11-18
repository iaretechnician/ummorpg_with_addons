using UnityEngine;
using Mirror;

#if _iMMODOORS
// PLAYER
public partial class PlayerAddonsConfigurator
{
    //public Player player;
    //public Combat combat;
    //public Movement mov

    [HideInInspector] public Doors selectedDoor;

    // -----------------------------------------------------------------------------------
    // OnSelect_Door
    // @Client
    // -----------------------------------------------------------------------------------
    [Client]
    public void OnSelect_Door(Doors _selectedDoors)
    {
        selectedDoor = _selectedDoors;
        Cmd_checkDoorAccess(selectedDoor.gameObject);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_checkDoorAccess
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    protected void Cmd_checkDoorAccess(GameObject _selectedDoor)
    {
        selectedDoor = _selectedDoor.GetComponent<Doors>();

        if (DoorValidation())
        {
            Debug.Log(selectedDoor.hingeState);
            if( selectedDoor.hingeState == Doors.DoorState.closed || selectedDoor.hingeState == Doors.DoorState.opened)
                Target_startDoorAccess(player.connectionToClient);
        }
        else
        {
           
            if (selectedDoor != null && selectedDoor.checkInteractionRange(player) && selectedDoor.lockedMessage != "")
            {
                player.Tools_ShowPrompt(selectedDoor.lockedMessage);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // Target_startDoorAccess
    // @Server -> @Client
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    protected void Target_startDoorAccess(NetworkConnection target)
    {
        if (DoorValidation())
        {
            player.Tools_addTask();
            player.Tools_setTimer(selectedDoor.secondsToOpen);
            if(selectedDoor.showProgressBar)
                player.Tools_CastbarShow(selectedDoor.accessLabel, selectedDoor.secondsToOpen);
        }
    }

    // -----------------------------------------------------------------------------------
    // DoorValidation
    // -----------------------------------------------------------------------------------
    public bool DoorValidation()
    {
        bool bValid = (selectedDoor != null &&
            selectedDoor.checkInteractionRange(player) &&
            selectedDoor.interactionRequirements.checkState(player));

        if (!bValid)
            CancelDoor();

        return bValid;
    }

    // -----------------------------------------------------------------------------------
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    private void LateUpdate_Doors()
    {
        if (DoorValidation() && player.Tools_checkTimer())
        {
            player.Tools_removeTask();
            player.Tools_stopTimer();
            if (selectedDoor.showProgressBar)
                player.Tools_CastbarHide();

            Cmd_finishDoorAccess();

        }
    }



    // -----------------------------------------------------------------------------------
    // cancelDoor
    // -----------------------------------------------------------------------------------
    public void CancelDoor()
    {
        if (selectedDoor != null)
        {
            player.Tools_stopTimer();
            player.Tools_removeTask();
            player.Tools_CastbarHide();
            selectedDoor = null;
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_finishDoorAccess
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_finishDoorAccess()
    {
        player.Tools_removeTask();
        player.Tools_stopTimer();
    }
}
#endif