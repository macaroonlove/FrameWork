using UnityEngine;

namespace FrameWork.VisualNovel
{
    public class ChapterTester : MonoBehaviour
    {
        [SerializeField] private ChapterTemplate _chapterTemplate;
        private VisualNovelManager _visualNovelManager;

        private void Awake()
        {
            _visualNovelManager = GetComponent<VisualNovelManager>();
        }

        [ContextMenu("√©≈Õ Ω√¿€")]
        private void Play()
        {
            _visualNovelManager.Load(_chapterTemplate);
        }
    }
}