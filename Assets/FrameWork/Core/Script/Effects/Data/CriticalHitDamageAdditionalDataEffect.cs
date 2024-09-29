using System;

namespace Temporary.Core
{
    [Serializable]
    public class CriticalHitDamageAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"ġ��Ÿ �������� �߰��ϰų� �ٿ��ּ���.";
            }
            else
            {
                return $"ġ��Ÿ ������  {value} �߰�";
            }
        }
    }
}