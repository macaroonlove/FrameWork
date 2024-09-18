using UnityEngine;

namespace FrameWork.Editor
{
    public class PopupAttribute : PropertyAttribute
    {
        public object[] options;

        public PopupAttribute(params object[] options)
        {
            this.options = options;
        }
    }
}