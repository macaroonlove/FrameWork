using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.VisualNovel
{
    [RequireComponent(typeof(Button))]
    public class UINextButton : MonoBehaviour
    {
        private void Awake()
        {
            var nextButton = GetComponent<Button>();
            nextButton.onClick.AddListener(Next);
        }

        private void Next()
        {
            CommandExecutor.Instance.Next();
        }
    }
}