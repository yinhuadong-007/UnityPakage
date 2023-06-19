using System;
using System.ComponentModel;
using System.Threading;
using System.Globalization;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
namespace ZFrame
{
    /// <summary>音效</summary>
    public class Audio
    {
        /// <summary>音效基础as</summary>
        private static AudioSource oneShotSource;
        private static AudioMixerGroup mixer;

        private static AudioSource bgmSource;

        public static void setMixer(string resName)
        {
            try
            {
                mixer = Res.getRes<UnityEngine.Audio.AudioMixerGroup>(resName);
            }
            catch (Exception ex)
            {
                Debug.Log("ex:"+ex);
                var am = Res.getRes<UnityEngine.Audio.AudioMixer>(resName);
                mixer = am.outputAudioMixerGroup;
            }
        }

        public static void playBGM(string resName)
        {
            if(Res.getRes<AudioClip>(resName)==null){
                return;
            }

            if (bgmSource == null)
            {
                bgmSource = Core.RootNode.AddComponent<AudioSource>();
            }
            if(bgmSource.clip!=null&&bgmSource.clip.GetHashCode()== Res.getRes<AudioClip>(resName).GetHashCode()){
                return;
            }

            bgmSource.loop = true;
            bgmSource.clip = Res.getRes<AudioClip>(resName);
            bgmSource.Play();
        }

        /// <summary>播放音效</summary>
        /// <param name="resName">寻址资源地址名称</param>
        public static void playSound(string resName, bool isLoop = false)
        {
             if(Res.getRes<AudioClip>(resName)==null){
                return;
            }

            if (oneShotSource == null)
            {
                oneShotSource = Core.RootNode.AddComponent<AudioSource>();
                if (mixer != null)
                {
                    oneShotSource.outputAudioMixerGroup = mixer;
                }
            }
            oneShotSource.loop = isLoop;
            oneShotSource.PlayOneShot(Res.getRes<AudioClip>(resName));
        }

        public static void test(string resName)
        {
            MonoSubstitute.instance.SStartCoroutine(bala(resName));
        }

        public static IEnumerator bala(string resName)
        {
            while (true)
            {
                playSound(resName);
                yield return new WaitForSeconds(0.03f);

            }
        }
    }
}