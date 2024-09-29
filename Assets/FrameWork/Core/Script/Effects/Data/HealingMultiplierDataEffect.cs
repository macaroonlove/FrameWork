using System;

namespace Temporary.Core
{
    [Serializable]
    public class HealingMultiplierDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"ȸ������ ��¡��϶� �����ּ���.";
            }
            else if (value > 0)
            {
                return $"ȸ����  {value * 100}% ���";
            }
            else
            {
                return $"ȸ����  {value * 100}% �϶�";
            }
        }
    }
}