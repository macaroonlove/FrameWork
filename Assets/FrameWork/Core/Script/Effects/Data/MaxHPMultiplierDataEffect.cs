using System;

namespace Temporary.Core
{
    [Serializable]
    public class MaxHPMultiplierDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"�ִ� ü���� ��¡��϶� �����ּ���.";
            }
            else if (value > 0)
            {
                return $"�ִ� ü��  {value * 100}% ���";
            }
            else
            {
                return $"�ִ� ü��  {value * 100}% �϶�";
            }
        }
    }
}