using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibrate : MonoBehaviour
{
    private Vector3 originPosition;
    private Quaternion originRotation;
    public float shake_decay = 0.002f;
    public float shake_intensity = .3f;
    public bool shakeNow = false;
    private float temp_shake_intensity = 0;
    AudioSource m_audioSource;
    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (temp_shake_intensity > 0)
        {
            if (!m_audioSource.isPlaying)
            {
                m_audioSource.Play();
            }
            Vector3 tempv3 = Random.insideUnitSphere;
            transform.position = originPosition + new Vector3(tempv3.x, 0, tempv3.z) * temp_shake_intensity;
            transform.rotation = new Quaternion(
                originRotation.x + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
                originRotation.y + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
                originRotation.z + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
                originRotation.w + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f);
            temp_shake_intensity -= shake_decay;
        }
        else
        {
            if (m_audioSource.isPlaying)
            {
                m_audioSource.Pause();
            }
        }
        if (shakeNow)
        {
            Shake();
            shakeNow = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if(other.tag == "Player")
        {
            Shake();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == "Player")
        {
            temp_shake_intensity = 0;
        }
    }

    void Shake()
    {
        originPosition = transform.position;
        originRotation = transform.rotation;
        temp_shake_intensity = shake_intensity;

    }
}
