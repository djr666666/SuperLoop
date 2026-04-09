using UnityEngine;
using UnityEngine.UI;
using static Game;

public class Preb_Item : MonoBehaviour
{
   
    public Text txt;

    public void Show(ItemData data)
    {
        txt.text = data.GetDisplayText();
    }
}
