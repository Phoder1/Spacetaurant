using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Spacetaurant
{
    public class SoundManager : MonoBehaviour
    {
       // public static SoundManager SM;

        [SerializeField]
        private AudioMixer AudioMixer;
        [SerializeField]
        private AudioSource AudioSource_Player_Walk;
        [SerializeField]
        float SpeedPitch;
        [SerializeField]
        float Multiplier;

        private void OnEnable()
        {
            //SM = this;
        }
      
        public void WalkSpeedSound(Vector3 walkSpeed)
        {
            float Pith = walkSpeed.magnitude;
            AudioSource_Player_Walk.pitch = 0.5f + (Pith / SpeedPitch* Multiplier);
            AudioMixer.SetFloat("PitchBlend", 1 / (0.5f + (Pith / SpeedPitch* Multiplier)));
            if (!AudioSource_Player_Walk.isPlaying&& Pith > 1)
            {
                AudioSource_Player_Walk.Play();
            }
            else if (AudioSource_Player_Walk.isPlaying && Pith < 1)
            {
                AudioSource_Player_Walk.Stop();

            }

        }
    }
}
