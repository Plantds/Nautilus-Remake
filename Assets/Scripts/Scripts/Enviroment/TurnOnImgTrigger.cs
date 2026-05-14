using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TurnOnImgTrigger : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] float secondsBeforeMenu;

    private void OnTriggerEnter(Collider other)
    {
        // if (image == null)
        // {
        //     image = FindAnyObjectByType<FadeOutImg>().GetComponent<Image>();
        // }

        if (!other.gameObject.CompareTag("Player"))
            return;

        image.enabled = true;


    }

    IEnumerator backToMenu()
    {
        yield return new WaitForSeconds(secondsBeforeMenu);

        //Nothing here :/

        yield return null;   
    }
}
