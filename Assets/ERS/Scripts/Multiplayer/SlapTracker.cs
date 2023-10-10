using UnityEngine;
using UnityEngine.Events;

public class SlapTracker : NetworkUI
{
    public UnityEvent slapEvent;
    private void Update()
    {
        if (!IsSpawned)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && !DeckManager.IsPointerOverUI())
        {
            slapEvent?.Invoke();
        }
    }
}
