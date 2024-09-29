using System;

namespace Temporary.Core
{
    [Serializable]
    public class ReceiveDamageMultiplierDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"�޴� ���ط��� ��¡��϶� �����ּ���.";
            }
            else if (value > 0)
            {
                return $"�޴� ���ط�  {value * 100}% ���";
            }
            else
            {
                return $"�޴� ���ط�  {value * 100}% �϶�";
            }
        }
    }
}