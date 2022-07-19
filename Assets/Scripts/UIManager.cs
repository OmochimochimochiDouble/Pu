using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject changeTurnPanel;
    [SerializeField] Text changeTurnText;
    [SerializeField] Image changeTurnPanelImage;

    public IEnumerator ShowChangeTurnPanel()
    {
        changeTurnPanel.SetActive(true);
 
        if (GameManager.instance.isWin == true)
        {
            if (GameManager.instance.isPlayerWin == true)
            {
                changeTurnText.text = "You Win";
                Color imageColor = changeTurnPanelImage.color;
                Color textColor = changeTurnText.color;
                imageColor = new Color(0, 1, 1, 1);
                textColor.a = 1;
                changeTurnPanelImage.color = imageColor;
                changeTurnText.color = textColor;
                yield return new WaitForSeconds(100f);
            }
            else
            {
                changeTurnText.text = "You Lose";
                Color imageColor = changeTurnPanelImage.color;
                Color textColor = changeTurnText.color;
                imageColor = new Color(1, 0, 1, 1);
                textColor.a = 1;
                changeTurnPanelImage.color = imageColor;
                changeTurnText.color = textColor;
                yield return new WaitForSeconds(100f);
            }
        }
        else
        {
            if (GameManager.instance.isPlayerTurn == true)
            {
                changeTurnText.text = "Your Turn";
                Color imageColor = changeTurnPanelImage.color;
                imageColor = new Color(0, 1, 1, 0);
                changeTurnPanelImage.color = imageColor;
            }
            else
            {
                changeTurnText.text = "Enemy Turn";
                Color imageColor = changeTurnPanelImage.color;
                imageColor = new Color(1, 0, 1, 0);
                changeTurnPanelImage.color = imageColor;
            }
        }

        for (float f = 0f; f <= 0.5f; f += 0.05f) 
        {
            Color imageColor = changeTurnPanelImage.color;
            Color textColor = changeTurnText.color;
            imageColor.a = f;
            textColor.a = f * 2;
            changeTurnPanelImage.color = imageColor;
            changeTurnText.color = textColor;
            yield return new WaitForSeconds(.02f);
        }

        yield return new WaitForSeconds(.2f);

        for (float f = 0.5f; f >= 0f; f -= 0.05f) 
        {
            Color imageColor = changeTurnPanelImage.color;
            Color textColor = changeTurnText.color;
            imageColor.a = f;
            textColor.a = f * 2;
            changeTurnPanelImage.color = imageColor;
            changeTurnText.color = textColor;
            yield return new WaitForSeconds(.02f);
        }

        changeTurnPanel.SetActive(false);
    }
}