using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownAction : MonoBehaviour
{
    public Image statusColor;
    public TMPro.TMP_Dropdown dropDownStatus;

    // Method to update the switch status and color
    public void SwitchStatus()
    {
        if (dropDownStatus.value == 0)
            statusColor.color = Color.green;
        else if (dropDownStatus.value == 1)
            statusColor.color = Color.red;
    }

    // Method to return the switch status value
    public int GetSwitchStatus()
    {
        return dropDownStatus.value; // Returns 0 or 1 based on the dropdown selection
    }
}
