using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]private AudioSource[] audioSource;

    private void Awake()
    {
        if(instance!=null&&instance!=this)
        {
            Destroy(gameObject);
             return;
        }
        instance=this;
    }

    public void PlaySFX(int index)
    {
        if (index < 0 || index >= audioSource.Length) return;
       
        var source = audioSource[index];
        if(source==null) return;

        if(source.clip!=null)
        {
            source.PlayOneShot(source.clip);
        }
        else source.Play();
    }

    public void StopSFX(int index)
    {
        if (index < 0 || index >= audioSource.Length) return;
       
        var source = audioSource[index];
        if(source==null) return;
        source.Stop();
    }
}
