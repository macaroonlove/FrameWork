using UnityEngine;

namespace FrameWork.UIPopup
{
    [RequireComponent(typeof(CanvasGroupController))]
    public class UIPopup : MonoBehaviour
    {
        private CanvasGroupController _controller;

        protected virtual void Awake()
        {
            _controller = GetComponent<CanvasGroupController>();
        }

        public virtual void Show(bool isForce = false)
        {
            if (_controller == null)
            {
                _controller = GetComponent<CanvasGroupController>();
            }

            _controller.Show(isForce);
        }

        public virtual void Hide(bool isForce = false)
        {
            if (_controller == null)
            {
                _controller = GetComponent<CanvasGroupController>();
            }

            _controller.Hide(isForce);
        }
    }
}