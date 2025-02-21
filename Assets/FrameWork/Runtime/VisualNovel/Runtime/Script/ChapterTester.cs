using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FrameWork.VisualNovel
{
    public class ChapterTester : MonoBehaviour
    {
        [SerializeField] private AssetReferenceT<ChapterTemplate> _chapterTemplate;
        private VisualNovelManager _visualNovelManager;

        private void Awake()
        {
            _visualNovelManager = GetComponent<VisualNovelManager>();
        }

        [ContextMenu("√©≈Õ Ω√¿€")]
        private void Play()
        {
            AddressableAssetManager.Instance.GetScriptableObject<ChapterTemplate>(_chapterTemplate.RuntimeKey.ToString(), (template) =>
            {
                _visualNovelManager.Load(template);
            });
        }
    }
}