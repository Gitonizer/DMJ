using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemParent : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Character>() != null)
        {
            if (other.gameObject.GetComponent<Character>().CharacterType == CharacterType.Player)
            {
                EventManager.OnClose?.Invoke();
            }
        }
    }
}
