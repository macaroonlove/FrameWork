using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FrameWork
{
    public class AddressableAssetManager : Singleton<AddressableAssetManager>
    {
        private Dictionary<string, AsyncOperationHandle<Sprite>> _sprites = new Dictionary<string, AsyncOperationHandle<Sprite>>();
        private Dictionary<string, AsyncOperationHandle<AudioClip>> _audioClip = new Dictionary<string, AsyncOperationHandle<AudioClip>>();

        #region Sprite
        public void GetSprite(string key, UnityAction<Sprite> onComplete)
        {
            if (string.IsNullOrEmpty(key) || _sprites.ContainsKey(key))
            {
                onComplete?.Invoke(_sprites[key].Result);
                return;
            }

            Addressables.LoadAssetAsync<Sprite>(key).Completed += (AsyncOperationHandle<Sprite> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _sprites.TryAdd(key, handle);
                    onComplete?.Invoke(handle.Result);
                }
                else
                {
                    Addressables.Release(handle);
                }
            };
        }

        public void ReleaseSprite(string key)
        {
            if (_sprites.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
                _sprites.Remove(key);
            }
        }

        public void ReleaseAllSprites()
        {
            foreach (var handle in _sprites.Values)
            {
                Addressables.Release(handle);
            }
            _sprites.Clear();
        }
        #endregion

        #region AudioClip
        public void GetAudioClip(string key, UnityAction<AudioClip> onComplete)
        {
            if (string.IsNullOrEmpty(key) || _audioClip.ContainsKey(key))
            {
                onComplete?.Invoke(_audioClip[key].Result);
                return;
            }

            Addressables.LoadAssetAsync<AudioClip>(key).Completed += (AsyncOperationHandle<AudioClip> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _audioClip[key] = handle;
                    onComplete?.Invoke(handle.Result);
                }
                else
                {
                    Addressables.Release(handle);
                }
            };
        }

        public void ReleaseAudioClip(string key)
        {
            if (_audioClip.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
                _audioClip.Remove(key);
            }
        }

        public void ReleaseAllAudioClips()
        {
            foreach (var handle in _audioClip.Values)
            {
                Addressables.Release(handle);
            }
            _audioClip.Clear();
        }
        #endregion

        public void ReleaseAll()
        {
            ReleaseAllSprites();
            ReleaseAllAudioClips();
        }

        private void OnDestroy()
        {
            ReleaseAll();
        }
    }
}
