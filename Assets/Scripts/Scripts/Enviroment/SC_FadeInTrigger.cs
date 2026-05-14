using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SC_FadeInTrigger : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Image text;
    bool fading = false;
    bool triggerd = false;
    bool fadeIn = true;
    bool wait = false;
    bool textFadingIn = true;
    bool textFading = true;
    float fadeTimer;
    [SerializeField] float imgFadeInTime;
    [SerializeField] float timeBetween;
    [SerializeField] float textFadeInTime;
    [SerializeField] float stayTime;
    [SerializeField] float textFadeOutTime;
    [SerializeField] float timeBetween2;
    [SerializeField] float imgFadeOutTime;
    // [SerializeField] FMODUnity.StudioEventEmitter textApearAudio;
    [SerializeField] FMODUnity.StudioEventEmitter fadeOutAudio;


    void Start()
    {
        image.enabled = true;
        text.enabled = true;
        text.CrossFadeAlpha(0.0f,0.0f,false);
        image.CrossFadeAlpha(0.0f,0.0f,false);
    }

    void Update()
    {
        // if (fading)
        // {
        //     if (fadeIn && !wait)
        //     {
        //         image.CrossFadeAlpha(1, imgFadeInTime, false);
        //         wait = true;
        //         fadeIn = false;
        //     }
        //     else if(!fadeIn && !wait)
        //     {
        //         fadeOutAudio.Play();
        //         image.CrossFadeAlpha(0, imgFadeOutTime, false);
        //         fading = false;
        //     }
            
        //     if (wait)
        //     {
        //         if (textFading)
        //         {
        //             if (fadeTimer < stayTime + imgFadeInTime + textFadeInTime && fadeTimer >= imgFadeInTime + timeBetween && textFadingIn)
        //             {
        //                 // textApearAudio.Play();
        //                 text.CrossFadeAlpha(1,textFadeInTime,false);
        //                 textFadingIn = false;
        //             }
        //             else if (fadeTimer >= stayTime + imgFadeInTime + textFadeInTime + timeBetween)
        //             {
        //                 text.CrossFadeAlpha(0,textFadeOutTime,false);
        //                 textFading = false;
        //             }
        //         }
                

        //         if (fadeTimer < stayTime + imgFadeInTime + textFadeInTime + textFadeOutTime + timeBetween + timeBetween2)
        //         {
        //             fadeTimer += Time.deltaTime;
        //         }

        //         if (fadeTimer >= stayTime + imgFadeInTime + textFadeInTime + textFadeOutTime + timeBetween + timeBetween2)
        //         {
        //             wait = false;
        //             fadeTimer = 0;
        //         }
        //     }

        // }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("SubmarineCollider"))
            return;
        
        if (!triggerd)
        {
            fading = true;
            triggerd = true;


            StartCoroutine(fadeInOut());
        }
    }

    IEnumerator fadeInOut()
    {
        image.CrossFadeAlpha(1, imgFadeInTime, false);
        yield return new WaitForSeconds(imgFadeInTime);
        yield return new WaitForSeconds(timeBetween);
        text.CrossFadeAlpha(1, textFadeInTime, false);
        yield return new WaitForSeconds(textFadeInTime);
        yield return new WaitForSeconds(stayTime);
        text.CrossFadeAlpha(0, textFadeOutTime, false);
        yield return new WaitForSeconds(textFadeOutTime);
        yield return new WaitForSeconds(timeBetween2);
        image.CrossFadeAlpha(0, imgFadeOutTime, false);
        fadeOutAudio.Play();
        yield return null;
    }
}
