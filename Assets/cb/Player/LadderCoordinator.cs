using Sirenix.OdinInspector;
using UnityEngine;

class LadderCoordinator : MonoBehaviour
{
    public PlayerController PlayerController;
    public LadderController LadderController;

    [ShowInInspector]
    public int LadderContacts { get; private set; }

    void Start()
    {
        LadderController.enabled = false;
        PlayerController.enabled = true;
    }

    public void MadeContact()
    {
        LadderContacts++;

        if (LadderContacts == 1)
        {
            PlayerController.enabled = false;
            LadderController.enabled = true;
        }
    }

    public void LeftContact()
    {
        LadderContacts--;

        if (LadderContacts == 0)
        {
            PlayerController.enabled = true;
            LadderController.enabled = false;
        }
    }
}