using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Northwind.Essentials
{
    public class Audio : Object
    {
        public static AudioSource PlayAtPosition(AudioClip clip, Vector3 point, float spatialBlend = 1f, float minDistance = 1f)
        {
            GameObject tempObj = new GameObject("Audio Clip");
            tempObj.transform.position = point;

            AudioSource source = tempObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = spatialBlend;
            source.minDistance = minDistance;
            source.clip = clip;

            source.Play();
            Destroy(tempObj, clip.length);
            return source;
        }
    }
}