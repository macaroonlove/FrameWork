using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/Unit/Skin", fileName = "Skin_Lobby_", order = 0)]
    public class SkinLobbyTemplate : ScriptableObject
    {
        [Header("스프라이트")]
        public Sprite sprite_FullBody;

        [Header("보이스")]
        public AudioClip voice_OnInfo;
        public AudioClip voice_OnGet;
        public AudioClip voice_OnTalk1;
        public AudioClip voice_OnTalk2;
        public AudioClip voice_OnTalk3;
    }
}