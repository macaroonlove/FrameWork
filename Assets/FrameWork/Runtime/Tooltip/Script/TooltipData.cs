using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrameWork.Tooltip
{
    [System.Serializable]
    public class TooltipData
    {
        [SerializeField] private List<string> _keys = new List<string>();
        [SerializeField] private List<string> _stringValues = new List<string>();
        [SerializeField] private List<Texture> _textureValues = new List<Texture>();

        private Dictionary<string, string> _stringData = new Dictionary<string, string>();
        private Dictionary<string, Texture> _textureData = new Dictionary<string, Texture>();

        internal Dictionary<string, string> getAllString => _stringData;
        internal Dictionary<string, Texture> getAllTexture => _textureData;

        public void InitializeData()
        {
            _stringData = _keys.Zip(_stringValues, (key, value) => new { key, value })
                             .ToDictionary(x => x.key, x => x.value);
            _textureData = _keys.Zip(_textureValues, (key, value) => new { key, value })
                              .ToDictionary(x => x.key, x => x.value);
        }

        #region 데이터 추가
        internal void Add(string key, string value)
        {
            if (_stringData.ContainsKey(key)) return;

            _keys.Add(key);
            _stringData[key] = value;
            _stringValues = _keys.Select(key => _stringData.ContainsKey(key) ? _stringData[key] : "").ToList();
        }

        internal void Add(string key, Texture value)
        {
            if (_textureData.ContainsKey(key)) return;

            _keys.Add(key);
            _textureData[key] = value;
            _textureValues = _keys.Select(key => _textureData.ContainsKey(key) ? _textureData[key] : null).ToList();
        }
        #endregion

        #region 데이터 수정
        internal void SetString(string key, string value)
        {
            if (_stringData.ContainsKey(key))
            {
                _stringData[key] = value;
                _stringValues = _keys.Select(key => _stringData.ContainsKey(key) ? _stringData[key] : "").ToList();
            }
            else
            {
                Add(key, value);
            }
        }

        internal void SetTexture(string key, Texture value)
        {
            if (_textureData.ContainsKey(key))
            {
                _textureData[key] = value;
                _textureValues = _keys.Select(key => _textureData.ContainsKey(key) ? _textureData[key] : null).ToList();
            }
            else
            {
                Add(key, value);
            }
        }
        #endregion

        internal string GetString(string key) => _stringData.ContainsKey(key) ? _stringData[key] : string.Empty;
        internal Texture GetTexture(string key) => _textureData.ContainsKey(key) ? _textureData[key] : null;

        internal bool IsInitialize()
        {
            return _keys.Count > 0;
        }

        internal bool IsInitializeData()
        {
            return _stringData.Count > 0 || _textureData.Count > 0;
        }
    }
}