using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUI : MonoBehaviour
{
    public Text Description;
    public Text Quantity;
    public Image Check;
    public ObjectiveType Type;

    private int _quantity;
    private int _currentQuantity = 0;

    public ObjectiveGoal Goal { get; private set; }

    public void Initialize(ObjectiveGoal goal)
    {
        Check.gameObject.SetActive(false);
        _quantity = goal.Quantity;
        Type = goal.Type;
        Goal = goal;

        Description.text = goal.Description;
        Quantity.text = _currentQuantity + " / " + _quantity;
    }

    public void UpdateObjective()
    {
        _currentQuantity++;
        Quantity.text = _currentQuantity + " / " + _quantity;

        Check.gameObject.SetActive(_currentQuantity >= _quantity);
    }
}
